using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Represents the top level object in the scene.
  /// </summary>
  public class Sequence
  {
    public Metadata Metadata { get; set; }
    public Gallery Gallery { get; set; }
    public Color BackgroundColor { get; set; }
    public string DefaultViewpoint { get; set; }
    public Layer RootLayer { get; set; }
    public UInt32 LastStrokeId { get; set; }
  }
}
