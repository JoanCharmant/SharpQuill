using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public struct Vector3
  {
    public float X;
    public float Y;
    public float Z;

    public Vector3(List<float> value)
    {
      X = value[0];
      Y = value[1];
      Z = value[2];
    }

    public Vector3(float x, float y, float z)
    {
      X = x;
      Y = y;
      Z = z;
    }
  }
}
