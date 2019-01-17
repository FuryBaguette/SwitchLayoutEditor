using ExtensionMethods;
using SwitchThemesCommon.Bflyt;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using static SwitchThemes.Common.Custom.BFLYT;

namespace SwitchThemes.Common.Custom
{
	public class Pic1Pane : EditablePane
	{
		public Color ColorTopRight { get; set; } = Color.White;
		public Color ColorTopLeft { get; set; } = Color.White;
		public Color ColorBottomRight { get; set; } = Color.White;
		public Color ColorBottomLeft { get; set; } = Color.White;

		public UInt16 MaterialIndex { get; set; } = 0;
		public Vector2[] UVCoords { get; set; } = new Vector2[0];

		public Pic1Pane(ByteOrder b) : base("pic1",b)	{	}

		public Pic1Pane(BasePane p, ByteOrder b) : base(p, b)
		{
			BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
			dataReader.ByteOrder = b;
			dataReader.Position = 0x54 - 8;
			ColorTopLeft = dataReader.ReadColorRGBA();
			ColorTopRight = dataReader.ReadColorRGBA();
			ColorBottomLeft = dataReader.ReadColorRGBA();
			ColorBottomRight = dataReader.ReadColorRGBA();
			MaterialIndex = dataReader.ReadUInt16();
			byte UVCount = dataReader.ReadByte();
			dataReader.ReadByte(); //padding
			UVCoords = new Vector2[UVCount];
			for (int i = 0; i < UVCount; i++)
				UVCoords[i] = dataReader.ReadVector2();
		}

		protected override void ApplyChanges(BinaryDataWriter bin)
		{
			base.ApplyChanges(bin);
			bin.Write(ColorTopLeft);
			bin.Write(ColorTopRight);
			bin.Write(ColorBottomLeft);
			bin.Write(ColorBottomRight);
			bin.Write(MaterialIndex);
			bin.Write((byte)UVCoords.Length);
			bin.Write((byte)0);
			for (int i = 0; i < UVCoords.Length; i++)
				bin.Write(UVCoords[i]);
		}

		public override BasePane Clone()
		{
			return new Pic1Pane(base.Clone(),order);
		}
	}
}
