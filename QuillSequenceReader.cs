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
  public static class QuillSequenceReader
  {
    public static Sequence Read(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new InvalidOperationException();

      if (!Directory.Exists(path))
        throw new InvalidOperationException();

      string filename = Path.Combine(path, "Quill.json");
      if (!File.Exists(filename))
        throw new InvalidOperationException();
        
      string json = File.ReadAllText(filename);
      dynamic document = JsonConvert.DeserializeObject(json);

      int version = document.Version;
      return Parse(document.Sequence);
    }

    private static Sequence Parse(dynamic s)
    {
      Sequence sequence = new Sequence();
      sequence.BackgroundColor = ParseColor(s.BackgroundColor);
      sequence.HomePosition = ParseMatrix4(s.HomePosition);
      sequence.TrackingOrigin = s.TrackingOrigin;
      sequence.RootLayer = ParseLayer(s.RootLayer);
      return sequence;
    }

    private static Color ParseColor(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 3)
        throw new InvalidDataException();

      return new Color(value);
    }

    private static Matrix4f ParseMatrix4(JArray jValue)
    {
      List<float> value = jValue.ToObject<List<float>>();

      if (value.Count != 16)
        throw new InvalidDataException();

      return new Matrix4f(value);
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
      
      //layer.Type = (LayerType)Enum.Parse(typeof(LayerType), (string)l.Type.ToObject(typeof(string)));
      layer.Transform = ParseMatrix4(l.Transform);
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
  }
}
