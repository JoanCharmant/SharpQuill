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

    public Stroke Clone()
    {
      Stroke s = new Stroke();
      s.Id = Id;
      s.u2 = u2;
      s.BoundingBox = BoundingBox;
      s.BrushType = BrushType;
      s.DisableRotationalOpacity = DisableRotationalOpacity;
      s.u3 = u3;

      foreach (Vertex vertex in Vertices)
      {
        Vertex v = vertex.Clone();
        s.Vertices.Add(v);
      }

      return s;
    }
  }
}
