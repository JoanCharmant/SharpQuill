using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a camera layer.
  /// </summary>
  public class LayerImplementationCamera : LayerImplementation
  {
    /// <summary>
    /// Field of view of the camera.
    /// </summary>
    public float FOV { get; set; }
  }
}
