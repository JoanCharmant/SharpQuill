using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuill
{
  public class QBinReader : BinaryReader
  {
    public QBinReader(Stream _stream)
			: base(_stream)
		{
    }

    public PaintLayerData ReadPaintLayer()
    {
      PaintLayerData pl = new PaintLayerData();

      int count = ReadInt32();
      List<Stroke> strokes = ReadStrokes(count);
      
      return pl;
    }

    private List<Stroke> ReadStrokes(int count)
    {
      List<Stroke> strokes = new List<Stroke>();
      if (count == 0)
        return strokes;

      for (int i = 0; i < count; i++)
        strokes.Add(ReadStroke());
       
      return strokes;
    }

    private Stroke ReadStroke()
    {
      Stroke stroke = new Stroke();

      stroke.u1 = ReadInt32();
      stroke.u2 = ReadInt32();
      stroke.BoundingBox = ReadBoundingBox();
      stroke.u3 = ReadInt16();
      stroke.u4 = ReadInt16();
      stroke.CountVertices = ReadInt32();
      for (int i = 0; i < stroke.CountVertices; i++)
        stroke.Vertices.Add(ReadVertex());
      
      return stroke;
    }

    private BoundingBox ReadBoundingBox()
    {
      BoundingBox bbox = new BoundingBox();

      bbox.MinX = ReadSingle();
      bbox.MaxX = ReadSingle();
      bbox.MinY = ReadSingle();
      bbox.MaxY = ReadSingle();
      bbox.MinZ = ReadSingle();
      bbox.MaxZ = ReadSingle();

      return bbox;
    }

    private Vertex ReadVertex()
    {
      Vertex v = new Vertex();

      v.u1 = ReadSingle();
      v.u2 = ReadSingle();
      v.u3 = ReadSingle();
      v.u4 = ReadSingle();
      v.u5 = ReadSingle();
      v.u6 = ReadSingle();
      v.u7 = ReadSingle();
      v.u8 = ReadSingle();
      v.u9 = ReadSingle();
      v.Color = ReadColor();
      v.u10 = ReadSingle();
      v.u11 = ReadSingle();
      
      return v;
    }

    private Color ReadColor()
    {
      Color c = new Color();

      c.R = ReadSingle();
      c.G = ReadSingle();
      c.B = ReadSingle();

      return c;
    }
  }
}
