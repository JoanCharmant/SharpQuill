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
        Directory.CreateDirectory(path);

      string sequenceFilename = Path.Combine(path, "Quill.json");
      string paintDataFilename = Path.Combine(path, "Quill.qbin");
      string stateFilename = Path.Combine(path, "State.json");

      // Write the qbin file first to update the layers offsets.
      FileStream qbinStream = File.Create(paintDataFilename);
      QBinWriter qbinWriter = new QBinWriter(qbinStream);
      WriteLastStrokeId(seq, qbinWriter);
      WritePaintLayerData(seq.RootLayer, qbinWriter);
      qbinStream.Close();

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
      // We don't care because it will be saved back to flat arrays when saving from Quill.
      string json = JsonConvert.SerializeObject(root, Formatting.Indented);
      File.WriteAllText(path, json);
    }

    private static JObject WriteSequence(Sequence seq)
    {
      JObject jSeq = new JObject();
      jSeq.Add(new JProperty("BackgroundColor", WriteColor(seq.BackgroundColor)));
      jSeq.Add(new JProperty("HomePosition", WriteTransform(seq.HomePosition)));
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
      jLayer.Add(new JProperty("Transform", WriteTransform(layer.Transform)));
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

    private static JArray WriteTransform(Transform value)
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
    /// </summary>
    private static void WritePaintLayerData(Layer layer, QBinWriter qbinWriter)
    {
      if (layer.Type == LayerType.Group)
      {
        foreach (Layer l in ((LayerImplementationGroup)layer.Implementation).Children)
          WritePaintLayerData(l, qbinWriter);
      }
      else if (layer.Type == LayerType.Paint)
      {
        LayerImplementationPaint lip = layer.Implementation as LayerImplementationPaint;
        lip.DataFileOffset = qbinWriter.BaseStream.Position;
        qbinWriter.Write(lip.Data);
      }
    }

    private static void WriteState(string path)
    {
      // We write the state just to be able to read the file in Quill.
      // Use a dummy structure with all default values.
      // Unlike quill default new document, we explicitely not start with any paint layer in move or paint mode.
      
      JObject root = new JObject();
      
      JObject jQuill = new JObject();
      jQuill.Add(new JProperty("ShowGrid", false));
      jQuill.Add(new JProperty("MoveLayer", ""));
      jQuill.Add(new JProperty("PaintLayer", ""));
      jQuill.Add(new JProperty("DirectManipulation", false));
      jQuill.Add(new JProperty("ToolID", 0));

      JObject jTool = new JObject();
      jTool.Add(new JProperty("BrushID", 3));
      jTool.Add(new JProperty("Color", new List<float>() { 0, 0, 0 }));
      jTool.Add(new JProperty("Opacity", 1.0f));
      jTool.Add(new JProperty("Size", 0.01f));
      jTool.Add(new JProperty("TransparentTaper", true));
      jTool.Add(new JProperty("WidthTaper", true));
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
