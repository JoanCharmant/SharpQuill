using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public static class QuillUtil
  {
    public static Sequence CreateDefaultSequence()
    {
      Sequence seq = new Sequence();
      seq.Metadata = new Metadata();
      seq.Gallery = new Gallery();
      seq.BackgroundColor = new Color(0.8f, 0.8f, 0.8f);
      seq.DefaultViewpoint = "Root/InitialSpawnArea";
      Layer root = CreateDefaultGroup("Root");
      
      seq.RootLayer = root;
      seq.LastStrokeId = 0;
      return seq;
    }

    public static Layer CreateDefaultGroup(string name)
    {
      Layer layer = new Layer();
      layer.Name = name;
      layer.Visible = true;
      layer.Locked = false;
      layer.Collapsed = false;
      layer.BBoxVisible = false;
      layer.Opacity = 1.0f;
      layer.Type = LayerType.Group;
      layer.IsModelTopLayer = false;
      layer.KeepAlive = new KeepAlive();
      layer.KeepAlive.Type = KeepAliveType.None;
      layer.Transform = new Transform();
      layer.Pivot = new Transform();
      layer.Animation = new Animation();
      layer.Animation.Keys = new Keyframes();

      LayerImplementationGroup impl = new LayerImplementationGroup();
      impl.Children = new List<Layer>();
      layer.Implementation = impl;

      return layer;
    }
    
    public static Layer CreateDefaultPaint(string name = "Paint")
    {
      Layer layer = new Layer();
      layer.Name = name;
      layer.Visible = true;
      layer.Locked = false;
      layer.Collapsed = false;
      layer.BBoxVisible = false;
      layer.Opacity = 1.0f;
      layer.Type = LayerType.Paint;
      layer.IsModelTopLayer = false;
      layer.KeepAlive = new KeepAlive();
      layer.KeepAlive.Type = KeepAliveType.None;
      layer.Transform = new Transform();
      layer.Pivot = new Transform();
      layer.Animation = new Animation();
      layer.Animation.Keys = new Keyframes();
      
      LayerImplementationPaint impl = new LayerImplementationPaint();
      
      impl.Framerate = 24.0f;
      impl.MaxRepeatCount = 0;
      impl.Drawings = new List<Drawing>();
      impl.Frames = new List<float>();

      Drawing drawing = new Drawing();
      impl.Drawings.Add(drawing);
      impl.Frames.Add(0);

      drawing.Data = new DrawingData();
      ComputeBoundingBox(drawing);

      layer.Implementation = impl;
      
      return layer;
    }
    
    public static void ComputeBoundingBox(Stroke stroke)
    {
      BoundingBox bbox = new BoundingBox();
      bbox.Reset();
      foreach (Vertex v in stroke.Vertices)
        bbox = BoundingBox.Extend(bbox, v.Position);

      stroke.BoundingBox = bbox;
    }

    /// <summary>
    /// Set the bounding box of the drawing. Assumes the bboxes of all individual strokes have already been computed.
    /// </summary>
    public static void ComputeBoundingBox(Drawing drawing)
    {
      BoundingBox bbox = new BoundingBox();
      bbox.Reset();
      foreach (Stroke s in drawing.Data.Strokes)
        bbox = BoundingBox.Extend(bbox, s.BoundingBox);

      drawing.BoundingBox = bbox;
    }

    /// <summary>
    /// Set the bounding box of the layer. Assumes the bboxes of all individual drawings have already been computed.
    /// </summary>
    //public static void ComputeBoundingBox(LayerImplementationPaint impl)
    //{
    //  BoundingBox bbox = new BoundingBox();
    //  bbox.Reset();
    //  foreach (Drawing drawing in impl.Drawings)
    //    ConsolidateBoundingBox(bbox, drawing.BoundingBox);
    //  impl.
    //}
    


    /// <summary>
    /// Adds a new group layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// That is, to create a layer at Root/Main/Test, pass "/Main/Test".
    /// </summary>
    public static Layer CreateGroupLayer(Sequence seq, string path)
    {
      Layer parent = seq.RootLayer;
      string[] nodes = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < nodes.Length; i++)
      {
        Layer child = FindChild(parent, nodes[i]);

        if (child == null || child.Type != LayerType.Group)
        {
          child = CreateDefaultGroup(nodes[i]);
          ((LayerImplementationGroup)parent.Implementation).Children.Add(child);
        }

        parent = child;
      }

      return parent;
    }

    /// <summary>
    /// Adds a new paint layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// That is, to create a layer at Root/Main/Test, pass "/Main/Test".
    /// </summary>
    public static Layer CreatePaintLayer(Sequence seq, string path)
    {
      Layer result = null;

      string groupPath = Path.GetDirectoryName(path);
      Layer layerGroup = CreateGroupLayer(seq, groupPath);

      // At this point we have found or created the insertion point.
      // Double check that the paint layer itself doesn't already exist.
      string name = Path.GetFileName(path);
      List<Layer> children = ((LayerImplementationGroup)layerGroup.Implementation).Children;
      foreach (Layer child in children)
      {
        if (child.Type != LayerType.Paint || child.Name != name)
          continue;

        result = child;
        break;
      }

      if (result == null)
      {
        result = CreateDefaultPaint(name);
        children.Add(result);
      }

      return result;
    }

    /// <summary>
    /// Adds a layer to a group.
    /// </summary>
    public static void Add(Sequence seq, string path, Layer layer)
    {
      Layer layerGroup = CreateGroupLayer(seq, path);

      List<Layer> children = ((LayerImplementationGroup)layerGroup.Implementation).Children;
      children.Add(layer);
    }

    /// <summary>
    /// Finds a layer at the specified path. Does not create the groups along the way.
    /// </summary>
    public static Layer FindLayer(Sequence seq, string path)
    {
      Layer parent = seq.RootLayer;
      string[] nodes = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < nodes.Length; i++)
      {
        Layer child = FindChild(parent, nodes[i]);

        if (child == null)
          return null;

        if (i < nodes.Length - 1 && child.Type != LayerType.Group)
          return null;

        parent = child;
      }

      return parent;
    }

    /// <summary>
    /// Look for an immediate child matching the name.
    /// </summary>
    public static Layer FindChild(Layer parent, string name)
    {
      if (parent.Type != LayerType.Group)
        throw new InvalidProgramException();

      Layer result = null;
      List<Layer> children = ((LayerImplementationGroup)parent.Implementation).Children;
      foreach (Layer child in children)
      {
        if (child.Name != name)
          continue;

        result = child;
        break;
      }

      return result;
    }
  }
}
