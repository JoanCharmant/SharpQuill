using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Sequence
  {
    public Metadata Metadata;
    public Gallery Gallery;
    public Color BackgroundColor;
    public string DefaultViewpoint;
    public Layer RootLayer;
    public UInt32 LastStrokeId;
    
    public Sequence()
    {
    }
  }
}
