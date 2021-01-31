using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  // A group layer contains other layers.
  public class LayerGroup : Layer
  {
    public override LayerType Type { get { return LayerType.Group; } }

    public List<Layer> Children { get; set; } = new List<Layer>();
  }
}
