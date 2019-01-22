using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtensionMethods;
using SARCExt;
using SwitchThemes.Common;
using SwitchThemes.Common.Custom;
using SwitchThemesCommon.Bflyt;
using Syroot.BinaryData;
using static SwitchThemes.Common.Custom.BFLYT;

namespace SwitchThemes
{
	public static class LayoutDiff
	{
		readonly static string[] IgnorePaneList = new string[] { "usd1", "lyt1", "mat1", "txl1", "fnl1", "grp1", "pae1", "pas1" };

		public static LayoutPatch Diff(SarcData original, SarcData edited)
		{
			List<LayoutFilePatch> Patches = new List<LayoutFilePatch>();
			if (!ScrambledEquals<string>(original.Files.Keys, edited.Files.Keys))
			{
				MessageBox.Show("The provided archives don't have the same files");
				return null;
			}
			foreach (var f in original.Files.Keys.Where(x => x.EndsWith(".bflyt")))
			{
				if (original.Files[f].SequenceEqual(edited.Files[f])) continue;
				BFLYT or = new BFLYT(original.Files[f]);
				BFLYT ed = new BFLYT(edited.Files[f]);
				string[] orPaneNames = GetPaneNames(or);
				string[] edPaneNames = GetPaneNames(ed);
				List<PanePatch> curFile = new List<PanePatch>();
				for (int i = 0; i < edPaneNames.Length; i++)
				{
					if (edPaneNames[i] == null || !(ed[i] is EditablePane)) continue;
					var j = Array.IndexOf(orPaneNames, edPaneNames[i]);
					if (j == -1) continue;
					if (ed[i].data.SequenceEqual(or[j].data)) continue;
					if (!(or[j] is EditablePane)) continue;
					PanePatch curPatch = new PanePatch() { PaneName = edPaneNames[i] };
					var orPan = (EditablePane)(or[j]);
					var edPan = (EditablePane)(ed[i]);
					if (!VecEqual(edPan.Position, orPan.Position))
						curPatch.Position = ToNullVec(edPan.Position);
					if (!VecEqual(edPan.Rotation, orPan.Rotation))
						curPatch.Rotation = ToNullVec(edPan.Rotation);
					if (!VecEqual(edPan.Scale, orPan.Scale))
						curPatch.Scale = ToNullVec(edPan.Scale);
					if (!VecEqual(edPan.Size, orPan.Size))
						curPatch.Size = ToNullVec(edPan.Size);
					if (edPan.Visible != orPan.Visible)
						curPatch.Visible = edPan.Visible;
					if (edPan is Pic1Pane && orPan is Pic1Pane)
					{
						var edPic = (Pic1Pane)edPan;
						var orPic = (Pic1Pane)orPan;
						if (edPic.ColorTopLeft != orPic.ColorTopLeft)
							curPatch.ColorTL = ColorToLEByte(edPic.ColorTopLeft);
						if (edPic.ColorTopRight != orPic.ColorTopRight)
							curPatch.ColorTR = ColorToLEByte(edPic.ColorTopRight);
						if (edPic.ColorBottomLeft != orPic.ColorBottomLeft)
							curPatch.ColorBL = ColorToLEByte(edPic.ColorBottomLeft);
						if (edPic.ColorBottomRight != orPic.ColorBottomRight)
							curPatch.ColorBR = ColorToLEByte(edPic.ColorBottomRight);
					}
					curFile.Add(curPatch);
				}
				if (curFile.Count > 0)
					Patches.Add(new LayoutFilePatch() { FileName = f, Patches = curFile.ToArray() });
			}
			if (Patches.Count == 0)
			{
				MessageBox.Show("Couldn't find any difference");
				return null; 
			}
			return new LayoutPatch()
			{
				PatchName = "diffPatch",
				AuthorName = "autoDiff",
				Files = Patches.ToArray()
			};
		}

		static bool VecEqual(Vector3 v, Vector3 v1) => v.X == v1.X && v.Y == v1.Y && v.Z == v1.Z;
		static NullableVector3 ToNullVec(Vector3 v) => new NullableVector3() { X = v.X, Y = v.Y, Z = v.Z };
		static bool VecEqual(Vector2 v, Vector2 v1) => v.X == v1.X && v.Y == v1.Y;
		static NullableVector2 ToNullVec(Vector2 v) => new NullableVector2() { X = v.X, Y = v.Y };

		static string ColorToLEByte(System.Drawing.Color col) => ((uint)(col.R | col.G << 8 | col.B << 16 | col.A << 24)).ToString("X");

		static string[] GetPaneNames(BFLYT layout)
		{
			string TryGetPaneName(BasePane p)
			{
				if (p.data.Length < 0x18 + 4) return null;
				BinaryDataReader dataReader = new BinaryDataReader(new MemoryStream(p.data), Encoding.ASCII, false);
				dataReader.ByteOrder = layout.FileByteOrder;
				dataReader.ReadInt32(); //Unknown
				string PaneName = "";
				for (int i = 0; i < 0x18; i++)
				{
					var c = dataReader.ReadChar();
					if (c == 0) break;
					PaneName += c;
				}
				return PaneName;
			}

			List<string> str = new List<string>();
			foreach (var p in layout.Panes)
			{
				string res = null;
				if (!IgnorePaneList.Contains(p.name))
					res = TryGetPaneName(p);
				str.Add(res);
			}
			return str.ToArray();
		}

		public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2)
		{
			var cnt = new Dictionary<T, int>();
			foreach (T s in list1)
			{
				if (cnt.ContainsKey(s))
				{
					cnt[s]++;
				}
				else
				{
					cnt.Add(s, 1);
				}
			}
			foreach (T s in list2)
			{
				if (cnt.ContainsKey(s))
				{
					cnt[s]--;
				}
				else
				{
					return false;
				}
			}
			return cnt.Values.All(c => c == 0);
		}
	}
}
