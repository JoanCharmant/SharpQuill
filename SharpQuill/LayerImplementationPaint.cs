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
    /// <summary>
    /// The framerate for animated layers.
    /// </summary>
    public int Framerate { get; set; }

    /// <summary>
    /// Number of loops of animation. 0 means inifinite loops.
    /// </summary>
    public int MaxRepeatCount { get; set; }

    /// <summary>
    /// The list of drawings (keyframes) in this layer.
    /// Empty keyframes are still drawings.
    /// Non-keyframes are not drawings.
    /// Inserted keyframes are appended at the end of the list.
    /// </summary>
    public List<Drawing> Drawings { get; set; } = new List<Drawing>();

    /// <summary>
    /// The list of frames in the layer, values are indices into the Drawings list.
    /// A non-keyframe means an index is repeated.
    /// The values are not necessarily in order if we have inserted a keyframe in the middle.
    /// </summary>
    public List<int> Frames { get; set; } = new List<int>();

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
      foreach (int frame in Frames)
        l.Frames.Add(frame);

      return l;
    }
  }
}
