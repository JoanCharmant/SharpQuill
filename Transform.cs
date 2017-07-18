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
    public List<float> data;

    public Transform()
    {
      // Initialize with identity.
      data = new List<float>() { 
        1, 0, 0, 0,
        0, 1, 0, 0, 
        0, 0, 1, 0, 
        0, 0, 0, 1 };
    }

    public Transform(List<float> value)
    {
      data = new List<float>(value);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < 4; i++)
      {
        for (int j = 0; j < 4; j++)
        {
          int index = i * 4 + j;
          sb.AppendFormat("{0}", data[index]);
          if (index != 15)
            sb.Append(", ");
        }
       
        if (i != 3)
          sb.Append(Environment.NewLine);
      }

      return sb.ToString();
    }
  }
}
