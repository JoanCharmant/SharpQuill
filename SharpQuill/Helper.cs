using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Helper methods to create, search or manipulate the data object model.
  /// </summary>
  public static class Helper
  {
    /// <summary>
    /// Adds a new group layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// For example, to create a layer at Root/Main/Test, call this with path="/Main/Test".
    /// </summary>
    public static LayerGroup CreateGroupLayerAt(Sequence seq, string path, bool isSequence = false)
    {
      LayerGroup parent = seq.RootLayer;
      if (parent == null)
        return null;

      string[] nodes = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < nodes.Length; i++)
      {
        Layer child = FindChild(parent, nodes[i]);

        if (child == null || child.Type != LayerType.Group)
        {
          child = new LayerGroup(nodes[i], isSequence);
          parent.Children.Add(child);
        }

        parent = child as LayerGroup;
      }

      return parent;
    }

    /// <summary>
    /// Adds a new paint layer at the specified path, creates all the groups along the way if necessary.
    /// The path should not contain the "Root" group, use a single "/" instead or nothing.
    /// For example, to create a layer at Root/Main/Test, call this with path="/Main/Test".
    /// </summary>
    public static LayerPaint CreatePaintLayerAt(Sequence seq, string path)
    {
      string groupPath = Path.GetDirectoryName(path);
      LayerGroup layerGroup = CreateGroupLayerAt(seq, groupPath, false);
      if (layerGroup == null)
        return null;

      // At this point we have found or created the insertion point.
      // Double check if the paint layer itself already exist.
      string name = Path.GetFileName(path);
      LayerPaint paint = null;
      foreach (Layer child in layerGroup.Children)
      {
        if (child.Type != LayerType.Paint || child.Name != name)
          continue;

        paint = child as LayerPaint;
        break;
      }

      if (paint == null)
        layerGroup.Children.Add(new LayerPaint(name));

      return paint;
    }

    /// <summary>
    /// Adds an existing layer to a group.
    /// If the group doesn't exist at the specified path it will be created.
    /// </summary>
    /// <param name="seq">The sequence containing the parent group layer.</param>
    /// <param name="layer">The layer to add to the group layer.</param>
    /// <param name="path">The path to the group layer.</param>
    public static void InsertLayerAt(Sequence seq, Layer layer, string path)
    {
      LayerGroup parent = CreateGroupLayerAt(seq, path, false);
      if (parent != null)
        parent.Children.Add(layer);
    }
    
    /// <summary>
    /// Finds a layer at the specified path. Does not create the groups along the way if not found.
    /// </summary>
    public static Layer FindLayer(LayerGroup parent, string path)
    {
      string[] nodes = path.Split(new string[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries);

      Layer layer = parent;
      for (int i = 0; i < nodes.Length; i++)
      {
        if (parent == null)
          return null;

        Layer child = FindChild(parent, nodes[i]);
        if (child == null)
          return null;

        if (i == nodes.Length - 1)
          return child;
        else
          parent = child as LayerGroup;
      }

      return layer;
    }

    /// <summary>
    /// Finds an immediate child layer matching the name.
    /// </summary>
    public static Layer FindChild(LayerGroup parent, string name)
    {
      Layer layer = null;
      foreach (Layer child in parent.Children)
      {
        if (child.Name != name)
          continue;

        layer = child;
        break;
      }

      return layer;
    }
  }
}
