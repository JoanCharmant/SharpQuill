using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Sequence
  {
    public Color BackgroundColor;
    public Transform HomePosition;
    public int TrackingOrigin; // Unsure of type.
    public bool AnimateOnStart;
    public Layer RootLayer;
    public UInt32 LastStrokeId;

    public Sequence()
    {
    }
  }
}
