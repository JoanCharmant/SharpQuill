using System;
using System.Collections.Generic;
using System.Text;

namespace SharpQuill
{
  public class Keyframe<T>
  {
    /// <summary>
    /// Time of the keyframe in milliseconds.
    /// </summary>
    public int Time { get; set; }

    /// <summary>
    /// Value at that keyframe.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Interpolation mode at that keyframe.
    /// </summary>
    public Interpolation Interpolation { get; set; }
  }
}
