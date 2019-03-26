using ExtensionMethods;
using SwitchThemes.Common;
using SwitchThemesCommon.Bflyt;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitchThemes.Common.Custom
{
	public class BFLYT
	{
		public BasePane this[int index]
		{
			get => Panes[index];
			set => Panes[index] = value;
		}

		public ByteOrder FileByteOrder;
		public class BasePane
		{
			public BasePane Parent;
			public List<BasePane> Children = new List<BasePane>();

			[Browsable(true)]
			[TypeConverter(typeof(ExpandableObjectConverter))]
			virtual public Usd1Pane UserData { get; set; }  = null;

			public override string ToString()
			{
				return $"[Unknown pane type: {name}]";
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
				if (name != "usd1")
					UserData = basePane.UserData;
			}

			public BasePane(string _name, BinaryDataReader bin)
			{
				name = _name;
				length = bin.ReadInt32();
				data = bin.ReadBytes(length - 8);
			}

			public BasePane(string _name, byte[] _data)
			{
				name = _name;
				data = _data;
				length = data.Length + 8;
			}

			public virtual void WritePane(BinaryDataWriter bin)
			{
				bin.Write(name, BinaryStringFormat.NoPrefixOrTermination);
				length = data.Length + 8;
				bin.Write(length);
				bin.Write(data);
				if (UserData != null)
					UserData.WritePane(bin);
			}

			public virtual BasePane Clone()
			{
				MemoryStream mem = new MemoryStream();
				BinaryDataWriter bin = new BinaryDataWriter(mem);
				WritePane(bin);
				BasePane res = new BasePane(name, (byte[])data.Clone());
				if (name != "usd1" && UserData != null)
					res.UserData = (Usd1Pane)UserData.Clone();
				return res;
			}

			protected virtual void ApplyChanges(BinaryDataWriter bin) { }
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
					uint endPos = (uint)dataWriter.Position;
					dataWriter.Position = 4 + i * 4;
					dataWriter.Write(off - 4);
					dataWriter.Position = endPos;
				}
				while (dataWriter.BaseStream.Position % 4 != 0)
					dataWriter.Write((byte)0);
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
			UInt16 PaneCount = (UInt16)Panes.Count;
			for (int i = 0; i < Panes.Count; i++)
				if (Panes[i].UserData != null) PaneCount++;
			bin.Write(PaneCount);
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
					case "usd1":
						Panes.Last().UserData = new Usd1Pane(bin); 
						break;
					default:
						var pane = new BasePane(name, bin);
						Panes.Add(DetectProperPaneClass(pane));
						break;
				}
			}

			RebuildParentingData();
		}

		public void RemovePane(BasePane pane)
		{
			int paneIndex = Panes.IndexOf(pane);
			if (Panes[paneIndex + 1].name == "pas1")
			{
				int ChildLevel = 0;
				int TargetDelete = -1;
				for (int i = paneIndex + 2; i < Panes.Count; i++)
				{
					if (Panes[i].name == "pae1")
					{
						if (ChildLevel == 0)
						{
							TargetDelete = i;
							break;
						}
						ChildLevel--;
					}
					if (Panes[i].name == "pas1")
						ChildLevel++;
				}
				if (TargetDelete == -1)
					throw new Exception("Couldn't find the children data");
				Panes.RemoveRange(paneIndex, TargetDelete - paneIndex);
			}
			else Panes.Remove(pane);
			if (pane.Parent != null)
				pane.Parent.Children.Remove(pane);
			RebuildParentingData();
		}

		public void AddPane(BasePane pane, BasePane Parent)
		{
			if (Parent == null) Parent = RootPane;
			int parentIndex = Panes.IndexOf(Parent);
			if (Panes[parentIndex + 1].name != "pas1")
			{
				if (Parent.Children.Count != 0) throw new Exception("Inconsistend data !");
				Panes.Insert(parentIndex + 1, new BasePane("pas1", 8));
				Panes.Insert(parentIndex + 2, new BasePane("pae1", 8));
			}
			Parent.Children.Add(pane);
			pane.Parent = Parent;
			Panes.Insert(parentIndex + 2, pane);
			RebuildParentingData();
		}

		void RebuildParentingData()
		{
			BasePane CurrentRoot = null;
			int RootIndex = -1;
			for (int i = 0; i < Panes.Count; i++)
			{
				if (Panes[i] is EditablePane && ((EditablePane)Panes[i]).PaneName == "RootPane")
				{
					CurrentRoot = Panes[i];
					RootIndex = i;
					break;
				}
			}
			this.RootPane = CurrentRoot ?? throw new Exception("Couldn't find the root pane");
			RootPane.Children.Clear();
			RootPane.Parent = null;
			for (int i = RootIndex + 1; i < Panes.Count; i++)
			{
				if (Panes[i].name == "pas1")
				{
					CurrentRoot = Panes[i - 1];
					CurrentRoot.Children.Clear();
					continue;
				}
				else if (Panes[i].name == "pae1")
				{
					CurrentRoot = CurrentRoot.Parent;
					if (CurrentRoot == null) return;
					continue;
				}
				Panes[i].Parent = CurrentRoot;
				CurrentRoot.Children.Add(Panes[i]);
			}
			if (CurrentRoot != null)
				throw new Exception("Unexpected pane data ending: one or more children sections are not closed by the end of the file");
		}

		BasePane DetectProperPaneClass(BasePane pane)
		{			
			switch (pane.name)
			{
				case "pic1":
					return new Pic1Pane(pane, FileByteOrder);
                case "txt1":
                    return new Txt1Pane(pane, FileByteOrder);
                default:
					if (pane.data.Length < 0x4C || pane.name == "grp1" || pane.name == "cnt1")
						return pane;
					return new EditablePane(pane, FileByteOrder);
			}
		}

        public static string TryGetPaneName(BasePane p)
        {
            if (p.data.Length < 0x18 + 4) return null;
            BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(p.data), Encoding.ASCII, false);
            dataReader.ByteOrder = ByteOrder.LittleEndian;
            dataReader.ReadInt32();
            string PaneName = "";
            for (int i = 0; i < 0x18; i++)
            {
                var c = dataReader.ReadChar();
                if (c == 0) break;
                PaneName += c;
            }
            return PaneName;
        }

        public string[] GetPaneNames()
        {
            string[] paneNames = new string[Panes.Count];
            for (int i = 0; i < Panes.Count; i++)
                paneNames[i] = TryGetPaneName(Panes[i]);
            return paneNames;
        }

        public bool ApplyLayoutPatch(PanePatch[] Patches)
        {
            string[] paneNames = GetPaneNames();
            for (int i = 0; i < Patches.Length; i++)
            {
                int index = Array.IndexOf(paneNames, Patches[i].PaneName);
                if (index == -1)
                    return false;
                var p = Patches[i];
                var e = new EditablePane(Panes[index], FileByteOrder);
                Panes[index] = e;
                if (p.Visible != null)
                    e.Visible = p.Visible.Value;
                #region ChangeTransform
                if (p.Position != null)
                {
                    e.Position = new Vector3(
                        p.Position.Value.X ?? e.Position.X,
                        p.Position.Value.Y ?? e.Position.Y,
                        p.Position.Value.Z ?? e.Position.Z);
                }
                if (p.Rotation != null)
                {
                    e.Rotation = new Vector3(
                        p.Rotation.Value.X ?? e.Rotation.X,
                        p.Rotation.Value.Y ?? e.Rotation.Y,
                        p.Rotation.Value.Z ?? e.Rotation.Z);
                }
                if (p.Scale != null)
                {
                    e.Scale = new Vector2(
                        p.Scale.Value.X ?? e.Scale.X,
                        p.Scale.Value.Y ?? e.Scale.Y);
                }
                if (p.Size != null)
                {
                    e.Size = new Vector2(
                        p.Size.Value.X ?? e.Size.X,
                        p.Size.Value.Y ?? e.Size.Y);
                }
				#endregion
				/*#region ColorDataForPic1
                if (e.name == "pic1")
                {
                    if (p.ColorTL != null)
                        e.ColorData[0] = Convert.ToUInt32(p.ColorTL, 16);
                    if (p.ColorTR != null)
                        e.ColorData[1] = Convert.ToUInt32(p.ColorTR, 16);
                    if (p.ColorBL != null)
                        e.ColorData[2] = Convert.ToUInt32(p.ColorBL, 16);
                    if (p.ColorBR != null)
                        e.ColorData[3] = Convert.ToUInt32(p.ColorBR, 16);
                }
                #endregion*/
				#region usdPane
				if (e.UserData != null)
				{
					Usd1Pane usd = e.UserData;
					foreach (var patch in p.UsdPatches)
					{
						var v = usd.FindName(patch.PropName);
						if (v == null)
							usd.Properties.Add(new Usd1Pane.EditableProperty() { Name = patch.PropName, value = patch.PropValues, type = (Usd1Pane.EditableProperty.ValueType)patch.type });
						if (v != null && v.ValueCount == patch.PropValues.Length && (int)v.type == patch.type)
							v.value = patch.PropValues;
					}
					usd.ApplyChanges();
				}
				#endregion
			}
			return true;
        }
    }
}
