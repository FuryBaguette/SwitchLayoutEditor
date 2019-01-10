//TODO: not complete, before release this has to be implemented in the differ and in SwitchThemes injector and installer


//using ExtensionMethods;
//using Syroot.BinaryData;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Text;
//using static SwitchThemes.Common.Custom.BFLYT;

//namespace SwitchThemes.Common.Custom
//{
//	public class Txt1Pane : EditablePane
//	{
//		public UInt16 TextLength { get; set; }
//		public UInt16 RestrictedTextLength { get; set; }
//		public UInt16 MaterialIndex { get; set; }
//		public UInt16 FontIndex { get; set; }

//		byte TextAlign;
//		public OriginX HorizontalAlignment
//		{
//			get => (OriginX)((TextAlign >> 2) & 0x3);
//			set
//			{
//				TextAlign &= unchecked((byte)(~0xC));
//				TextAlign |= (byte)((byte)(value) << 2);
//			}
//		}
//		public OriginX VerticalAlignment
//		{
//			get => (OriginX)((TextAlign) & 0x3);
//			set
//			{
//				TextAlign &= unchecked((byte)(~0x3));
//				TextAlign |= (byte)(value);
//			}
//		}

//		public enum LineAlign : byte
//		{
//			Unspecified = 0,
//			Left = 1,
//			Center = 2,
//			Right = 3,
//		};
//		public LineAlign LineAlignment { get; set; }

//		byte flags;
//		public bool PerCharTransform
//		{
//			get => (flags & 0x10) != 0;
//			set => flags = value ? (byte)(flags | 0x10) : unchecked((byte)(flags & (~0x10)));
//		}
//		public bool RestrictedTextLengthEnabled
//		{
//			get => (flags & 0x2) != 0;
//			set => flags = value ? (byte)(flags | 0x2) : unchecked((byte)(flags & (~0x2)));
//		}
//		public bool ShadowEnabled
//		{
//			get => (flags & 1) != 0;
//			set => flags = value ? (byte)(flags | 1) : unchecked((byte)(flags & (~1)));
//		}

//		public enum BorderType : byte
//		{
//			Sdandard = 0,
//			DeleteBorder = 1,
//			RenderTwoCycles = 2,
//		};
//		public BorderType BorderFormat
//		{
//			get => (BorderType)((flags >> 2) & 0x3);
//			set
//			{
//				flags &= unchecked((byte)(~0xC));
//				flags |= (byte)value;
//			}
//		}

//		public float ItalicTilt { get; set;}
//		public Color FontTopColor { get; set; }
//		public Color FontBottomColor { get; set; }
//		public Vector2 FontXYSize { get; set; }
//		public float CharacterSpace { get; set; }
//		public float LineSpace { get; set; }

//		public Txt1Pane(BasePane p, ByteOrder b) : base(p, b)
//		{
//			BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
//			dataReader.ByteOrder = b;
//			dataReader.Position = 0x54 - 8;
//		}

//		protected override void ApplyChanges(BinaryDataWriter bin)
//		{
//			base.ApplyChanges(bin);
//		}
//	}
//}
