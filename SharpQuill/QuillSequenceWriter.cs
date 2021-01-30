using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// A QuillSequenceWriter writes the scene hierarchy and data to a quill project folder.
  /// </summary>
  public static class QuillSequenceWriter
  {
    public static void Write(Sequence seq, string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new InvalidOperationException();

      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);

      string sequenceFilename = Path.Combine(path, "Quill.json");
      string paintDataFilename = Path.Combine(path, "Quill.qbin");
      string stateFilename = Path.Combine(path, "State.json");

      // Write the qbin file first to update the data offsets.
      using (FileStream qbinStream = File.Create(paintDataFilename))
      {
        QBinWriter qbinWriter = new QBinWriter(qbinStream);
        WriteLastStrokeId(seq, qbinWriter);
        WriteDrawingData(seq.RootLayer, qbinWriter);
        qbinWriter.Flush();
      }

      WriteManifest(seq, sequenceFilename);
      WriteState(stateFilename);
    }

    private static void WriteManifest(Sequence seq, string path)
    {
      // Ref: http://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm
      JObject root = new JObject();
      root.Add(new JProperty("Version", 1));
      root.Add(new JProperty("Sequence", WriteSequence(seq)));
      
      // Note: the formatter will fully indent arrays of primitives.
      string json = JsonConvert.SerializeObject(root, Formatting.Indented);
      File.WriteAllText(path, json);
    }

    private static JObject WriteSequence(Sequence seq)
    {
      JObject jSeq = new JObject();
      jSeq.Add(new JProperty("Metadata", WriteMetadata(seq.Metadata)));
      jSeq.Add(new JProperty("Gallery", WriteGallery(seq.Gallery)));
      jSeq.Add(new JProperty("BackgroundColor", WriteColor(seq.BackgroundColor)));
      jSeq.Add(new JProperty("DefaultViewpoint", seq.DefaultViewpoint));
      jSeq.Add(new JProperty("RootLayer", WriteLayer(seq.RootLayer)));
      return jSeq;
    }

    private static JObject WriteMetadata(Metadata value)
    {
      JObject jObj = new JObject();

      jObj.Add(new JProperty("Title", value.Title));
      jObj.Add(new JProperty("Description", value.Description));
      jObj.Add(new JProperty("ThumbnailCropPosition", value.ThumbnailCropPosition));
      
      return jObj;
    }

    private static JObject WriteGallery(Gallery value)
    {
      JObject jObj = new JObject();

      if (value.Pictures.Count == 0)
      {
        jObj.Add(new JProperty("Thumbnails", new JObject()));
        jObj.Add(new JProperty("Pictures", new JArray()));
      }
      else
      {
        throw new NotImplementedException();
      }
      
      return jObj;
    }

    private static JObject WriteLayer(Layer layer)
    {
      JObject jLayer = new JObject();

      jLayer.Add(new JProperty("Name", layer.Name));
      jLayer.Add(new JProperty("Visible", layer.Visible));
      jLayer.Add(new JProperty("Locked", layer.Locked));
      jLayer.Add(new JProperty("Collapsed", layer.Collapsed));
      jLayer.Add(new JProperty("BBoxVisible", layer.BBoxVisible));
      jLayer.Add(new JProperty("Opacity", layer.Opacity));
      jLayer.Add(new JProperty("Type", layer.Type.ToString()));
      jLayer.Add(new JProperty("IsModelTopLayer", layer.IsModelTopLayer));
      jLayer.Add(new JProperty("KeepAlive", WriteKeepAlive(layer.KeepAlive)));
      jLayer.Add(new JProperty("Transform", WriteTransform(layer.Transform)));
      jLayer.Add(new JProperty("Pivot", WriteTransform(layer.Pivot)));
      jLayer.Add(new JProperty("Animation", WriteAnimation(layer.Animation)));
      jLayer.Add(new JProperty("Implementation", WriteLayerImplementation(layer.Implementation, layer.Type)));
      return jLayer;
    }

    private static JObject WriteLayerImplementation(LayerImplementation impl, LayerType type)
    {
      switch (type)
      {
        case LayerType.Group:
          return WriteLayerImplementationGroup(impl as LayerImplementationGroup);
        case LayerType.Paint:
          return WriteLayerImplementationPaint(impl as LayerImplementationPaint);
        case LayerType.Picture:
          return null;
          //return WriteLayerImplementationPicture(impl as LayerImplementationPicture);
        case LayerType.Sound:
          return null;
          //return WriteLayerImplementationSound(impl as LayerImplementationSound);
        case LayerType.Viewpoint:
          return WriteLayerImplementationViewpoint(impl as LayerImplementationViewpoint);
        case LayerType.Model:
          return null;
        case LayerType.Camera:
          return WriteLayerImplementationCamera(impl as LayerImplementationCamera);
        default:
          return null;
      }
    }

    private static bool IsSupported(Layer layer)
    {
      if (layer == null)
        return false;

      return layer.Type == LayerType.Group ||
             layer.Type == LayerType.Paint ||
             layer.Type == LayerType.Camera ||
             layer.Type == LayerType.Viewpoint;
    }

    private static JObject WriteLayerImplementationGroup(LayerImplementationGroup impl)
    {
      JObject jLayer = new JObject();
      JArray jChildren = new JArray();

      foreach (Layer child in impl.Children)
      {
        if (IsSupported(child))
          jChildren.Add(WriteLayer(child));
      }

      jLayer.Add(new JProperty("Children", jChildren));

      return jLayer;
    }

    private static JObject WriteLayerImplementationPaint(LayerImplementationPaint impl)
    {
      JObject jLayer = new JObject();

      jLayer.Add(new JProperty("Framerate", impl.Framerate));
      jLayer.Add(new JProperty("MaxRepeatCount", impl.MaxRepeatCount));

      JArray jDrawings = new JArray();
      foreach (Drawing drawing in impl.Drawings)
        jDrawings.Add(WriteDrawing(drawing));

      jLayer.Add(new JProperty("Drawings", jDrawings));

      JArray jFrames = new JArray();
      foreach (float frame in impl.Frames)
        jFrames.Add(frame);

      jLayer.Add(new JProperty("Frames", jFrames));
      
      return jLayer;
    }

    private static JObject WriteLayerImplementationCamera(LayerImplementationCamera impl)
    {
      JObject jLayer = new JObject();
      jLayer.Add(new JProperty("FOV", impl.FOV));
      return jLayer;
    }

    private static JObject WriteLayerImplementationViewpoint(LayerImplementationViewpoint impl)
    {
      JObject jLayer = new JObject();
      jLayer.Add(new JProperty("Version", impl.Version));
      jLayer.Add(new JProperty("Color", WriteColor(impl.Color)));
      jLayer.Add(new JProperty("Sphere", WriteVector4(impl.Sphere)));
      jLayer.Add(new JProperty("AllowTranslationX", impl.AllowTranslationX));
      jLayer.Add(new JProperty("AllowTranslationY", impl.AllowTranslationY));
      jLayer.Add(new JProperty("AllowTranslationZ", impl.AllowTranslationZ));
      jLayer.Add(new JProperty("Exporting", impl.Exporting));
      jLayer.Add(new JProperty("ShowingVolume", impl.ShowingVolume));
      jLayer.Add(new JProperty("TypeStr", impl.TypeStr));
      return jLayer;
    }

    private static JObject WriteLayerImplementationPicture(LayerImplementationPicture impl)
    {
      JObject jLayer = new JObject();

      throw new NotImplementedException();
      //jLayer.Add(new JProperty("Mode", impl.Mode.ToString()));
      //jLayer.Add(new JProperty("DataFile", impl.Filename));

      return jLayer;
    }
    
    private static JObject WriteLayerImplementationSound(LayerImplementationSound impl)
    {
      JObject jLayer = new JObject();

      throw new NotImplementedException();
      //jLayer.Add(new JProperty("Duration", impl.Duration));
      //jLayer.Add(new JProperty("Volume", impl.Volume));
      //jLayer.Add(new JProperty("AttenMode", impl.AttenMode));
      //jLayer.Add(new JProperty("AttenMin", impl.AttenMin));
      //jLayer.Add(new JProperty("AttenMax", impl.AttenMax));
      //jLayer.Add(new JProperty("Loop", impl.Loop));
      //jLayer.Add(new JProperty("IsSpatialized", impl.IsSpatialized));
      //jLayer.Add(new JProperty("Play", impl.Play));
      //jLayer.Add(new JProperty("File", impl.Filename));

      return jLayer;
    }

    private static JArray WriteColor(Color value)
    {
      return new JArray(value.R, value.G, value.B);
    }

    private static JArray WriteVector3(Vector3 value)
    {
      return new JArray(value.X, value.Y, value.Z);
    }

    private static JArray WriteVector4(Vector4 value)
    {
      return new JArray(value.X, value.Y, value.Z, value.W);
    }

    private static JArray WriteQuaternion(Quaternion value)
    {
      return new JArray(value.X, value.Y, value.Z, value.W);
    }

    private static JObject WriteTransform(Transform value)
    {
      JObject jT = new JObject();

      jT.Add(new JProperty("Rotation", WriteQuaternion(value.Rotation)));
      jT.Add(new JProperty("Scale", value.Scale));
      jT.Add(new JProperty("Flip", value.Flip));
      jT.Add(new JProperty("Translation", WriteVector3(value.Translation)));

      return jT;
    }

    private static JArray WriteBoundingBox(BoundingBox value)
    {
      return new JArray(value.MinX, value.MaxX, value.MinY, value.MaxY, value.MinZ, value.MaxZ);
    }

    private static JObject WriteKeepAlive(KeepAlive value)
    {
      JObject jKA = new JObject();

      jKA.Add(new JProperty("Type", value.Type.ToString()));

      return jKA;
    }

    private static JObject WriteAnimation(Animation value)
    {
      JObject jAnimation = new JObject();

      jAnimation.Add(new JProperty("Duration", value.Duration));
      jAnimation.Add(new JProperty("Timeline", value.Timeline));
      jAnimation.Add(new JProperty("StartOffset", value.StartOffset));
      jAnimation.Add(new JProperty("MaxRepeatCount", value.MaxRepeatCount));

      if (value.Keys == null)
      {
        jAnimation.Add(new JProperty("Keys", new JObject()));
      }
      else
      {
        jAnimation.Add(new JProperty("Keys", WriteKeyframes(value.Keys)));
      }
      
      return jAnimation;
    }

    private static JObject WriteKeyframes(Keyframes value)
    {
      JObject jKeyframes = new JObject();

      // Visibility.
      JArray jVisibility = new JArray();
      foreach (var kf in value.Visibility)
      {
        JObject jK = new JObject();
        jK.Add(new JProperty("Time", kf.Time));
        jK.Add(new JProperty("Value", kf.Value));
        jK.Add(new JProperty("Interpolation", kf.Interpolation.ToString()));
        jVisibility.Add(jK);
      }
      jKeyframes.Add(new JProperty("Visibility", jVisibility));

      // Offset.
      JArray jOffset = new JArray();
      foreach (var kf in value.Offset)
      {
        JObject jK = new JObject();
        jK.Add(new JProperty("Time", kf.Time));
        jK.Add(new JProperty("Value", kf.Value));
        jK.Add(new JProperty("Interpolation", kf.Interpolation.ToString()));
        jOffset.Add(jK);
      }
      jKeyframes.Add(new JProperty("Offset", jOffset));

      // Opacity.
      JArray jOpacity = new JArray();
      foreach (var kf in value.Opacity)
      {
        JObject jK = new JObject();
        jK.Add(new JProperty("Time", kf.Time));
        jK.Add(new JProperty("Value", kf.Value));
        jK.Add(new JProperty("Interpolation", kf.Interpolation.ToString()));
        jOpacity.Add(jK);
      }
      jKeyframes.Add(new JProperty("Opacity", jOpacity));

      // Transform.
      JArray jTransform = new JArray();
      foreach (var kf in value.Transform)
      {
        JObject jK = new JObject();
        jK.Add(new JProperty("Time", kf.Time));
        jK.Add(new JProperty("Value", WriteTransform(kf.Value)));
        jK.Add(new JProperty("Interpolation", kf.Interpolation.ToString()));
        jTransform.Add(jK);
      }
      jKeyframes.Add(new JProperty("Transform", jTransform));

      return jKeyframes;
    }

    private static JObject WriteDrawing(Drawing drawing)
    {
      JObject jDrawing = new JObject();

      jDrawing.Add(new JProperty("BoundingBox", WriteBoundingBox(drawing.BoundingBox)));
      jDrawing.Add(new JProperty("DataFileOffset", drawing.DataFileOffset.ToString("X")));

      return jDrawing;
    }

    private static void WriteLastStrokeId(Sequence seq, QBinWriter qbinWriter)
    {
      // 8-byte header.
      // This value is sometimes seemingly broken in quill files.
      qbinWriter.Write(seq.LastStrokeId);
      int padding = 0;
      qbinWriter.Write(padding);
    }

    /// <summary>
    /// Recursive function to write the paint data to file and update the layers offsets.
    /// This is called with the root layer and will write all the data for the sequence.
    /// </summary>
    private static void WriteDrawingData(Layer layer, QBinWriter qbinWriter)
    {
      if (layer.Type == LayerType.Group)
      {
        foreach (Layer l in ((LayerImplementationGroup)layer.Implementation).Children)
        {
          if (l != null)
            WriteDrawingData(l, qbinWriter);
        }
      }
      else if (layer.Type == LayerType.Paint)
      {
        LayerImplementationPaint lip = layer.Implementation as LayerImplementationPaint;

        foreach (Drawing drawing in lip.Drawings)
        {
          drawing.DataFileOffset = qbinWriter.BaseStream.Position;
          qbinWriter.Write(drawing.Data);
        }
      }
    }
    
    private static void WriteState(string path)
    {
      // We need to write the scene state to be able to read the file in Quill.
      // Use a dummy structure with all default values.
      // Unlike quill default new document, we explicitely not start with any paint layer in move or paint mode.
      
      JObject root = new JObject();
      
      JObject jQuill = new JObject();

      JObject jRulers = new JObject();
      jRulers.Add(new JProperty("ShowGrid", false));
      jQuill.Add(new JProperty("Rulers", jRulers));

      JObject jSurface = new JObject();
      jSurface.Add(new JProperty("Texture", "None"));
      jSurface.Add(new JProperty("Scale", 1.0f));

      JObject jDetailRender = new JObject();
      jDetailRender.Add(new JProperty("Surface", jSurface));
      jQuill.Add(new JProperty("DetailRender", jDetailRender));

      jQuill.Add(new JProperty("MoveLayer", ""));
      jQuill.Add(new JProperty("PaintLayer", ""));
      jQuill.Add(new JProperty("CameraLayer", ""));
      jQuill.Add(new JProperty("DirectManipulation", 0));

      jQuill.Add(new JProperty("ToolName", "Paint"));

      JObject jTool = new JObject();
      jTool.Add(new JProperty("BrushID", 3));
      jTool.Add(new JProperty("Color", new List<float>() { 0, 0, 0 }));
      jTool.Add(new JProperty("Opacity", 1.0f));
      jTool.Add(new JProperty("Size", 0.01f));
      jTool.Add(new JProperty("TransparentTaper", "None"));
      jTool.Add(new JProperty("WidthTaper", "Pressure"));
      jTool.Add(new JProperty("DirectionalStroke", false));
      jQuill.Add(new JProperty("Tool", jTool));

      JArray jColorPalette = new JArray();
      for (int i = 0; i < 16; i++)
      {
        float luma = 0.82f;
        jColorPalette.Add(luma);
        jColorPalette.Add(luma);
        jColorPalette.Add(luma);
      }
      jQuill.Add(new JProperty("ColorPalette", jColorPalette));

      root.Add(new JProperty("Quill", jQuill));
      
      string json = JsonConvert.SerializeObject(root, Formatting.Indented);
      File.WriteAllText(path, json);
    }
  }
}
