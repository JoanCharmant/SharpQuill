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

    public UInt32 ReadLastStrokeId()
    {
      UInt32 strokeId = ReadUInt32();
      return strokeId;
    }

    public PaintLayerData ReadPaintLayer()
    {
      PaintLayerData pl = new PaintLayerData();

      int count = ReadInt32();
      pl.Strokes = ReadStrokes(count);
      
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

      stroke.Id = ReadUInt32();
      
      stroke.u2 = ReadInt32();
      stroke.BoundingBox = ReadBoundingBox();
      stroke.BrushType = (BrushType)ReadInt16();
      stroke.DisableRotationalOpacity = ReadBoolean();
      stroke.u3 = ReadByte();
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

      v.Position = ReadVector3();
      v.Normal = ReadVector3();
      v.Tangeant = ReadVector3();
      v.Color = ReadColor();
      v.Opacity = ReadSingle();
      v.Width = ReadSingle();
      
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

    private Vector3f ReadVector3()
    {
      Vector3f v = new Vector3f();

      v.X = ReadSingle();
      v.Y = ReadSingle();
      v.Z = ReadSingle();

      return v;
    }
  }
}
