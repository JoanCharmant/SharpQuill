using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public struct BoundingBox
  {
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;
    public float MinZ;
    public float MaxZ;

    public BoundingBox(List<float> value)
    {
      MinX = value[0];
      MaxX = value[1];
      MinY = value[2];
      MaxY = value[3];
      MinZ = value[4];
      MaxZ = value[5];
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}, {4}, {5}", 
        MinX, MaxX, MinY, MaxY, MinZ, MaxZ);
    }
  }
}
