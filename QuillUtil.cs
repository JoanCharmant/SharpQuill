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
    public static Sequence CreateDefaultSequence(bool createDefaultPaintLayer)
    {
      Sequence seq = new Sequence();
      seq.BackgroundColor = new Color(0.8f, 0.8f, 0.8f);
      seq.HomePosition = new Transform();
      seq.TrackingOrigin = 1;
      seq.AnimateOnStart = false;

      Layer root = CreateDefaultGroup("Root");

      if (createDefaultPaintLayer)
        ((LayerImplementationGroup)root.Implementation).Children.Add(CreateDefaultPaint());

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
      layer.IsAnimationCycle = false;
      layer.AnimationCycleRepeat = 0;
      layer.KeepAlive = new KeepAlive();
      layer.KeepAlive.Type = KeepAliveType.None;
      layer.Animation = new Animation();
      layer.AnimOffset = 0;
      layer.Transform = new Transform();
      layer.Pivot = new Transform();
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
      layer.IsAnimationCycle = false;
      layer.AnimationCycleRepeat = 0;
      layer.KeepAlive = new KeepAlive();
      layer.KeepAlive.Type = KeepAliveType.None;
      layer.Animation = new Animation();
      layer.AnimOffset = 0;
      layer.Transform = new Transform();
      layer.Pivot = new Transform();
      LayerImplementationPaint impl = new LayerImplementationPaint();
      
      impl.Framerate = 24.0f;
      impl.MaxRepeatCount = 0;
      impl.Drawings = new List<Drawing>();
      impl.Frames = new List<float>();

      Drawing drawing = new Drawing();
      impl.Drawings.Add(drawing);
      impl.Frames.Add(0);

      drawing.Data = new DrawingData();
      UpdateBoundingBox(drawing);

      layer.Implementation = impl;
      
      return layer;
    }

    public static void UpdateBoundingBox(Stroke stroke, Vertex v)
    {
      stroke.BoundingBox.MinX = Math.Min(stroke.BoundingBox.MinX, v.Position.X);
      stroke.BoundingBox.MinY = Math.Min(stroke.BoundingBox.MinY, v.Position.Y);
      stroke.BoundingBox.MinZ = Math.Min(stroke.BoundingBox.MinZ, v.Position.Z);
      stroke.BoundingBox.MaxX = Math.Max(stroke.BoundingBox.MaxX, v.Position.X);
      stroke.BoundingBox.MaxY = Math.Max(stroke.BoundingBox.MaxY, v.Position.Y);
      stroke.BoundingBox.MaxZ = Math.Max(stroke.BoundingBox.MaxZ, v.Position.Z);
    }

    public static void UpdateBoundingBox(Stroke stroke)
    {
      foreach (Vertex v in stroke.Vertices)
        UpdateBoundingBox(stroke, v);
    }

    public static void UpdateBoundingBox(Drawing drawing)
    {
      float max = 1.0e+20f;
      float min = -1.0e+20f;
      List<float> values = new List<float>() { max, min, max, min, max, min };
      BoundingBox bbox = new BoundingBox(values);

      foreach (Stroke s in drawing.Data.Strokes)
        bbox = ConsolidateBoundingBox(bbox, s.BoundingBox);

      drawing.BoundingBox = bbox;
    }

    public static void UpdateBoundingBox(LayerImplementationPaint impl)
    {
      foreach (Drawing drawing in impl.Drawings)
        UpdateBoundingBox(drawing);
    }

    private static BoundingBox ConsolidateBoundingBox(BoundingBox a, BoundingBox b)
    {
      a.MinX = Math.Min(a.MinX, b.MinX);
      a.MinY = Math.Min(a.MinY, b.MinY);
      a.MinZ = Math.Min(a.MinZ, b.MinZ);
      a.MaxX = Math.Max(a.MaxX, b.MaxX);
      a.MaxY = Math.Max(a.MaxY, b.MaxY);
      a.MaxZ = Math.Max(a.MaxZ, b.MaxZ);
      return a;
    }


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
    /// Find a layer at the specified path. Does not create the groups along the way.
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
