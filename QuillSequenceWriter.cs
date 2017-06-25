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
  public static class QuillSequenceWriter
  {
    public static void Write(Sequence seq, string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new InvalidOperationException();

      if (!Directory.Exists(path))
        throw new InvalidOperationException();

      string sequenceFilename = Path.Combine(path, "Quill.json");
      string paintDataFilename = Path.Combine(path, "Quill.qbin");

      WriteManifest(seq, sequenceFilename);
      

    }

    private static void WriteManifest(Sequence seq, string path)
    {
      // Ref: http://www.newtonsoft.com/json/help/html/CreatingLINQtoJSON.htm
      JObject root = new JObject();
      root.Add(new JProperty("Version", 1));
      root.Add(new JProperty("Sequence", WriteSequence(seq)));
      
      // Note: the formatter will fully indent arrays of primitives.
      // We don't care because it will be saved back to flat arrays when saving from Quill.
      string json = JsonConvert.SerializeObject(root, Formatting.Indented);
      File.WriteAllText(path, json);
    }

    private static JObject WriteSequence(Sequence seq)
    {
      JObject jSeq = new JObject();
      jSeq.Add(new JProperty("BackgroundColor", WriteColor(seq.BackgroundColor)));
      jSeq.Add(new JProperty("HomePosition", WriteMatrix4(seq.HomePosition)));
      jSeq.Add(new JProperty("TrackingOrigin", seq.TrackingOrigin));
      jSeq.Add(new JProperty("RootLayer", WriteLayer(seq.RootLayer)));
      return jSeq;
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
      jLayer.Add(new JProperty("Transform", WriteMatrix4(layer.Transform)));
      jLayer.Add(new JProperty("AnimOffset", layer.AnimOffset));
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
          return WriteLayerImplementationPicture(impl as LayerImplementationPicture);
        case LayerType.Sound:
          return WriteLayerImplementationSound(impl as LayerImplementationSound);
        default:
          return null;
      }
    }

    private static JObject WriteLayerImplementationGroup(LayerImplementationGroup impl)
    {
      JObject jLayer = new JObject();
      JArray jChildren = new JArray();

      foreach (Layer child in impl.Children)
        jChildren.Add(WriteLayer(child));

      jLayer.Add(new JProperty("Children", jChildren));

      return jLayer;
    }

    private static JObject WriteLayerImplementationPaint(LayerImplementationPaint impl)
    {
      JObject jLayer = new JObject();

      jLayer.Add(new JProperty("BoundingBox", WriteBoundingBox(impl.BoundingBox)));
      jLayer.Add(new JProperty("AnimSpeed", impl.AnimSpeed));
      jLayer.Add(new JProperty("PlaybackReduce", impl.PlaybackReduce));
      jLayer.Add(new JProperty("DataFileOffset", impl.DataFileOffset.ToString("X")));

      return jLayer;
    }

    private static JObject WriteLayerImplementationPicture(LayerImplementationPicture impl)
    {
      JObject jLayer = new JObject();

      jLayer.Add(new JProperty("Mode", impl.Mode.ToString()));
      jLayer.Add(new JProperty("DataFile", impl.Filename));

      return jLayer;
    }
    
    private static JObject WriteLayerImplementationSound(LayerImplementationSound impl)
    {
      JObject jLayer = new JObject();

      jLayer.Add(new JProperty("Duration", impl.Duration));
      jLayer.Add(new JProperty("Loop", impl.Loop));
      jLayer.Add(new JProperty("Play", impl.Play));
      jLayer.Add(new JProperty("File", impl.Filename));

      return jLayer;
    }

    private static JArray WriteColor(Color value)
    {
      return new JArray(value.R, value.G, value.B);
    }

    private static JArray WriteMatrix4(Matrix4f value)
    {
      return new JArray(value.data);

      /*JArray output = new JArray();
      foreach (float entry in value.data)
        output.Add(entry);

      return output;*/
    }

    private static JArray WriteBoundingBox(BoundingBox value)
    {
      return new JArray(value.MinX, value.MaxX, value.MinY, value.MaxY, value.MinZ, value.MaxZ);
    }

  }
}
