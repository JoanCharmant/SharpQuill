using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  /// <summary>
  /// The type-specific data of a sound layer.
  /// </summary>
  public class LayerImplementationSound : LayerImplementation
  {
    public long DataFileOffset { get; set; }
    public string ImportFilePath { get; set; }
    public SoundType Type { get; set; }
    public float Gain { get; set; }
    public bool Loop { get; set; }
    public SoundAttenuation Attenuation { get; set; }
    public List<SoundModifier> Modifiers { get; set; } = new List<SoundModifier>();
  }
}
