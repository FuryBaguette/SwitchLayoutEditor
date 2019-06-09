using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using ExtensionMethods;
using System.ComponentModel;

namespace SwitchThemesCommon.Bflyt
{
	public class BflytMaterial
	{
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public struct TextureReference
		{
			public override string ToString() => $"{{Texture reference}}";

			public enum WRAPS : byte
			{
				NearClamp = 0,
				NearRepeat = 1,
				NearMirror = 2,
				GX2MirrorOnce = 3,
				Clamp = 4,
				Repeat = 5,
				Mirror = 6,
				GX2MirrorOnceBorder = 7
			}

			public UInt16 TextureId { get; set; }
			public WRAPS WrapS { get; set; }
			public WRAPS WrapT { get; set; }
		}

		byte[] Data;
		Int32 bitflags;

		string _name = "";
		public string Name
		{
			get => _name;
			set
			{
				if (value.Length > 27) throw new Exception("This name is too long");
				_name = value;
			}
		}

		//TODO: finish the implementation
		//public Color ForegroundColor { get; set; }
		//public Color BackgroundColor { get; set; }

		//public bool HasAlphaComparisonConditions { get; set; }
		//public bool HasIndirectAdjustment { get; set; }
		//public bool HasShadowBlending { get; set; }

		public TextureReference[] Textures { get; set; }

		public BflytMaterial(byte[] data, ByteOrder bo, uint version)
		{
			Data = data;
			BinaryDataReader bin = new BinaryDataReader(new MemoryStream(data));
			bin.ByteOrder = bo;
			Name = bin.ReadFixedLenString(28);
			if (version >= 0x08000000)
			{
				bitflags = bin.ReadInt32();
				bin.ReadUInt64();
				bin.ReadUInt32();
			}
			else
			{
				bin.ReadUInt64();
				bitflags = bin.ReadInt32();
			}
			Textures = new TextureReference[bitflags & 3];
			for (int i = 0; i < (bitflags & 3); i++)
			{
				Textures[i] = new TextureReference()
				{
					TextureId = bin.ReadUInt16(),
					WrapS = (TextureReference.WRAPS)bin.ReadByte(),
					WrapT = (TextureReference.WRAPS)bin.ReadByte()
				};
			}
		}

		public byte[] Write(uint version, ByteOrder _bo)
		{
			if (Textures.Length > 3) throw new Exception($"[{Name}] A material can have no more than 3 texture references");
			bitflags &= ~3;
			bitflags |= Textures.Length;

			var mem = new MemoryStream();
			BinaryDataWriter bin = new BinaryDataWriter(mem);
			bin.ByteOrder = _bo;
			bin.Write(Data);
			bin.BaseStream.Position = 0;
			bin.WriteFixedLenString(Name, 27);
			if (version >= 0x08000000)
			{
				bin.Write(bitflags);
				bin.BaseStream.Position += 12;
			}
			else
			{
				bin.BaseStream.Position += 8;
				bin.Write(bitflags);
			}

			for (int i = 0; i < Textures.Length; i++)
			{
				bin.Write(Textures[i].TextureId);
				bin.Write((byte)Textures[i].WrapS);
				bin.Write((byte)Textures[i].WrapT);
			}
			return mem.ToArray();
		}

		public override string ToString() => Name;
	}
}
