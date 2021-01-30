using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Represents the scene hierarchy and its metadata.
  /// </summary>
  public class Sequence
  {

    public Metadata Metadata { get; set; }

    public Gallery Gallery { get; set; }

    public Color BackgroundColor { get; set; }

    public string DefaultViewpoint { get; set; }

    public int Framerate { get; set; }

    public int ExportStart { get; set; }

    public int ExportEnd { get; set; }

    public Size CameraResolution { get; set; }

    /// <summary>
    /// Root of the hierarchy of layers.
    /// </summary>
    public Layer RootLayer { get; set; }

    public UInt32 LastStrokeId { get; set; }
  }
}
