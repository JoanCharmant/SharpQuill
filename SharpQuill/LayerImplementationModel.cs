using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationModel
  {
    public long DataFileOffset { get; set; }
    public string ImportFilePath { get; set; }
    public ModelShadingMode ShadingModel { get; set; }
    public bool RenderWireFrame { get; set; }
  }
}
