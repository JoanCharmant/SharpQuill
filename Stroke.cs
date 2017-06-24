using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Stroke
  {
    public UInt32 Id; // Not too sure of this one, there are issues with some large values in some files.
    public int u2;
    public BoundingBox BoundingBox;
    public BrushType BrushType;
    public bool DisableRotationalOpacity;
    public byte u3;
    public int CountVertices;
    public List<Vertex> Vertices = new List<Vertex>();
  }
}
