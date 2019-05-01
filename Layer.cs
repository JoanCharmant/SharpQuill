using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Layer
  {
    public string Name;
    public bool Visible;
    public bool Locked;
    public bool Collapsed;
    public bool BBoxVisible;
    public float Opacity;
    public LayerType Type;
    public bool IsModelTopLayer;
    public KeepAlive KeepAlive;
    public Transform Transform;
    public Transform Pivot;
    public Animation Animation;
    public LayerImplementation Implementation;
  }
}
