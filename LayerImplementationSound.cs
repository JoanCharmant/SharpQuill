using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class LayerImplementationSound : LayerImplementation
  {
    public long DataFileOffset;
    public string ImportFilePath;
    public SoundType Type;
    public float Gain;
    public bool Loop;
    public SoundAttenuation Attenuation;
    public List<SoundModifier> Modifiers;
  }
}
