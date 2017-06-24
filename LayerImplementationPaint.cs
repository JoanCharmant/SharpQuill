using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationPaint : LayerImplementation
  {
    public BoundingBox BoundingBox;
    public float AnimSpeed;
    public int PlaybackReduce; // Type unsure. (1).
    public long DataFileOffset;
  }
}
