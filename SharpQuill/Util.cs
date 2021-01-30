using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Helper methods to create or manipulate the data object model.
  /// </summary>
  public static class Util
  {

    /// <summary>
    /// Creates a simple Quill scene.
    /// </summary>
    public static Sequence CreateDefaultSequence()
    {
      Sequence seq = new Sequence();
      seq.Metadata = new Metadata();
      seq.Gallery = new Gallery();
      seq.BackgroundColor = new Color(0.8f, 0.8f, 0.8f);
      seq.DefaultViewpoint = "Root/InitialSpawnArea";
      seq.Framerate = 24;
      seq.ExportStart = 0;
      seq.ExportEnd = 126000;
      seq.CameraResolution = new Size(1920, 1080);

      Layer root = CreateDefaultGroup("Root", true);
      Layer spawn = CreateDefaultSpawnArea("InitialSpawnArea");
      List<Layer> children = ((LayerImplementationGroup)root.Implementation).Children;
      children.Add(spawn);

      seq.RootLayer = root;
      seq.LastStrokeId = 0;
      return seq;
    }

    /// <summary>
    /// Creates a simple group layer.
    /// </summary>
    public static Layer CreateDefaultGroup(string name, bool isSequence = false)
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
      layer.Transform = Transform.Identity;
      layer.Pivot = Transform.Identity;
      layer.Animation = new Animation();
      layer.Animation.Keys = new Keyframes();

      if (isSequence)
      {
        layer.Animation.Duration = 45360000;
        layer.Animation.Timeline = true;
        layer.Animation.Keys.Visibility.Add(new Keyframe<bool>(0, true, Interpolation.None));
        layer.Animation.Keys.Offset.Add(new Keyframe<int>(0, 0, Interpolation.None));
      }

      LayerImplementationGroup impl = new LayerImplementationGroup();
      impl.Children = new List<Layer>();
      layer.Implementation = impl;

      return layer;
    }

    /// <summary>
    /// Creates a simple empty paint layer.
    /// </summary>
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
      layer.Transform = Transform.Identity;
      layer.Pivot = Transform.Identity;
      layer.Animation = new Animation();
      layer.Animation.Keys = new Keyframes();
      
      LayerImplementationPaint impl = new LayerImplementationPaint();
      impl.Framerate = 24;
      impl.MaxRepeatCount = 0;
      impl.Drawings = new List<Drawing>();
      impl.Frames = new List<int>();

      Drawing drawing = new Drawing();
      drawing.Data = new DrawingData();
      drawing.UpdateBoundingBox(true);
      impl.Drawings.Add(drawing);
      impl.Frames.Add(0);

      layer.Implementation = impl;
      
      return layer;
    }
    
    public static Layer CreateDefaultSpawnArea(string name = "InitialSpawnArea")
    {
      Layer layer = new Layer();
      layer.Name = name;
      layer.Visible = false;
      layer.Locked = false;
      layer.Collapsed = false;
      layer.BBoxVisible = false;
      layer.Opacity = 1.0f;
      layer.Type = LayerType.Viewpoint;
      layer.IsModelTopLayer = false;
      layer.KeepAlive = new KeepAlive();
      layer.KeepAlive.Type = KeepAliveType.None;
      layer.Transform = Transform.Identity;
      layer.Pivot = Transform.Identity;
      layer.Animation = new Animation();
      layer.Animation.Keys = new Keyframes();

      LayerImplementationViewpoint impl = new LayerImplementationViewpoint();

      impl.Version = 1;
      impl.Color = new Color(0.113542f, 0.409455f, 0.808914f);
      impl.Sphere = new Vector4(0, 1, 0, 2);
      impl.AllowTranslationX = true;
      impl.AllowTranslationY = true;
      impl.AllowTranslationZ = true;
      impl.Exporting = true;
      impl.ShowingVolume = false;
      impl.TypeStr = "FloorLevel";

      layer.Implementation = impl;

      return layer;
    }

    /// <summary>
    /// Adds a new group layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// For example, to create a layer at Root/Main/Test, call this with path="/Main/Test".
    /// </summary>
    public static Layer CreateGroupLayer(Sequence seq, string path, bool isSequence = false)
    {
      Layer parent = seq.RootLayer;
      string[] nodes = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < nodes.Length; i++)
      {
        Layer child = FindChild(parent, nodes[i]);

        if (child == null || child.Type != LayerType.Group)
        {
          child = CreateDefaultGroup(nodes[i], isSequence);
          ((LayerImplementationGroup)parent.Implementation).Children.Add(child);
        }

        parent = child;
      }

      return parent;
    }

    /// <summary>
    /// Adds a new paint layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// For example, to create a layer at Root/Main/Test, call this with path="/Main/Test".
    /// </summary>
    public static Layer CreatePaintLayer(Sequence seq, string path)
    {
      Layer result = null;

      string groupPath = Path.GetDirectoryName(path);
      Layer layerGroup = CreateGroupLayer(seq, groupPath, false);

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
    /// If the group doesn't exist at the specified path it will be created.
    /// </summary>
    /// <param name="seq">The sequence containing the group.</param>
    /// <param name="path">The path of the group.</param>
    /// <param name="layer">The layer to add to the group.</param>
    public static void Add(Sequence seq, string path, Layer layer)
    {
      Layer layerGroup = CreateGroupLayer(seq, path, false);
      List<Layer> children = ((LayerImplementationGroup)layerGroup.Implementation).Children;
      children.Add(layer);
    }

    /// <summary>
    /// Finds a layer at the specified path. Does not create the groups along the way if not found.
    /// </summary>
    public static Layer FindLayer(Sequence seq, string path)
    {
      Layer parent = seq.RootLayer;
      return FindLayer(parent, path);
    }

    /// <summary>
    /// Finds a layer at the specified path. Does not create the groups along the way if not found.
    /// </summary>
    public static Layer FindLayer(Layer parent, string path)
    {
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
    /// Finds an immediate child layer matching the name.
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
