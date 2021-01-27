using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Animation
  {
    public float Duration;
    public bool Timeline;
    public float StartOffset;
    public float MaxRepeatCount;
    public Keyframes Keys = new Keyframes();
  }
}
