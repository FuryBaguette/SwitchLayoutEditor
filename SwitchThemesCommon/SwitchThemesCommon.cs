﻿using SwitchThemes.Common.Bntxx;
using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SwitchThemes.Common
{
    static class SwitchThemesCommon
    {
		public const string CoreVer = "3.4";
		const string LoadFileText =
			"To create a theme open an szs first, these are the patches available in this version:" +
			"{0} \r\n" +
			"Always read the instructions because they are slightly different for each version";

		public static byte[] GenerateNXTheme(ThemeFileManifest info, byte[] image, string layout = null)
		{
			Dictionary<string, byte[]> Files = new Dictionary<string, byte[]>();
			Files.Add("info.json", Encoding.UTF8.GetBytes(info.Serialize()));
			Files.Add("image.dds", image);
			if (layout != null)
				Files.Add("layout.json", Encoding.UTF8.GetBytes(layout));

			var sarc = SARCExt.SARC.PackN(new SARCExt.SarcData() {  endianness = ByteOrder.LittleEndian, Files = Files, HashOnly = false} );
			return ManagedYaz0.Compress(sarc.Item2, 1, (int)sarc.Item1);
		}

		public static string GeneratePatchListString(IEnumerable<PatchTemplate> Templates)
		{
			var sortedTemplates = Templates.OrderBy(x => x.FirmName).Reverse();

			string curSection = "";
			string FileList = "";
			foreach (var p in sortedTemplates)
			{
				if (curSection != p.FirmName)
				{
					curSection = p.FirmName;
					FileList += $"\r\nFor {curSection}: \r\n";
				}
				FileList += $"  - {p.TemplateName} : the file is called {p.szsName} from title {p.TitleId}\r\n";
			}
			return string.Format(LoadFileText, FileList);
		}

		public static BflytFile.PatchResult PatchLayouts(SARCExt.SarcData sarc, LayoutFilePatch[] Files)
		{
			foreach (var p in Files)
			{
				if (!sarc.Files.ContainsKey(p.FileName))
					return BflytFile.PatchResult.Fail;
				var target = new BflytFile(sarc.Files[p.FileName]);
				var res = target.ApplyLayoutPatch(p.Patches);
				if (res != BflytFile.PatchResult.OK)
					return res;
				sarc.Files[p.FileName] = target.SaveFile();
			}
			return BflytFile.PatchResult.OK;
		}

		public static BflytFile.PatchResult PatchBgLayouts(SARCExt.SarcData sarc, PatchTemplate template)
		{
			BflytFile BflytFromSzs(string name) => new BflytFile(sarc.Files[name]);
			BflytFile MainFile = BflytFromSzs(template.MainLayoutName);
			var res = MainFile.PatchBgLayout(template);
			if (res == BflytFile.PatchResult.OK)
			{
				sarc.Files[template.MainLayoutName] = MainFile.SaveFile();
				foreach (var f in template.SecondaryLayouts)
				{
					BflytFile curTarget = BflytFromSzs(f);
					curTarget.PatchTextureName(template.MaintextureName, template.SecondaryTexReplace);
					sarc.Files[f] = curTarget.SaveFile();
				}
			}
			return res;
		}

		public static BflytFile.PatchResult PatchBntx(SARCExt.SarcData sarc, byte[] DDS, PatchTemplate targetPatch)
		{
			QuickBntx q = new QuickBntx(new BinaryDataReader(new MemoryStream(sarc.Files[@"timg/__Combined.bntx"])));
			if (q.Rlt.Length != 0x80)
			{
				return BflytFile.PatchResult.Fail;
			}
			q.ReplaceTex(targetPatch.MaintextureName, DDS);
			DDS = null;
			sarc.Files[@"timg/__Combined.bntx"] = null;
			sarc.Files[@"timg/__Combined.bntx"] = q.Write();
			return BflytFile.PatchResult.OK;
		}

		public static BflytFile.PatchResult PatchBntx(SARCExt.SarcData sarc, DDSEncoder.DDSLoadResult DDS, PatchTemplate targetPatch)
		{
			QuickBntx q = new QuickBntx(new BinaryDataReader(new MemoryStream(sarc.Files[@"timg/__Combined.bntx"])));
			if (q.Rlt.Length != 0x80)
			{
				return BflytFile.PatchResult.Fail;
			}
			q.ReplaceTex(targetPatch.MaintextureName, DDS);
			DDS = null;
			sarc.Files[@"timg/__Combined.bntx"] = null;
			sarc.Files[@"timg/__Combined.bntx"] = q.Write();
			return BflytFile.PatchResult.OK;
		}

		public static PatchTemplate DetectSarc(SARCExt.SarcData sarc, IEnumerable<PatchTemplate> Templates)
		{
			bool SzsHasKey(string key) => sarc.Files.ContainsKey(key);

			if (!SzsHasKey(@"timg/__Combined.bntx"))
				return null;

			foreach (var p in Templates)
			{
				if (!SzsHasKey(p.MainLayoutName))
					continue;
				bool isTarget = true;
				foreach (string s in p.SecondaryLayouts)
				{
					if (!SzsHasKey(s))
					{
						isTarget = false;
						break;
					}
				}
				if (!isTarget) continue;
				foreach (string s in p.FnameIdentifier)
				{
					if (!SzsHasKey(s))
					{
						isTarget = false;
						break;
					}
				}
				if (!isTarget) continue;
				foreach (string s in p.FnameNotIdentifier)
				{
					if (SzsHasKey(s))
					{
						isTarget = false;
						break;
					}
				}
				if (!isTarget) continue;
				return p;
			}
			return null;
		}

		public static Dictionary<string, string> PartToFileName = new Dictionary<string, string>() {
			{"home","ResidentMenu.szs"},
			{"lock","Entrance.szs"},
			{"user","MyPage.szs"},
			{"apps","Flaunch.szs"},
			{"set","Set.szs"},
			{"news","Notification.szs"},
		};

	}
}
