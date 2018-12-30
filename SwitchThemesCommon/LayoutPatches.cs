﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SwitchThemes.Common
{
	public class LayoutPatch
    {
		public string PatchName;
		public string AuthorName;
		public string TargetName;
		public LayoutFilePatch[] Files;

		public override string ToString() => PatchName + " by " + AuthorName;

		public bool IsCompatible(SARCExt.SarcData szs)
		{
			for (int i = 0; i < Files.Length; i++)
			{
				if (!szs.Files.ContainsKey(Files[i].FileName)) return false;
				string TargetFileAsString = ASCIIEncoding.ASCII.GetString(szs.Files[Files[i].FileName]);
				for (int j = 0; j < Files[i].Patches.Length; j++)
					if (!TargetFileAsString.Contains(Files[i].Patches[j].PaneName))
						return false;
			}
			return true;
		}

		public string AsJson()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
#if WIN
				Formatting = Formatting.Indented,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
#endif
			};
			return JsonConvert.SerializeObject(this, settings);
		}

#if DEBUG && WIN
		public static void CreateTestTemplates()
		{
			var p = new LayoutPatch()
			{
				PatchName = "test patch",
				AuthorName = "exelix",
				Files = new LayoutFilePatch[]
				{
					new LayoutFilePatch()
					{
						FileName = "blyt/RdtBase.bflyt",
						Patches = new PanePatch[]
						{
							new PanePatch()
							{
								PaneName = "L_BtnNoti",
								Position = new NullableVector3{ X = 0, Y= -500},
							}
						}
					}
				}
			};
			System.IO.File.WriteAllText("ExtraLayouts.json", JsonConvert.SerializeObject(p));
		}
#endif
		public static LayoutPatch LoadTemplate(string json) =>
			JsonConvert.DeserializeObject<LayoutPatch>(json);
	}

	public class LayoutFilePatch
	{
		public string FileName;
		public PanePatch[] Patches;
	}

	public class PanePatch
	{
		public string PaneName;
		public NullableVector3? Position = null;
		public NullableVector3? Rotation = null;
		public NullableVector2? Scale = null;
		public NullableVector2? Size = null;
		public bool? Visible = null;

		public string ColorTL = null;
		public string ColorTR = null;
		public string ColorBL = null;
		public string ColorBR = null;
	}

	public struct NullableVector3 { public float? X, Y, Z; public NullableVector3(float x, float y, float z) { X = x;Y = y;Z = z; } }
	public struct Vector3 { public float X, Y, Z; public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; } }
	public struct NullableVector2 { public float? X, Y; public NullableVector2(float x, float y) { X = x; Y = y;} }
	public struct Vector2 { public float X, Y; public Vector2(float x, float y) { X = x; Y = y; } }
}
