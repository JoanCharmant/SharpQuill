using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// Model layers wrap a mesh.
  /// </summary>
  public class LayerModel : Layer
  {
    public override LayerType Type { get { return LayerType.Model; } }

    public long DataFileOffset { get; set; }
    public string ImportFilePath { get; set; }
    public ModelShadingMode ShadingModel { get; set; }
    public bool RenderWireFrame { get; set; }
  }
}
