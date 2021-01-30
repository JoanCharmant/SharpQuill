using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// A generic layer.
  /// The data is contained in type specific implementation.
  /// </summary>
  public class Layer
  {
    /// <summary>
    /// The name of the layer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Whether the layer is visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Whether the layer is locked.
    /// Locked layers cannot be modfied or expanded.
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    /// Whether the layer is collapsed.
    /// </summary>
    public bool Collapsed { get; set; }

    /// <summary>
    /// Whether the bounding box of the layer is visible.
    /// </summary>
    public bool BBoxVisible { get; set; }

    /// <summary>
    /// Layer-specific opacity level.
    /// </summary>
    public float Opacity { get; set; }

    /// <summary>
    /// The type of the layer.
    /// </summary>
    public LayerType Type { get; set; }

    /// <summary>
    /// Whether this layer is the root of a model hierarchy.
    /// </summary>
    public bool IsModelTopLayer { get; set; }

    public KeepAlive KeepAlive { get; set; }

    /// <summary>
    /// The local coordinate system in relation to the parent layer.
    /// </summary>
    public Transform Transform { get; set; } = Transform.Identity;

    /// <summary>
    /// The transform of the pivot for this layer, in layer space.
    /// </summary>
    public Transform Pivot { get; set; } = Transform.Identity;

    /// <summary>
    /// The animation channels for this layer (for interpolated animation).
    /// </summary>
    public Animation Animation { get; set; }

    /// <summary>
    /// The type-specific data for this layer.
    /// </summary>
    public LayerImplementation Implementation { get; set; }
  }
}
