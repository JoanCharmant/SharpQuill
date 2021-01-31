using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a group layer.
  /// A group layer contains other layers.
  /// </summary>
  public class LayerGroup : Layer
  {
    public override LayerType Type { get { return LayerType.Group; } }

    public List<Layer> Children { get; set; } = new List<Layer>();
  }
}
