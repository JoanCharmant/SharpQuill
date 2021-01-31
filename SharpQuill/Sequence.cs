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

    public Metadata Metadata { get; set; } = new Metadata();

    public Gallery Gallery { get; set; } = new Gallery();

    public Color BackgroundColor { get; set; } = new Color(0.8f, 0.8f, 0.8f);

    public string DefaultViewpoint { get; set; }

    public int Framerate { get; set; } = 24;

    public int ExportStart { get; set; } = 0;

    public int ExportEnd { get; set; } = 126000;

    public Size CameraResolution { get; set; } = new Size(1920, 1080);

    /// <summary>
    /// Root of the hierarchy of layers.
    /// </summary>
    public Layer RootLayer { get; set; }

    public UInt32 LastStrokeId { get; set; }
  }
}
