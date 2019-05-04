using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class DrawingData
  {
    public List<Stroke> Strokes = new List<Stroke>();

    public DrawingData Clone()
    {
      DrawingData d = new DrawingData();
      foreach (Stroke stroke in Strokes)
        d.Strokes.Add(stroke.Clone());

      return d;
    }
  }
}
