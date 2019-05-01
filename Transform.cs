using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Transform
  {
    public Vector4 Rotation;
    public float Scale;
    public string Flip;
    public Vector3 Translation;

    public Transform()
    {
      // Initialize with identity.
      Rotation = new Vector4(0, 0, 0, 1);
      Scale = 1.0f;
      Flip = "N";
      Translation = new Vector3(0, 0, 0);
    }

    public Transform(Vector4 rotation, float scale, string flip, Vector3 translation)
    {
      Rotation = rotation;
      Scale = scale;
      Flip = flip;
      Translation = translation;
    }

    //public override string ToString()
    //{
    //  StringBuilder sb = new StringBuilder();
    //  for (int i = 0; i < 4; i++)
    //  {
    //    for (int j = 0; j < 4; j++)
    //    {
    //      int index = i * 4 + j;
    //      sb.AppendFormat("{0}", data[index]);
    //      if (index != 15)
    //        sb.Append(", ");
    //    }
       
    //    if (i != 3)
    //      sb.Append(Environment.NewLine);
    //  }

    //  return sb.ToString();
    //}
  }
}
