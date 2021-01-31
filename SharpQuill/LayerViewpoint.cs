using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a viewpoint layer.
  /// </summary>
  public class LayerViewpoint : Layer
  {
    public override LayerType Type { get { return LayerType.Viewpoint; } }
    public int Version { get; set; }
    public Color Color { get; set; }
    public Vector4 Sphere { get; set; }
    public bool AllowTranslationX { get; set; }
    public bool AllowTranslationY { get; set; }
    public bool AllowTranslationZ { get; set; }
    public bool Exporting { get; set; }
    public bool ShowingVolume { get; set; }

    /// <summary>
    /// The type of viewpoint. Possible values are "FloorLevel" and "EyeLevel".
    /// </summary>
    public string TypeStr { get; set; } = "FloorLevel";
  }
}
