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
      public Matrix4f HomePosition;
      public int TrackingOrigin; // Unsure of type.
      public Layer RootLayer;
      public UInt32 LastStrokeId;

      public Sequence()
      {
        // TODO: reasonable default values.
      }
    }
}
