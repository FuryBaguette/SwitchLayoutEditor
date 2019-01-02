using ExtensionMethods;
using SwitchThemes.Common;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchThemes.Common.Custom
{
	public class BFLYT
	{
		public ByteOrder FileByteOrder;
		public class BasePane
		{
			public BasePane Parent;
			public List<BasePane> Children = new List<BasePane>();

			public override string ToString()
			{
				return $"Pane {name} len: 0x{length.ToString("X")}";
			}

			public readonly string name;
			public Int32 length;
			public byte[] data;

			public BasePane(string _name, int len)
			{
				name = _name;
				length = len;
				data = new byte[length - 8];
			}

			//used for PropertyEditablePane, data is not cloned so it can be changed from the other classs
			public BasePane(BasePane basePane)
			{
				name = basePane.name;
				length = basePane.length;
				data = basePane.data;
			}

			public BasePane(string _name, BinaryDataReader bin)
			{
				name = _name;
				length = bin.ReadInt32();
				data = bin.ReadBytes(length - 8);
			}

			public virtual void WritePane(BinaryDataWriter bin)
			{
				bin.Write(name, BinaryStringFormat.NoPrefixOrTermination);
				length = data.Length + 8;
				bin.Write(length);
				bin.Write(data);
			}
		}

        public class CusRectangle
        {
            public int x, y, width, height, scaleX, scaleY;

            public CusRectangle (int _x, int _y, int _width, int _height)
            {
                x = _x;
                y = _y;
                width = _width;
                height = _height;
            }
        }

		public class EditablePane : BasePane
		{
			public CusRectangle transformedRect
			{
				get
				{
                    if (Alpha == 0 || !ParentVisibility)
                        return new CusRectangle(0,0,0,0);

					Vector2 ParentSize;

					if (Parent != null && Parent is EditablePane)
                        ParentSize = ((EditablePane)Parent).Size;
					else
                        ParentSize = new Vector2(0,0);

                    float RelativeX;
                    if (ParentOriginX == OriginX.Center) RelativeX = 0;
                    else if (ParentOriginX == OriginX.Right) RelativeX = ParentSize.X;
                    else RelativeX = ParentSize.X / 2;

                    float RelativeY;
                    if (ParentOriginY == OriginY.Center) RelativeY = 0;
                    else if (ParentOriginY == OriginY.Bottom) RelativeY = ParentSize.Y;
                    else RelativeY = ParentSize.Y / 2;

                    if (originX == OriginX.Center) RelativeX -= Size.X / 2;
                    else if (originX == OriginX.Right) RelativeX -= Size.X;

                    if (originY == OriginY.Center) RelativeY -= Size.Y / 2;
                    else if (originY == OriginY.Bottom) RelativeY -= Size.Y;

                    return new CusRectangle(
                        (int)((RelativeX)),
                        (int)((RelativeY)),
                        (int)(Size.X),
                        (int)(Size.Y));
                }
            }
			////My attempt to calculate the box position manually, doesn't work.
			//public Rectangle BoundingBox
			//{
			//	get 
			//	{
			//		if (Alpha == 0 || !ParentVisibility)
			//			return new Rectangle(0, 0, 0, 0);

			//		RectangleF ParentBox;
			//		if (Parent != null && Parent is EditablePane)
			//		{
			//			ParentBox = ((EditablePane)Parent).BoundingBox;
			//		}
			//		else
			//		{
			//			ParentBox = new RectangleF(0, 0, 0, 0);
			//		}

			//		float ActualW = Size.X * Scale.X;
			//		float ActualH = Size.Y * Scale.Y;

			//		float RelativeX;
			//		if (ParentOriginX == OriginX.Left) RelativeX = Position.X;
			//		else if (ParentOriginX == OriginX.Right) RelativeX = ParentBox.Width + Position.X;
			//		else RelativeX = ParentBox.Width / 2 + Position.X;

			//		float RelativeY;
			//		if (ParentOriginY == OriginY.Top) RelativeY = Position.Y;
			//		else if (ParentOriginY == OriginY.Bottom) RelativeY = ParentBox.Height - Position.Y;
			//		else RelativeY = ParentBox.Height / 2 + Position.Y;

			//		return new Rectangle(
			//			(int)((RelativeX + ParentBox.X) * ParentScale.X),
			//			(int)((RelativeY + ParentBox.Y) * ParentScale.Y),
			//			(int)(Size.X * ActualScale.X),
			//			(int)(Size.Y * ActualScale.Y));
			//	}
			//}

			//This is not an actual property, it's just to hide it from the view
			public bool ViewInEditor { get; set; } = true;

			public bool ParentVisibility
			{
				get
				{
					if (Scale.X == 0 || Scale.Y == 0 || Size.X == 0 || Size.Y == 0)
						return false;
					if (!Visible)
						return false;
					if (Parent != null && Parent is EditablePane)
					{
						return ((EditablePane)Parent).ParentVisibility && Visible;
					}
					return true;
				}
			}

			public Vector2 ParentScale
			{
				get
				{
					if (Parent != null && Parent is EditablePane)
					{
						return ((EditablePane)Parent).ActualScale;
					}
					return new Vector2(1, 1);
				}
			}

			public Vector2 ActualScale
			{
				get
				{
					if (Parent != null && Parent is EditablePane)
					{
						var pScale = ((EditablePane)Parent).ActualScale;
						return new Vector2(pScale.X * Scale.X, pScale.Y * Scale.Y);
					}
					return Scale;
				}
			}

			public enum OriginX : byte
			{
				Center = 0,
				Left = 1,
				Right = 2
			};

			public enum OriginY : byte
			{
				Center = 0,
				Top = 1,
				Bottom = 2
			};

			public override string ToString()
			{
				return $"Pane {name} {PaneName}";
			}

			byte _flag1;
			byte _flag2;
			public byte Alpha;
			public byte Unknown1;
			public readonly string PaneName;
			public readonly string UserInfo;
			public Vector3 Position { get; set; }
			public Vector3 Rotation { get; set; }
			public Vector2 Scale { get; set; }
			public Vector2 Size { get; set; }

			public bool Visible
			{
				get => (_flag1 & 0x1) == 0x1;
				set
				{
					if (value)
						_flag1 |= 0x1;
					else
						_flag1 &= 0xFE;
				}
			}

			public bool InfluenceAlpha
			{
				get => (_flag1 & 0x2) == 0x2;
				set
				{
					if (value)
						_flag1 |= 0x2;
					else
						_flag1 &= 0xFD;
				}
			}

			public OriginX originX
			{
				get => (OriginX)((_flag2 & 0xC0) >> 6);
				set
				{
					_flag2 &= unchecked((byte)(~0xC0));
					_flag2 |= (byte)((byte)value << 6);
				}
			}

			public OriginY originY
			{
				get => (OriginY)((_flag2 & 0x30) >> 4);
				set
				{
					_flag2 &= unchecked((byte)(~0x30));
					_flag2 |= (byte)((byte)value << 4);
				}
			}

			public OriginX ParentOriginX
			{
				get => (OriginX)((_flag2 & 0xC) >> 2);
				set
				{
					_flag2 &= unchecked((byte)(~0xC));
					_flag2 |= (byte)((byte)value << 2);
				}
			}

			public OriginY ParentOriginY
			{
				get => (OriginY)((_flag2 & 0x3));
				set
				{
					_flag2 &= unchecked((byte)(~0x3));
					_flag2 |= (byte)value;
				}
			}

			//public uint[] ColorData = null; //only for pic1 panes

			public EditablePane(BasePane p, ByteOrder order) : base(p)
			{
				BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
				dataReader.ByteOrder = order;

				string ReadBinaryString(int max)
				{
					string res = "";
					for (int i = 0; i < max; i++)
					{
						var c = (char)dataReader.ReadByte();
						if (c == 0) continue;
						res += c;
					}
					return res;
				}

				_flag1 = dataReader.ReadByte();
				_flag2 = dataReader.ReadByte();
				Alpha = dataReader.ReadByte();
				Unknown1 = dataReader.ReadByte();
				PaneName = ReadBinaryString(0x18);
				UserInfo = ReadBinaryString(0x8);
				Position = dataReader.ReadVector3();
				Rotation = dataReader.ReadVector3();
				Scale = dataReader.ReadVector2();
				Size = dataReader.ReadVector2();
				//if (name == "pic1")
				//{
				//	dataReader.BaseStream.Position = 0x54 - 8;
				//	ColorData = dataReader.ReadUInt32s(4);
				//}
			}

			public void ApplyChanges(ByteOrder order)
			{
				using (var mem = new MemoryStream())
				{
					BinaryDataWriter bin = new BinaryDataWriter(mem);
					bin.ByteOrder = order;
					bin.Write(data);
					bin.BaseStream.Position = 0;
					bin.Write(_flag1);
					bin.Write(_flag2);
					bin.Write(Alpha);
					bin.Write(Unknown1);
					bin.BaseStream.Position = 0x2C - 8;
					bin.Write(Position);
					bin.Write(Rotation);
					bin.Write(Scale);
					bin.Write(Size);
					//if (name == "pic1")
					//{
					//	bin.BaseStream.Position = 0x54 - 8;
					//	bin.Write(ColorData);
					//}
					data = mem.ToArray();
				}
			}

			public override void WritePane(BinaryDataWriter bin)
			{
				ApplyChanges(bin.ByteOrder);
				base.WritePane(bin);
			}
		}

		public class TextureSection : BasePane
		{
			public List<string> Textures = new List<string>();
			public TextureSection(BinaryDataReader bin) : base("txl1", bin)
			{
				BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
				dataReader.ByteOrder = bin.ByteOrder;
				int texCount = dataReader.ReadInt16();
				dataReader.ReadInt16(); //padding
				uint BaseOff = (uint)dataReader.Position;
				var Offsets = dataReader.ReadInt32s(texCount);
				foreach (var off in Offsets)
				{
					dataReader.Position = BaseOff + off;
					Textures.Add(dataReader.ReadString(BinaryStringFormat.ZeroTerminated));
				}
			}

			public TextureSection() : base("txl1", 8) { }

			public override void WritePane(BinaryDataWriter bin)
			{
				var newData = new MemoryStream();
				BinaryDataWriter dataWriter = new BinaryDataWriter(newData);
				dataWriter.ByteOrder = bin.ByteOrder;
				dataWriter.Write(Textures.Count);
				dataWriter.Write(new int[Textures.Count]);
				for (int i = 0; i < Textures.Count; i++)
				{
					uint off = (uint)dataWriter.Position;
					dataWriter.Write(Textures[i], BinaryStringFormat.ZeroTerminated);
					while (dataWriter.BaseStream.Position % 4 != 0)
						dataWriter.Write((byte)0);
					uint endPos = (uint)dataWriter.Position;
					dataWriter.Position = 4 + i * 4;
					dataWriter.Write(off - 4);
					dataWriter.Position = endPos;
				}
				data = newData.ToArray();
				base.WritePane(bin);
			}
		}

		public class MaterialsSection : BasePane
		{
			public List<byte[]> Materials = new List<byte[]>();
			public MaterialsSection(BinaryDataReader bin) : base("mat1", bin)
			{
				BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
				dataReader.ByteOrder = bin.ByteOrder;
				int matCount = dataReader.ReadInt16();
				dataReader.ReadInt16(); //padding
				var Offsets = dataReader.ReadInt32s(matCount).Select(x => x - 8).ToArray(); // offsets relative to the stream
				for (int i = 0; i < matCount; i++)
				{
					int matLen = (i == matCount - 1 ? (int)dataReader.BaseStream.Length : Offsets[i + 1]) - (int)dataReader.Position;
					Materials.Add(dataReader.ReadBytes(matLen));
				}
			}

			public MaterialsSection() : base("mat1", 8) { }

			public override void WritePane(BinaryDataWriter bin)
			{
				var newData = new MemoryStream();
				BinaryDataWriter dataWriter = new BinaryDataWriter(newData);
				dataWriter.ByteOrder = bin.ByteOrder;
				dataWriter.Write(Materials.Count);
				dataWriter.Write(new int[Materials.Count]);
				for (int i = 0; i < Materials.Count; i++)
				{
					uint off = (uint)dataWriter.Position;
					dataWriter.Write(Materials[i]);
					uint endPos = (uint)dataWriter.Position;
					dataWriter.Position = 4 + i * 4;
					dataWriter.Write(off + 8);
					dataWriter.Position = endPos;
				}
				data = newData.ToArray();
				base.WritePane(bin);
			}
		}

		public BasePane RootPane;
		public List<BasePane> Panes = new List<BasePane>();
		UInt32 version;

		public byte[] SaveFile()
		{
			var res = new MemoryStream();
			BinaryDataWriter bin = new BinaryDataWriter(res);
			bin.ByteOrder = FileByteOrder;
			bin.Write("FLYT", BinaryStringFormat.NoPrefixOrTermination);
			bin.Write((ushort)0xFEFF); //should match 0xFF 0xFE
			bin.Write((UInt16)0x14); //Header size
			bin.Write(version);
			bin.Write((Int32)0);
			bin.Write((UInt16)Panes.Count);
			bin.Write((UInt16)0); //padding
			foreach (var p in Panes)
				p.WritePane(bin);
			while (bin.BaseStream.Position % 4 != 0)
				bin.Write((byte)0);
			bin.BaseStream.Position = 0xC;
			bin.Write((uint)bin.BaseStream.Length);
			bin.BaseStream.Position = bin.BaseStream.Length;
			return res.ToArray();
		}

		public TextureSection GetTex
		{
			get
			{
				var res = (TextureSection)Panes.Find(x => x is TextureSection); ;
				if (res == null)
				{
					res = new TextureSection();
					Panes.Insert(2, res);
				}
				return res;
			}
		}

		public MaterialsSection GetMat
		{
			get
			{
				var res = (MaterialsSection)Panes.Find(x => x is MaterialsSection);
				if (res == null)
				{
					res = new MaterialsSection();
					Panes.Insert(3, res);
				}
				return res;
			}
		}

		public BFLYT(byte[] data) : this(new MemoryStream(data)) { }

		public BFLYT(Stream file)
		{
			BinaryDataReader bin = new BinaryDataReader(file);
			FileByteOrder = ByteOrder.LittleEndian;
			bin.ByteOrder = FileByteOrder;
			if (bin.ReadString(4) != "FLYT") throw new Exception("Wrong signature");
			var bOrder = bin.ReadUInt16(); //BOM
			if (bOrder == 0xFFFE)
			{
				FileByteOrder = ByteOrder.BigEndian;
				bin.ByteOrder = FileByteOrder;
			}
			bin.ReadUInt16(); //HeaderSize
			version = bin.ReadUInt32();
			bin.ReadUInt32(); //File size
			var sectionCount = bin.ReadUInt16();
			bin.ReadUInt16(); //padding

			BasePane CurrentParent = null;

			for (int i = 0; i < sectionCount; i++)
			{
				string name = bin.ReadString(4);
				switch (name)
				{
					case "txl1":
						Panes.Add(new TextureSection(bin));
						break;
					case "mat1":
						Panes.Add(new MaterialsSection(bin));
						break;
					case "pas1":
						CurrentParent = Panes.Last();
						Panes.Add(new BasePane(name, bin));
						break;
					case "pae1":
						CurrentParent = CurrentParent.Parent;
						Panes.Add(new BasePane(name, bin));
						break;
					default:
						var pane = new BasePane(name, bin);
						Panes.Add(pane.data.Length >= 0x4C && name != "usd1" && name != "grp1" ? new EditablePane(pane, FileByteOrder) : pane);
						if (CurrentParent != null)
						{
							CurrentParent.Children.Add(Panes.Last());
							Panes.Last().Parent = CurrentParent;
						}
						if (Panes.Last() is EditablePane && ((EditablePane)Panes.Last()).PaneName == "RootPane")
							RootPane = CurrentParent = Panes.Last();
						break;
				}
			}
		}
	}
}
