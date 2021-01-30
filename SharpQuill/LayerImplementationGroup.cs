using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationGroup : LayerImplementation
  {
    public List<Layer> Children { get; set; } = new List<Layer>();
  }
}
