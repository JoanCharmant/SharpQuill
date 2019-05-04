using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class Vertex
  {
    public Vector3 Position;
    public Vector3 Normal;
    public Vector3 Tangeant;
    public Color Color;
    public float Opacity;
    public float Width;

    public Vertex Clone()
    {
      Vertex v = new Vertex();
      v.Position = Position;
      v.Normal = Normal;
      v.Tangeant = Tangeant;
      v.Color = Color;
      v.Opacity = Opacity;
      v.Width = Width;
      return v;
    }
  }
}
