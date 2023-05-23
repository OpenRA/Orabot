using System.IO;

namespace Orabot.Core.Objects.OpenRaReplay
{
	public static class StreamExtensions
	{
		public static byte ReadUInt8(this Stream s)
		{
			var b = s.ReadByte();
			if (b == -1)
				throw new EndOfStreamException();
			return (byte)b;
		}

		public static int ReadInt32(this Stream s)
		{
			return s.ReadUInt8() | s.ReadUInt8() << 8 | s.ReadUInt8() << 16 | s.ReadUInt8() << 24;
		}
	}
}
