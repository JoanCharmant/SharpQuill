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
  /// A project consist in a json manifest and binary data.
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

      // Now that we have loaded the entire hierarchy, read the actual attached data from the qbin.
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

      KeepAliveType keepAliveType;
      bool parsed = Enum.TryParse((string)ka.Type.ToObject(typeof(string)), out keepAliveType);
      keepAlive.Type = parsed ? keepAliveType : KeepAliveType.None;
      
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


    private static T ParseEnum<T>(dynamic v) where T : struct
    {
      Enum.TryParse((string)v.ToObject(typeof(string)), out T result);
      return result;
    }
    

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

    private static Layer ParseLayer(dynamic l)
    {
      Layer layer = new Layer();
      layer.Name = l.Name;
      layer.Visible = l.Visible;
      layer.Locked = l.Locked;
      layer.Collapsed = l.Collapsed;
      layer.BBoxVisible = l.BBoxVisible;
      layer.Opacity = l.Opacity;

      bool parsed = Enum.TryParse((string)l.Type.ToObject(typeof(string)), out LayerType layerType);
      layer.Type = parsed ? layerType : LayerType.Unknown;

      layer.IsModelTopLayer = l.IsModelTopLayer;
      layer.KeepAlive = ParseKeepAlive(l.KeepAlive);
      layer.Transform = ParseTransform(l.Transform);
      layer.Pivot = ParseTransform(l.Pivot);
      layer.Animation = ParseAnimation(l.Animation);
      
      layer.Implementation = ParseLayerImplementation(l.Implementation, layer.Type);
      return layer;
    }

    private static LayerImplementation ParseLayerImplementation(dynamic li, LayerType type)
    {
      LayerImplementation result = null;

      switch (type)
      {
        case LayerType.Group:
        {
            LayerImplementationGroup impl = new LayerImplementationGroup();
            foreach (var c in li.Children)
              impl.Children.Add(ParseLayer(c));

            result = impl;
            break;
        }
        case LayerType.Paint:
        {
            LayerImplementationPaint impl = new LayerImplementationPaint();

            impl.Framerate = li.Framerate;
            impl.MaxRepeatCount = li.MaxRepeatCount;

            foreach (var d in li.Drawings)
              impl.Drawings.Add(ParseDrawing(d));

            impl.Frames = li.Frames.ToObject<List<int>>();
            
            result = impl;
            break;
        }
        case LayerType.Picture:
          {
            //LayerImplementationPicture impl = new LayerImplementationPicture();
            


            //PictureMode mode;
            //bool parsed = Enum.TryParse((string)li.Mode.ToObject(typeof(string)), out mode);
            //impl.Mode = parsed ? mode : PictureMode.Unknown;

            //impl.Filename = li.Filename;
            //result = impl;
            break;
          }
        case LayerType.Sound:
          {
            
            //LayerImplementationSound impl = new LayerImplementationSound();


            //impl.Duration = li.Duration;
            //impl.Volume = li.Volume;
            //impl.AttenMode = li.AttenMode;
            //impl.AttenMin = li.AttenMin;
            //impl.AttenMax = li.AttenMax;
            //impl.Loop = li.Loop;
            //impl.IsSpatialized = li.IsSpatialized;
            //impl.Play = li.Play;
            //impl.Filename = li.Filename;

            //result = impl;
            break;
          }
        case LayerType.Viewpoint:
          {

            break;
          }
        case LayerType.Model:
          {

            break;
          }
      }

      return result;
    }

    /// <summary>
    /// Recursive function reading binary data for the entire hierarchy.
    /// </summary>
    private static void ReadLayerData(Layer layer, QBinReader qbinReader)
    {
      if (layer.Type == LayerType.Group)
      {
        foreach (Layer l in ((LayerImplementationGroup)layer.Implementation).Children)
          ReadLayerData(l, qbinReader);
      }
      else if (layer.Type == LayerType.Paint)
      {
        LayerImplementationPaint lip = layer.Implementation as LayerImplementationPaint;
        foreach (Drawing drawing in lip.Drawings)
        {
          qbinReader.BaseStream.Seek(drawing.DataFileOffset, SeekOrigin.Begin);
          drawing.Data = qbinReader.ReadDrawingData();
        }
      }
    }
  }
}
