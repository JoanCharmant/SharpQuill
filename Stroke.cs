using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Stroke
  {
    public int u1;
    public int u2;
    public BoundingBox BoundingBox;
    public short u3;
    public short u4;
    public int CountVertices; // Index of the vertex at the start of the next stroke.
    public List<Vertex> Vertices = new List<Vertex>();
  }
}
