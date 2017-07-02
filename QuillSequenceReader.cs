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
  /// Reads a quill project: JSON manifest and binary paint data.
  /// </summary>
  public static class QuillSequenceReader
  {
    public static Sequence Read(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new InvalidOperationException();

      if (!Directory.Exists(path))
        throw new InvalidOperationException();

      string sequenceFilename = Path.Combine(path, "Quill.json");
      if (!File.Exists(sequenceFilename))
        throw new InvalidOperationException();

      string paintDataFilename = Path.Combine(path, "Quill.qbin");
      if (!File.Exists(paintDataFilename))
        throw new InvalidOperationException();

      string json = File.ReadAllText(sequenceFilename);
      dynamic document = JsonConvert.DeserializeObject(json);
      Sequence seq = Parse(document);

      // Read all the paint data.
      Stream stream = File.OpenRead(paintDataFilename);
      QBinReader qbinReader = new QBinReader(stream);
      seq.LastStrokeId = qbinReader.ReadUInt32();

      // TODO: keep a global list of layer data.
      // If two layers point to the same offset only load it once in memory.
      ReadPaintLayerData(seq.RootLayer, qbinReader);
      qbinReader.Close();

      return seq;
    }

    private static Sequence Parse(dynamic s)
    {
      Sequence seq = new Sequence();
      seq.BackgroundColor = ParseColor(s.Sequence.BackgroundColor);
      seq.HomePosition = ParseTransform(s.Sequence.HomePosition);
      seq.TrackingOrigin = s.Sequence.TrackingOrigin;
      seq.RootLayer = ParseLayer(s.Sequence.RootLayer);
      return seq;
    }

    private static Color ParseColor(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 3)
        throw new InvalidDataException();

      return new Color(value);
    }

    private static Transform ParseTransform(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 16)
        throw new InvalidDataException();

      return new Transform(value);
    }

    private static BoundingBox ParseBoundingBox(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 6)
        throw new InvalidDataException();

      return new BoundingBox(value);
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

      LayerType layerType;
      bool parsed = Enum.TryParse((string)l.Type.ToObject(typeof(string)), out layerType);
      layer.Type = parsed ? layerType : LayerType.Unknown;
      
      layer.Transform = ParseTransform(l.Transform);
      layer.AnimOffset = l.AnimOffset;
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
            impl.BoundingBox = ParseBoundingBox(li.BoundingBox);
            impl.AnimSpeed = li.AnimSpeed;
            impl.PlaybackReduce = li.PlaybackReduce;

            long offset;
            bool parsed = long.TryParse((string)li.DataFileOffset.ToObject(typeof(string)), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out offset);
            impl.DataFileOffset = parsed ? offset : -1;
            
            result = impl;
            break;
        }
        case LayerType.Picture:
          {
            LayerImplementationPicture impl = new LayerImplementationPicture();
            
            PictureMode mode;
            bool parsed = Enum.TryParse((string)li.Mode.ToObject(typeof(string)), out mode);
            impl.Mode = parsed ? mode : PictureMode.Unknown;

            impl.Filename = li.Filename;
            result = impl;
            break;
          }
        case LayerType.Sound:
          {
            LayerImplementationSound impl = new LayerImplementationSound();
            impl.Duration = li.Duration;
            impl.Loop = li.Loop;
            impl.Play = li.Play;
            impl.Filename = li.Filename;

            result = impl;
            break;
          }
      }

      return result;
    }

    private static void ReadPaintLayerData(Layer layer, QBinReader qbinReader)
    {
      // Recursive function reading paint data for the entire hierarchy.
      if (layer.Type == LayerType.Group)
      {
        foreach (Layer l in ((LayerImplementationGroup)layer.Implementation).Children)
          ReadPaintLayerData(l, qbinReader);
      }
      else if (layer.Type == LayerType.Paint)
      {
        LayerImplementationPaint lip = layer.Implementation as LayerImplementationPaint;
        qbinReader.BaseStream.Seek(lip.DataFileOffset, SeekOrigin.Begin);
        lip.Data = qbinReader.ReadPaintLayer();
      }
    }
  }
}
