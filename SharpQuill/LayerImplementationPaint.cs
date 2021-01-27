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

    public LayerImplementationPaint Clone()
    {
      LayerImplementationPaint l = new LayerImplementationPaint();
      l.Framerate = Framerate;
      l.MaxRepeatCount = MaxRepeatCount;
      foreach (Drawing d in Drawings)
        l.Drawings.Add(d.Clone());
      foreach (float frame in Frames)
        l.Frames.Add(frame);

      return l;
    }
  }
}
