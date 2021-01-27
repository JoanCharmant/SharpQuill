using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Drawing
  {
    public BoundingBox BoundingBox;
    public long DataFileOffset;
    public DrawingData Data = new DrawingData();

    public Drawing Clone()
    {
      Drawing d = new Drawing();
      d.BoundingBox = BoundingBox;
      d.DataFileOffset = DataFileOffset; // This should be updated later.
      d.Data = Data.Clone();
      return d;
    }
  }
}
