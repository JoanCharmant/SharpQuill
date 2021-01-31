using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// A QuillSequenceReader reads a quill project folder and imports it in a Sequence object.
  /// A project consists in a json-based scene hierarchy, a json-based state file, and binary data.
  /// </summary>
  public static class QuillSequenceReader
  {
    public static Sequence Read(string path)
    {
      if (string.IsNullOrEmpty(path))
        return null;

      if (!Directory.Exists(path))
        return null;

      string sequenceFilename = Path.Combine(path, "Quill.json");
      if (!File.Exists(sequenceFilename))
        return null;

      string paintDataFilename = Path.Combine(path, "Quill.qbin");
      if (!File.Exists(paintDataFilename))
        return null;

      string json = File.ReadAllText(sequenceFilename);
      dynamic document = JsonConvert.DeserializeObject(json);
      Sequence seq = Parse(document);

      // Now that we have loaded the scene hierarchy, read the actual attached data from the qbin.
      using (Stream stream = File.OpenRead(paintDataFilename))
      {
        QBinReader qbinReader = new QBinReader(stream);
        seq.LastStrokeId = qbinReader.ReadUInt32();
        ReadLayerData(seq.RootLayer, qbinReader);
      }

      return seq;
    }

    private static Sequence Parse(dynamic s)
    {
      Sequence seq = new Sequence();
      seq.Metadata = ParseMetadata(s.Sequence.Metadata);
      seq.Gallery = ParseGallery(s.Sequence.Gallery);
      seq.BackgroundColor = ParseColor(s.Sequence.BackgroundColor);
      if (s.Sequence.DefaultViewpoint != null)
        seq.DefaultViewpoint = s.Sequence.DefaultViewpoint.ToObject(typeof(string));
      else
        seq.DefaultViewpoint = ""; // "Root/InitialSpawnArea"
      seq.Framerate = s.Sequence.Framerate;
      seq.ExportStart = s.Sequence.ExportStart;
      seq.ExportEnd = s.Sequence.ExportEnd;
      seq.CameraResolution = ParseSize(s.Sequence.CameraResolution);
      seq.RootLayer = ParseLayer(s.Sequence.RootLayer);
      return seq;
    }

    private static Metadata ParseMetadata(dynamic m)
    {
      Metadata metadata = new Metadata();

      metadata.Title = m.Title.ToObject(typeof(string));
      metadata.Description = m.Description.ToObject(typeof(string));
      if (m.ThumbnailCropPosition != null)
        metadata.ThumbnailCropPosition = m.ThumbnailCropPosition.ToObject(typeof(float));
      
      return metadata;
    }

    private static Gallery ParseGallery(dynamic g)
    {
      Gallery gallery = new Gallery();
      
      // TODO.
      // ParseThumbnails: object.
      // ParsePictures: list.
      // Picture: Type, DataFileOffset, Metadata: VerticalFOV, HorizontalFOV.

      return gallery;
    }

    private static Color ParseColor(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 3)
        throw new InvalidDataException();

      return new Color(value);
    }

    private static Size ParseSize(JArray jValue)
    {
      List<int> value = jValue.ToObject<List<int>>();

      if (value.Count != 2)
        throw new InvalidDataException();

      return new Size(value);
    }

    private static Vector3 ParseVector3(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 3)
        throw new InvalidDataException();

      return new Vector3(value);
    }

    private static Vector4 ParseVector4(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 4)
        throw new InvalidDataException();

      return new Vector4(value);
    }

    private static Quaternion ParseQuaternion(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 4)
        throw new InvalidDataException();

      return new Quaternion(value);
    }

    private static Transform ParseTransform(dynamic t)
    {
      Transform transform = Transform.Identity;

      transform.Rotation = ParseQuaternion(t.Rotation);
      transform.Scale = t.Scale;
      transform.Flip = t.Flip;
      transform.Translation = ParseVector3(t.Translation);
      
      return transform;
    }

    private static BoundingBox ParseBoundingBox(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 6)
        throw new InvalidDataException();

      return new BoundingBox(value);
    }

    /// <summary>
    /// Parse the drawing metadata. The actual strokes will be read from the qbin later.
    /// </summary>
    private static Drawing ParseDrawing(dynamic d)
    {
      Drawing drawing = new Drawing();
      drawing.BoundingBox = ParseBoundingBox(d.BoundingBox);

      long offset;
      bool parsed = long.TryParse((string)d.DataFileOffset.ToObject(typeof(string)), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out offset);
      drawing.DataFileOffset = parsed ? offset : -1;

      return drawing;
    }
    
    private static KeepAlive ParseKeepAlive(dynamic ka)
    {
      KeepAlive keepAlive = new KeepAlive();
      keepAlive.Type = ParseEnum<KeepAliveType>(ka.Type);
      return keepAlive;
    }

    private static Animation ParseAnimation(dynamic a)
    {
      Animation animation = new Animation();

      animation.Duration = a.Duration;
      animation.Timeline = a.Timeline;
      animation.StartOffset = a.StartOffset;
      animation.MaxRepeatCount = a.MaxRepeatCount;

      animation.Keys = ParseKeyframes(a.Keys);
      
      return animation;
    }

    /// <summary>
    /// Parse one enum. If not found this will return the default value.
    /// </summary>
    private static T ParseEnum<T>(dynamic v) where T : struct
    {
      Enum.TryParse((string)v.ToObject(typeof(string)), out T result);
      return result;
    }
    
    /// <summary>
    /// Parse all the keyframe channels data.
    /// </summary>
    private static Keyframes ParseKeyframes(dynamic kkff)
    {
      Keyframes keyframes = new Keyframes();

      if (kkff.Visibility != null)
      {
        foreach (var kf in kkff.Visibility)
        {
          Keyframe<bool> keyframe = new Keyframe<bool>();
          keyframe.Time = kf.Time;
          keyframe.Value = kf.Value;
          keyframe.Interpolation = ParseEnum<Interpolation>(kf.Interpolation);
          keyframes.Visibility.Add(keyframe);
        }
      }

      if (kkff.Offset != null)
      {
        foreach (var kf in kkff.Offset)
        {
          Keyframe<int> keyframe = new Keyframe<int>();
          keyframe.Time = kf.Time;
          keyframe.Value = kf.Value;
          keyframe.Interpolation = ParseEnum<Interpolation>(kf.Interpolation);
          keyframes.Offset.Add(keyframe);
        }
      }

      if (kkff.Opacity != null)
      {
        foreach (var kf in kkff.Opacity)
        {
          Keyframe<float> keyframe = new Keyframe<float>();
          keyframe.Time = kf.Time;
          keyframe.Value = kf.Value;
          keyframe.Interpolation = ParseEnum<Interpolation>(kf.Interpolation);
          keyframes.Opacity.Add(keyframe);
        }
      }

      if (kkff.Transform != null)
      {
        foreach (var kf in kkff.Transform)
        {
          Keyframe<Transform> keyframe = new Keyframe<Transform>();
          keyframe.Time = kf.Time;
          keyframe.Value = ParseTransform(kf.Value);
          keyframe.Interpolation = ParseEnum<Interpolation>(kf.Interpolation);
          keyframes.Transform.Add(keyframe);
        }
      }

      return keyframes;
    }

    /// <summary>
    /// Parse one layer. Drill down recursively for groups.
    /// </summary>
    private static Layer ParseLayer(dynamic l)
    {
      if (l == null)
        return null;

      Layer layer;
      LayerType type = ParseEnum<LayerType>(l.Type);
      switch (type)
      {
        case LayerType.Group:
          {
            layer = new LayerGroup();
            LayerGroup lg = layer as LayerGroup;
            foreach (var c in l.Implementation.Children)
            {
              Layer child = ParseLayer(c);
              if (child != null)
                lg.Children.Add(child);
            }

            break;
          }
        case LayerType.Paint:
          {
            layer = new LayerPaint();
            LayerPaint lp = layer as LayerPaint;
            lp.Framerate = l.Implementation.Framerate;
            lp.MaxRepeatCount = l.Implementation.MaxRepeatCount;
            if (l.Implementation.Drawings != null && l.Implementation.Frames != null)
            {
              foreach (var d in l.Implementation.Drawings)
              {
                Drawing drawing = ParseDrawing(d);
                if (drawing != null)
                  lp.Drawings.Add(drawing);
              }

              lp.Frames = l.Implementation.Frames.ToObject<List<int>>();
            }

            break;
          }
        case LayerType.Viewpoint:
          {
            layer = new LayerViewpoint();
            LayerViewpoint lv = layer as LayerViewpoint;
            lv.Version = l.Implementation.Version;
            lv.Color = ParseColor(l.Implementation.Color);
            lv.Sphere = ParseVector4(l.Implementation.Sphere);
            lv.AllowTranslationX = l.Implementation.AllowTranslationX;
            lv.AllowTranslationY = l.Implementation.AllowTranslationY;
            lv.AllowTranslationZ = l.Implementation.AllowTranslationZ;
            lv.Exporting = l.Implementation.Exporting;
            lv.ShowingVolume = l.Implementation.ShowingVolume;
            lv.TypeStr = l.Implementation.TypeStr;
            break;
          }
        case LayerType.Camera:
          {
            layer = new LayerCamera();
            LayerCamera lc = layer as LayerCamera;
            lc.FOV = l.Implementation.FOV;
            break;
          }
        case LayerType.Model:
        case LayerType.Picture:
        case LayerType.Sound:
        case LayerType.Unknown:
        default:
          layer = null;
          break;
      }

      if (layer != null)
        ParseLayerCommon(layer, l);

      return layer;
    }

    /// <summary>
    /// Parse the common part of the layer info.
    /// </summary>
    private static void ParseLayerCommon(Layer layer, dynamic l)
    {
      layer.Name = l.Name;
      layer.Visible = l.Visible;
      layer.Locked = l.Locked;
      layer.Collapsed = l.Collapsed;
      layer.BBoxVisible = l.BBoxVisible;
      layer.Opacity = l.Opacity;
      layer.IsModelTopLayer = l.IsModelTopLayer;
      layer.KeepAlive = ParseKeepAlive(l.KeepAlive);
      layer.Transform = ParseTransform(l.Transform);
      layer.Pivot = ParseTransform(l.Pivot);
      layer.Animation = ParseAnimation(l.Animation);
    }

    /// <summary>
    /// Recursive function reading binary data for the entire hierarchy.
    /// </summary>
    private static void ReadLayerData(Layer layer, QBinReader qbinReader)
    {
      if (layer.Type == LayerType.Group)
      {
        foreach (Layer l in ((LayerGroup)layer).Children)
          ReadLayerData(l, qbinReader);
      }
      else if (layer.Type == LayerType.Paint)
      {
        foreach (Drawing drawing in ((LayerPaint)layer).Drawings)
        {
          qbinReader.BaseStream.Seek(drawing.DataFileOffset, SeekOrigin.Begin);
          drawing.Data = qbinReader.ReadDrawingData();
        }
      }
    }
  }
}
