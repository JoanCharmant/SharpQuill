using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
    public struct Sequence
    {
      public Color BackgroundColor;
      public Matrix4f HomePosition;
      public int TrackingOrigin; // Unsure of type.
      public Layer RootLayer;
    }
}
