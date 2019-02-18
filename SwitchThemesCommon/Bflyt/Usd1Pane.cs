using ExtensionMethods;
using SwitchThemesCommon.Bflyt;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Text;
using static SwitchThemes.Common.Custom.BFLYT;
using System.Linq;
using System.Collections;
using System.Windows.Forms;

namespace SwitchThemes.Common.Custom
{
	public class Usd1Pane : BasePane, ICustomTypeDescriptor
	{
		ByteOrder order;

		Dictionary<string, dynamic> Properties;

		EditableProperty[] PropertyMetadata;
		public struct EditableProperty
		{
			public enum ValueType : byte
			{
				data = 0,
				int32 = 1,
				single = 2,
				other = 3
			}

			public string Name;
			public long ValueOffset;
			public ushort ValueCount;
			public ValueType type;
		}

		public override string ToString()
		{
			return "[usd1 pane]";
		}

		public Usd1Pane(BasePane p, ByteOrder b) : base(p)
		{
			BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(data));
			order = b;
			dataReader.ByteOrder = order;
			dataReader.Position = 0;
			ushort Count = dataReader.ReadUInt16();
			ushort Unk1 = dataReader.ReadUInt16();
			Properties = new Dictionary<string, dynamic>();
			var prop = new List<EditableProperty>();
			for (int i = 0; i < Count; i++)
			{
				var EntryOffset = dataReader.Position;
				uint NameOffset = dataReader.ReadUInt32();
				uint DataOffset = dataReader.ReadUInt32();
				ushort ValueLen = dataReader.ReadUInt16();
				byte dataType = dataReader.ReadByte();
				dataReader.ReadByte(); //padding ?

				if (!(dataType == 1 || dataType == 2)) continue;

				var pos = dataReader.Position;
				dataReader.Position = EntryOffset + NameOffset;
				string propName = dataReader.ReadString(BinaryStringFormat.ZeroTerminated);
				var type = (EditableProperty.ValueType)dataType;

				dataReader.Position = EntryOffset + DataOffset;
				dynamic values;
				if (type == EditableProperty.ValueType.int32)
					values = dataReader.ReadUInt32s(ValueLen);
				else
					values = dataReader.ReadSingles(ValueLen);


				prop.Add(new EditableProperty()
				{
					Name = propName,
					type = type,
					ValueOffset = EntryOffset + DataOffset,
					ValueCount = ValueLen,
				});

				Properties.Add(propName, values);

				dataReader.Position = pos;
			}
			PropertyMetadata = prop.ToArray();
		}

		protected override void ApplyChanges(BinaryDataWriter bin)
		{
			bin.Write(data);
			bin.Position = 0;
			foreach (var m in PropertyMetadata)
			{
				if ((byte)m.type != 1 && (byte)m.type != 2) continue;
				if (!Properties.ContainsKey(m.Name)) continue;
				if (((Array)Properties[m.Name]).Length > m.ValueCount)
					MessageBox.Show("The number of values for usd1 panes must not be changed, extra values will be skipped");
				if (((Array)Properties[m.Name]).Length < m.ValueCount)
				{
					MessageBox.Show("The number of values for usd1 panes must not be changed, removed values will be ignored");
					continue;
				}
				bin.Position = m.ValueOffset;
				for (int i = 0; i < m.ValueCount; i++)
					bin.Write(Properties[m.Name][i]);
			}
		}

		public override void WritePane(BinaryDataWriter bin)
		{
			using (var mem = new MemoryStream())
			{
				BinaryDataWriter dataWriter = new BinaryDataWriter(mem);
				dataWriter.ByteOrder = bin.ByteOrder;
				ApplyChanges(dataWriter);
				data = mem.ToArray();
			}
			base.WritePane(bin);
		}

		public override BasePane Clone()
		{
			return new Usd1Pane(base.Clone(), order);
		}

		#region PropertyGridMagic
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			ArrayList properties = new ArrayList();
			foreach (DictionaryEntry e in (IDictionary)Properties)
			{
				properties.Add(new DictionaryPropertyDescriptor(Properties, e.Key));
			}

			PropertyDescriptor[] props =
				(PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

			return new PropertyDescriptorCollection(props);
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		class DictionaryPropertyDescriptor : PropertyDescriptor
		{
			IDictionary _dictionary;
			object _key;

			internal DictionaryPropertyDescriptor(IDictionary d, object key)
				: base(key.ToString(), null)
			{
				_dictionary = d;
				_key = key;
			}

			public override Type PropertyType
			{
				get { return _dictionary[_key].GetType(); }
			}

			public override void SetValue(object component, object value)
			{
				_dictionary[_key] = value;
			}

			public override object GetValue(object component)
			{
				return _dictionary[_key];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type ComponentType
			{
				get { return null; }
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override void ResetValue(object component)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
}
