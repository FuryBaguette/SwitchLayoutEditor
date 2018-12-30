﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SwitchThemes.Common
{
	public class ThemeFileManifest
	{
		public int Version;
		public string Author;
		public string ThemeName;
		public string LayoutInfo;
		public string Target;
		public bool UseCommon5X;

		public string Serialize() { return JsonConvert.SerializeObject(this); }
		public static ThemeFileManifest Deserialize(string json) { return JsonConvert.DeserializeObject<ThemeFileManifest>(json); }
	}

	public class PatchTemplate
	{
		public string FirmName = "";
		public string TemplateName;
		public string szsName;
		public string TitleId;

		public string[] FnameIdentifier;
		public string[] FnameNotIdentifier;

		public string MainLayoutName;
		public string MaintextureName;
		public string PatchIdentifier;
		public string[] targetPanels;
		public string[] SecondaryLayouts;
		public string SecondaryTexReplace;

		public string NXThemeName;

		public bool NoRemovePanel = false;
		//public bool ReplaceTarget = false;
		//public PatchTemplate[] UnpatchTargets;

#if WIN
#if DEBUG
		public static void BuildTemplateFile()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			};

			string json = JsonConvert.SerializeObject(DefaultTemplates.templates, settings);
			System.IO.File.WriteAllText("DefaultTemplates.json", json);
		}
#endif
		public static PatchTemplate[] LoadTemplates()=>
			JsonConvert.DeserializeObject<PatchTemplate[]>(System.IO.File.ReadAllText("ExtraTemplates.json"));
#endif
	}

	public static class DefaultTemplates
	{
		public static readonly PatchTemplate[] templates =
		{
			new PatchTemplate() { TemplateName = "home and applets" , szsName = "common.szs", TitleId = "0100000000001000", FirmName = "<= 5.X",
				FnameIdentifier = new string[] { },
				FnameNotIdentifier = new string[] { @"blyt/DHdrSoft.bflyt" } ,
				MainLayoutName = @"blyt/BgNml.bflyt",
				MaintextureName = "White1x1_180^r",
				PatchIdentifier = "exelixBG",
				targetPanels = new string[] { "P_Bg_00" },
				SecondaryLayouts = new string[] { @"blyt/SystemAppletFader.bflyt" },
				SecondaryTexReplace = "White1x1^r",
				NXThemeName = "home",
			},
			new PatchTemplate() { TemplateName = "home menu" , szsName = "ResidentMenu.szs", TitleId = "0100000000001000",  FirmName = "6.X",
				FnameIdentifier = new string[] { },
				FnameNotIdentifier = new string[] { @"anim/RdtBtnShop_LimitB.bflan" } ,
				MainLayoutName = @"blyt/BgNml.bflyt",
				MaintextureName = "White1x1A128^s",
				PatchIdentifier = "exelixBG",
				targetPanels = new string[] { "P_Bg_00" },
				SecondaryLayouts = new string[] { @"blyt/IconError.bflyt" },
				SecondaryTexReplace = "White1x1A64^t",
				NXThemeName = "home"
			},
			new PatchTemplate() { TemplateName = "lock screen" , szsName = "Entrance.szs", TitleId = "0100000000001000",  FirmName = "all firmwares",
				FnameIdentifier = new string[] { },
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName =@"blyt/EntMain.bflyt",
				MaintextureName = "White1x1^s",
				PatchIdentifier = "exelixLK",
				targetPanels = new string[] { "P_BgL", "P_BgR" },
				SecondaryLayouts = new string[] { @"blyt/EntBtnResumeSystemApplet.bflyt"},
				SecondaryTexReplace ="White1x1^r",
				NXThemeName = "lock"
			},
			new PatchTemplate() { TemplateName = "user page" , szsName = "MyPage.szs", TitleId = "0100000000001013",  FirmName = "all firmwares",
				FnameIdentifier = new string[] { @"blyt/MypUserIconMini.bflyt" },
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName = @"blyt/BaseTop.bflyt",
				MaintextureName = "NavBg_03^d",
				PatchIdentifier = "exelixMY",
				targetPanels = new string[] { "L_AreaNav", "L_AreaMain" },
				SecondaryLayouts = new string[] { @"blyt/BgNav_Root.bflyt"},
				SecondaryTexReplace = "White1x1A0^t",
				NXThemeName = "user"
			},
			new PatchTemplate() { TemplateName = "home menu only" , szsName = "ResidentMenu.szs", TitleId = "0100000000001000",  FirmName = "<= 5.X",
				FnameIdentifier = new string[] { @"anim/RdtBtnShop_LimitB.bflan" },
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName = @"blyt/RdtBase.bflyt",
				MaintextureName = "White1x1A128^s",
				PatchIdentifier = "exelixResBG",
				targetPanels = new string[] { "L_BgNml" },
				SecondaryLayouts = new string[] { @"blyt/IconError.bflyt" },
				SecondaryTexReplace = "White1x1A64^t",
				NXThemeName = "home"
			},
			new PatchTemplate() { TemplateName = "all apps menu" , szsName = "Flaunch.szs", TitleId = "0100000000001000", FirmName = "6.X",
				FnameIdentifier = new string[] { @"blyt/FlcBtnIconGame.bflyt", @"anim/BaseBg_Loading.bflan" }, //anim/BaseBg_Loading.bflan for 6.0
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName = @"blyt/BgNml.bflyt",
				MaintextureName = "NavBg_03^d",
				PatchIdentifier = "exelixFBG",
				targetPanels = new string[] { "P_Bg_00" },
				SecondaryLayouts = new string[] { @"blyt/BgNav_Root.bflyt"},
				SecondaryTexReplace = "White1x1A64^t",
				NXThemeName = "apps"
			},			
			new PatchTemplate() { TemplateName = "settings applet" , szsName = "Set.szs", TitleId = "0100000000001000",  FirmName = "6.X",
				FnameIdentifier = new string[] { @"blyt/SetCntDataMngPhoto.bflyt" , @"blyt/SetSideStory.bflyt"}, //blyt/SetSideStory.bflyt for 6.0 detection
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName = @"blyt/BgNml.bflyt",
				MaintextureName = "NavBg_03^d",
				PatchIdentifier = "exelixSET",
				targetPanels = new string[] { "P_Bg_00" },
				SecondaryLayouts = new string[] { @"blyt/BgNav_Root.bflyt"}, 
				SecondaryTexReplace = "White1x1A0^t",
				NXThemeName = "set"
			},
			new PatchTemplate() { TemplateName = "news applet" , szsName = "Notification.szs", TitleId = "0100000000001000", FirmName = "6.X",
				FnameIdentifier = new string[] { @"blyt/NtfBase.bflyt", @"blyt/NtfImage.bflyt" }, //blyt/NtfImage.bflyt for 6.0
				FnameNotIdentifier = new string[] { } ,
				MainLayoutName = @"blyt/BgNml.bflyt",
				MaintextureName = "NavBg_03^d",
				PatchIdentifier = "exelixNEW",
				targetPanels = new string[] { "P_Bg_00" },
				SecondaryLayouts = new string[] { @"blyt/BgNavNoHeader.bflyt",@"blyt/BgNav_Root.bflyt"},
				SecondaryTexReplace = "White1x1^r",
				NXThemeName = "news"
			},
		};
	}
}
