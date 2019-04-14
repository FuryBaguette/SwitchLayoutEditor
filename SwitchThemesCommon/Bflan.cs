using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExtensionMethods;
using Syroot.BinaryData;
using System.Linq;
using System.ComponentModel;

namespace SwitchThemesCommon
{
	public class BflanSection
	{
		public string TypeName { get; set; }
		public byte[] Data;

		public BflanSection(string name, byte[] data)
		{
			TypeName = name;
			Data = data;
		}

		public virtual void BuildData(ByteOrder byteOrder)
		{
			return;
		}

		public virtual void Write(BinaryDataWriter bin)
		{
			if (TypeName.Length != 4) throw new Exception("unexpected type len");
			bin.Write(TypeName, BinaryStringFormat.NoPrefixOrTermination);
			bin.Write((int)Data.Length);
			bin.Write(Data);
		}
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Pat1Section : BflanSection
	{
		public ushort AnimationOrder { get; set; }
		public string Name { get; set; }
		public byte ChildBinding { get; set; }
		public List<string> Groups { get; set; } = new List<string>();

		public uint Unk_StartOfFile { get; set; }
		public uint Unk_EndOfFile { get; set; }
		public byte[] Unk_EndOfHeader { get; set; }

		const int groupNameLen = 0x24;

		public Pat1Section(byte[] data, ByteOrder bo) : base("pat1", data)
		{
			ParseData(bo);
		}

		void ParseData(ByteOrder bo)
		{
			BinaryDataReader bin = new BinaryDataReader(new MemoryStream(Data));
			bin.ByteOrder = bo;
			AnimationOrder = bin.ReadUInt16();
			var groupCount = bin.ReadUInt16();
			if (groupCount != 1) throw new Exception("File with unexpected group count");
			var animName = bin.ReadUInt32() - 8; //all offsets are shifted by 8 cause this byte block doesn't include the section name and size
			var groupNames = bin.ReadUInt32() - 8;
			Unk_StartOfFile = bin.ReadUInt16();
			Unk_EndOfFile = bin.ReadUInt16();
			ChildBinding = bin.ReadByte();
			//Unk_EndOfHeader = bin.ReadBytes((int)animName - (int)bin.Position);
			bin.BaseStream.Position = animName;
			Name = bin.ReadString(BinaryStringFormat.ZeroTerminated);
			for (int i = 0; i < groupCount; i++)
			{
				bin.BaseStream.Position = groupNames + i * groupNameLen;
				Groups.Add(bin.ReadFixedLenString(groupNameLen));
			}
			if (Unk_StartOfFile != 0 || Unk_EndOfFile != 0)
			{
				Console.Write("");
			}
		}

		public override string ToString() => "[Pat1 section]";
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Pai1Section : BflanSection
	{
		public uint FrameSize { get; set; }
		public byte Flags { get; set; }
		public List<string> Textures { get; set; } = new List<string>();
		public List<PaiEntry> Entries = new List<PaiEntry>();

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class PaiEntry
		{
			public enum AnimationTarget : byte
			{
				Pane = 0,
				Material = 1
			}

			public string Name { get; set; }
			public AnimationTarget Target { get; set; }
			public List<PaiTag> Tags = new List<PaiTag>();

			public PaiEntry(BinaryDataReader bin)
			{
				uint SectionStart = (uint)bin.Position;
				Name = bin.ReadFixedLenString(28);
				var tagCount = bin.ReadByte();
				Target = (AnimationTarget)bin.ReadByte();
				bin.ReadUInt16(); //padding
				var tagOffsets = bin.ReadUInt32();
				bin.BaseStream.Position = tagOffsets + SectionStart;
				for (int i = 0; i < tagCount; i++)
					Tags.Add(new PaiTag(bin, (byte)Target));
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class PaiTag
		{
			public uint Unknown { get; set; }
			public string TagType { get; set; }
			public List<PaiTagEntry> Entries = new List<PaiTagEntry>();

			public PaiTag(BinaryDataReader bin, byte TargetType)
			{
				var sectionStart = (uint)bin.Position;
				if (TargetType == 2)
					Unknown = bin.ReadUInt32();
				TagType = bin.ReadString(4);
				var entryCount = bin.ReadUInt32();
				bin.Position = bin.ReadUInt32() + sectionStart;
				for (int i = 0; i < entryCount; i++)
					Entries.Add(new PaiTagEntry(bin));
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class PaiTagEntry
		{
			public byte Index { get; set; }
			public byte AnimationTarget { get; set; }
			public UInt16 DataType { get; set; }
			public List<KeyFrame> KeyFrames { get; set; } = new List<KeyFrame>();

			public PaiTagEntry(BinaryDataReader bin)
			{
				uint tagStart = (uint)bin.Position;
				Index = bin.ReadByte();
				AnimationTarget = bin.ReadByte();
				DataType = bin.ReadUInt16();
				var KeyFrameCount = bin.ReadUInt16();
				bin.ReadUInt16(); //Padding
				bin.BaseStream.Position = tagStart + bin.ReadUInt32(); //offset to first keyframe
				for (int i = 0; i < KeyFrameCount; i++)
					KeyFrames.Add(new KeyFrame(bin, DataType));
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class KeyFrame
		{
			public float Frame { get; set; }
			public float Value { get; set; }
			public float Blend { get; set; }
			public KeyFrame(BinaryDataReader bin, UInt16 DataType)
			{
				Frame = bin.ReadSingle();
				if (DataType == 2)
				{
					Value = bin.ReadSingle();
					Blend = bin.ReadSingle();
				}
				else if (DataType == 1)
				{
					Value = (float)bin.ReadInt16();
					Blend = (float)bin.ReadInt16();
				}
				else throw new Exception("Unexpected data type for keyframe");
			}
		}

		public Pai1Section(byte[] data, ByteOrder bo) : base("pai1", data)
		{
			ParseData(bo);
		}

		void ParseData(ByteOrder bo)
		{
			BinaryDataReader bin = new BinaryDataReader(new MemoryStream(Data));
			bin.ByteOrder = bo;
			FrameSize = bin.ReadUInt16();
			Flags = bin.ReadByte();
			bin.ReadByte(); //padding
			var texCount = bin.ReadUInt16();
			var entryCount = bin.ReadUInt16();
			var entryTable = bin.ReadUInt32() - 8;
			if (texCount != 0)
			{
				var texTable = bin.ReadUInt32() - 8; //apparently the texTable field is missing if texCount == 0
				var pos = bin.Position;
				bin.Position = texTable;
				for (int i = 0; i < texCount; i++)
					Textures.Add(bin.ReadString(BinaryStringFormat.ZeroTerminated));
				bin.Position = pos;
			}
			for (int i = 0; i < entryCount; i++)
			{
				bin.Position = entryTable + i * 4;
				bin.Position = bin.ReadUInt32() - 8;
				Entries.Add(new PaiEntry(bin));
			}
		}

		public override string ToString() => "[Pai1 section]";
	}

	public class Bflan
	{
		public ByteOrder byteOrder { get; set; }
		public uint Version { get; set; }

		public List<BflanSection> Sections = new List<BflanSection>();

		[Browsable(false)]
		public Pat1Section patData => Sections.Where(x => x is Pat1Section).FirstOrDefault() as Pat1Section;
		[Browsable(false)]
		public Pai1Section paiData => Sections.Where(x => x is Pai1Section).FirstOrDefault() as Pai1Section;

		public Bflan(byte[] data) => ParseFile(new MemoryStream(data));

		void ParseFile(Stream input)
		{
			var bin = new BinaryDataReader(input);
			if (!bin.ReadBytes(4).Matches("FLAN"))
				throw new Exception("Wrong bflan magic");
			byte BOM = bin.ReadByte();
			if (BOM == 0xFF) byteOrder = ByteOrder.LittleEndian;
			else if (BOM == 0xFE) byteOrder = ByteOrder.BigEndian;
			else throw new Exception("Unexpected BOM");
			bin.ByteOrder = byteOrder;
			bin.ReadByte(); //Second byte of the byte order mask
			if (bin.ReadUInt16() != 0x14) throw new Exception("Unexpected bflan header size");
			Version = bin.ReadUInt32();
			bin.ReadUInt32(); //FileSize
			var sectionCount = bin.ReadUInt16();
			bin.ReadUInt16(); //padding ?

			for (int i = 0; i < sectionCount; i++)
			{
				string sectionName = bin.ReadString(4);
				int sectionSize = bin.ReadInt32(); //this includes the first 8 bytes we read here
				byte[] sectionData = bin.ReadBytes(sectionSize - 8);
				BflanSection s = null;
				switch (sectionName)
				{
					case "pat1":
						s = new Pat1Section(sectionData, bin.ByteOrder);
						break;
					case "pai1":
						s = new Pai1Section(sectionData, bin.ByteOrder);
						break;
					default:
						throw new Exception("unexpected section");
				}
				Sections.Add(s);
			}
		}

	}
}
