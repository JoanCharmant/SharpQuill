using System.Collections.Generic;

namespace SharpQuill
{
  public struct Color
  {
    public float R;
    public float G;
    public float B;

    public Color(List<float> value)
    {
      R = value[0];
      G = value[1];
      B = value[2];
    }

    public Color(float r, float g, float b)
    {
      R = r;
      G = g;
      B = b;
    }
  }
}
