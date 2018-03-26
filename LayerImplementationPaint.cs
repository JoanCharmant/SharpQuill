using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationPaint : LayerImplementation
  {
    public float Framerate;
    public int MaxRepeatCount;
    public List<Drawing> Drawings = new List<Drawing>();
    public List<float> Frames = new List<float>();
  }
}
