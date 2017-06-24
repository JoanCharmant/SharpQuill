using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public static class QuillSequenceReader
  {
    public static Sequence Read(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new InvalidOperationException();

      if (!Directory.Exists(path))
        throw new InvalidOperationException();

      string filename = Path.Combine(path, "Quill.json");
      if (!File.Exists(filename))
        throw new InvalidOperationException();

      Sequence s = new Sequence();

      // Open and parse JSON, and fill in sequence.
      

      return s;
    }
  }
}
