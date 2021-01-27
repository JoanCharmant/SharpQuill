using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationPicture : LayerImplementation
  {
    public PictureType Type;
    public bool ViewerLocked;
    public long DataFileOffset;
    public string ImportFilePath;
  }
}
