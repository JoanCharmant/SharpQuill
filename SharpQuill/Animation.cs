using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Animation
  {
    public float Duration { get; set; }
    public bool Timeline { get; set; }
    public float StartOffset { get; set; }
    public float MaxRepeatCount { get; set; }
    public Keyframes Keys { get; set; } = new Keyframes();
  }
}
