using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a picture layer.
  /// </summary>
  public class LayerImplementationPicture : LayerImplementation
  {
    public PictureType Type { get; set; }
    public bool ViewerLocked { get; set; }
    public long DataFileOffset { get; set; }
    public string ImportFilePath { get; set; }
  }
}
