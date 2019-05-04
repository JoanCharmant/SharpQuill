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

    public void Reset()
    {
      MinX = MinY = MinZ = 1.0e+20f;
      MaxX = MaxY = MaxZ = -1.0e+20f; 
    }

    public Vector3 Center()
    {
      return new Vector3(MinX + (MaxX - MinX)/2, MinY + (MaxY - MinY)/2, MinZ + (MaxZ - MinZ)/2);
    }

    public static BoundingBox Extend(BoundingBox a, BoundingBox b)
    {
      a.MinX = Math.Min(a.MinX, b.MinX);
      a.MinY = Math.Min(a.MinY, b.MinY);
      a.MinZ = Math.Min(a.MinZ, b.MinZ);
      a.MaxX = Math.Max(a.MaxX, b.MaxX);
      a.MaxY = Math.Max(a.MaxY, b.MaxY);
      a.MaxZ = Math.Max(a.MaxZ, b.MaxZ);
      return a;
    }

    public static BoundingBox Extend(BoundingBox a, Vector3 v)
    {
      a.MinX = Math.Min(a.MinX, v.X);
      a.MinY = Math.Min(a.MinY, v.Y);
      a.MinZ = Math.Min(a.MinZ, v.Z);
      a.MaxX = Math.Max(a.MaxX, v.X);
      a.MaxY = Math.Max(a.MaxY, v.Y);
      a.MaxZ = Math.Max(a.MaxZ, v.Z);
      return a;
    }
  }
}
