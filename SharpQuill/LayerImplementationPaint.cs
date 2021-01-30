using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a paint layer.
  /// A paint layer contains drawings and keyframes.
  /// </summary>
  public class LayerImplementationPaint : LayerImplementation
  {
    public float Framerate { get; set; }
    public int MaxRepeatCount { get; set; }
    public List<Drawing> Drawings { get; set; } = new List<Drawing>();
    public List<float> Frames { get; set; } = new List<float>();

    /// <summary>
    /// Performs a deep copy of this LayerImplementationPaint.
    /// </summary>
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
