using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public struct Vector4
  {
    public float X;
    public float Y;
    public float Z;
    public float W;

    public Vector4(List<float> value)
    {
      X = value[0];
      Y = value[1];
      Z = value[2];
      W = value[3];
    }

    public Vector4(float x, float y, float z, float w)
    {
      X = x;
      Y = y;
      Z = z;
      W = w;
    }
  }
}
