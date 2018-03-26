using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationSound : LayerImplementation
  {
    public float Duration;
    public float Volume;
    public int AttenMode;
    public float AttenMin;
    public float AttenMax;
    public bool Loop;
    public bool IsSpatialized;
    public bool Play;
    public string Filename;
  }
}
