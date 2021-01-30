using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Brush types.
  /// The "capped" versions of the brushes are made by setting the start and end vertices width to zero.
  /// </summary>
  public enum BrushType
  {
    Cylinder, 
    Ellipse,
    Ribbon, 
    Cube,
    Line, 
  }
}
