using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Keyframes
  {
    /// <summary>
    /// List of visibility keyframes. Not clear if this is really animable.
    /// Seemingly only used for offsetting the layer to the right.
    /// In this case a single keyframe is created for Visibility and Offset,
    /// the value is set to true at the start time and interpolation set to None,
    /// and in the offset keyframe the value is set to 0 at the start time.
    /// </summary>
    public List<Keyframe<bool>> Visibility { get; set; } = new List<Keyframe<bool>>();

    /// <summary>
    /// List of offset keyframes. Not clear if this is really animable.
    /// Seemingly only used for offsetting the layer to the right.
    /// </summary>
    public List<Keyframe<int>> Offset { get; set; } = new List<Keyframe<int>>();

    /// <summary>
    /// List of opacity keyframes.
    /// </summary>
    public List<Keyframe<float>> Opacity { get; set; } = new List<Keyframe<float>>();

    /// <summary>
    /// List of transform keyframes.
    /// </summary>
    public List<Keyframe<Transform>> Transform { get; set; } = new List<Keyframe<Transform>>();

  }
}
