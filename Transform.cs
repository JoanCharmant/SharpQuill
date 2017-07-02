using System;
using System.Collections.Generic;
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
  }
}
