using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public struct Matrix4f
  {
    // TODO: proper matrix class, or just simple data.
    public List<float> data;

    public Matrix4f(List<float> value)
    {
      data = new List<float>(value);
    }
  }
}
