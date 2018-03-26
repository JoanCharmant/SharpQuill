using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Stroke
  {
    public UInt32 Id = 0; // Not too sure of this one, there are issues with some large values in some files.
    public int u2 = 0;
    public BoundingBox BoundingBox = new BoundingBox();
    public BrushType BrushType = BrushType.Ellipse;
    public bool DisableRotationalOpacity = true;
    public byte u3; // Probably a boolean flag.
    public List<Vertex> Vertices = new List<Vertex>();
  }
}
