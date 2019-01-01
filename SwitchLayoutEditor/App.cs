using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;
using Newtonsoft.Json;
using System;
using SARCExt;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SwitchThemes.Common;
using SwitchThemes.Common.Custom;

namespace SwitchLayoutEditor
{
    public class App
    {
        static SarcData CommonSzs = null;
        static PatchTemplate targetPatch = null;
        static BFLYT bflytLayout = null;

        public static void Main()
        {   
            /*var jsonBtn = new HTMLButtonElement
            {
                InnerHTML = "Upload JSON",
                OnClick = (ev) =>
                {
                    UploadLayoutBtn();
                }
            };*/

            var cszBtn = new HTMLButtonElement
            {
                InnerHTML = "Upload CSZ",
                OnClick = (ev) =>
                {
                    UploadSZSBtn();
                }
            };

            Document.GetElementById<HTMLDivElement>("uploadFile").AppendChild(cszBtn);
            //Document.Body.AppendChild(button);
        }

        public static void UploadLayoutBtn()
        {
            Document.GetElementById<HTMLInputElement>("JsonUploader").Click();
            return;
        }

        public static void UploadSZSBtn()
        {
            Document.GetElementById<HTMLInputElement>("SZSUploader").Click();
            return;
        }

        public static void UploadJSON(Uint8Array arr, string fileName)
        {
            System.Text.Encoding enc = System.Text.Encoding.ASCII;
            string json = enc.GetString(arr.ToArray());

            var layout = JsonConvert.DeserializeObject<Layout>(json);
            LayoutFiles RdtBase = getFileByName(layout, "RdtBase");
            if (RdtBase != null)
                SetDefaultsByFile(RdtBase, "RdtBase");
            return;
        }

        public static void UploadSZS(Uint8Array arr, string fileName)
        {
            byte[] sarc = ManagedYaz0.Decompress(arr.ToArray());
            CommonSzs = SARCExt.SARC.UnpackRamN(sarc);
            sarc = null;
            targetPatch = SwitchThemesCommon.DetectSarc(CommonSzs, DefaultTemplates.templates);
            if (targetPatch == null)
            {
                Console.WriteLine("This is not a valid theme file, if it's from a newer firmware it's not compatible with this tool yet");
                CommonSzs = null;
                targetPatch = null;
                return;
            }
            Console.WriteLine("Detected " + targetPatch.TemplateName + " " + targetPatch.FirmName);

            foreach (var file in CommonSzs.Files)
            {
                if (file.Key == "blyt/RdtBase.bflyt")
                {
                    bflytLayout = new BFLYT(file.Value);
                    var json = JsonConvert.SerializeObject(bflytLayout);
                    //Console.WriteLine(json);
                    /*foreach (var p in bflytLayout.Panes.Where(x => x is BFLYT.EditablePane))
                    {
                        Console.WriteLine(p.ToString());
                    }*/
                }
            }
            /*bflytLayout = new BFLYT(sarc);
            Console.WriteLine(bflytLayout.RootPane.name);*/
            return;
        }

        public static int PatchExists(Patches defaultPatches, List<Patches> patches)
        {
            for (int i = 0; i <= patches.Count() - 1; i++)
            {
                if (defaultPatches.PaneName == patches[i].PaneName)
                    return i;
            }
            return -42;
        }

        public static void SetDefaultsByFile(LayoutFiles RdtBase, string name)
        {
            XMLHttpRequest req = new XMLHttpRequest();
            req.ResponseType = XMLHttpRequestResponseType.String;
            req.OnReadyStateChange = () =>
            {
                if (req.ReadyState != AjaxReadyState.Done) return;
                string DownloadRes = req.Response as string;
                if (DownloadRes == null || DownloadRes.Length == 0)
                {
                    Console.WriteLine("Error getting default values");
                    return;
                }
                List<Patches> defaultPatches = JsonConvert.DeserializeObject<List<Patches>>(DownloadRes);
                DownloadRes = null;
                List<Patches> patches = RdtBase.Patches;

                for (int i = 0; i <= defaultPatches.Count() - 1; i++)
                {
                    int patchNb = PatchExists(defaultPatches[i], patches);
                    if (patchNb < 0)
                        patches.Add(defaultPatches[i]);
                    else
                    {
                        if (patches[patchNb].Position != null)
                        {
                            if (String.IsNullOrEmpty(patches[patchNb].Position.X))
                                patches[patchNb].Position.X = defaultPatches[i].Position.X;
                            if (String.IsNullOrEmpty(patches[patchNb].Position.Y))
                                patches[patchNb].Position.Y = defaultPatches[i].Position.Y;
                            if (String.IsNullOrEmpty(patches[patchNb].Position.Z))
                                patches[patchNb].Position.Z = defaultPatches[i].Position.Z;
                        }
                        else patches[patchNb].Position = defaultPatches[i].Position;

                        if (patches[patchNb].Size != null)
                        {
                            if (String.IsNullOrEmpty(patches[patchNb].Size.X))
                                patches[patchNb].Size.X = defaultPatches[i].Size.X;
                            if (String.IsNullOrEmpty(patches[patchNb].Size.Y))
                                patches[patchNb].Size.Y = defaultPatches[i].Size.Y;
                        }
                        else patches[patchNb].Size = defaultPatches[i].Size;

                        if (patches[patchNb].Scale != null)
                        {
                            if (String.IsNullOrEmpty(patches[patchNb].Scale.X))
                                patches[patchNb].Scale.X = defaultPatches[i].Scale.X;
                            if (String.IsNullOrEmpty(patches[patchNb].Scale.Y))
                                patches[patchNb].Scale.Y = defaultPatches[i].Scale.Y;
                        }
                        else patches[patchNb].Scale = defaultPatches[i].Scale;

                        if (String.IsNullOrEmpty(patches[patchNb].ParentName)) patches[patchNb].ParentName = defaultPatches[i].ParentName;
                    }
                }

                StartEditor(patches);
            };
            req.Open("GET", "defaults/" + name + ".json", true);
            req.Send();
        }

        public static LayoutFiles getFileByName(Layout layout, string fileName)
        {
            string fullName = "blyt/" + fileName + ".bflyt";

            for (var i = 0; i <= layout.Files.Count() - 1; i++)
            {
                if (layout.Files[i].FileName == fullName)
                    return (layout.Files[i]);
            }
            return null;
        }

        public static void StartEditor(List<Patches> patch)
        {
            var editorView = Document.GetElementById<HTMLDivElement>("editor_view");
            var uploadFile = Document.GetElementById<HTMLDivElement>("uploadFile");
            editorView.Hidden = false;
            uploadFile.Hidden = true;

            Script.Call("checkClick", JsonConvert.SerializeObject(patch));
        }


    }

    public class Layout
    {
        [JsonProperty("Files")]
        public List<LayoutFiles> Files { get; set; }
    }

    public class LayoutFiles
    {
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        [JsonProperty("Patches")]
        public List<Patches> Patches { get; set; }
    }

    public class Patches
    {
        [JsonProperty("PaneName")]
        public string PaneName { get; set; }

        [JsonProperty("Position")]
        public PanePosition Position { get; set; }

        [JsonProperty("Scale")]
        public PaneScale Scale { get; set; }

        [JsonProperty("Size")]
        public PaneSize Size { get; set; }

        [JsonProperty("Parent")]
        public string ParentName { get; set; }

        [JsonProperty("Visible")]
        public bool Visible { get; set; }
    }

    public class PanePosition
    {
        [JsonProperty("X")]
        public string X { get; set; }

        [JsonProperty("Y")]
        public string Y { get; set; }

        [JsonProperty("Z")]
        public string Z { get; set; }
    }

    public class PaneScale
    {
        [JsonProperty("X")]
        public string X { get; set; }

        [JsonProperty("Y")]
        public string Y { get; set; }
    }

    public class PaneSize
    {
        [JsonProperty("X")]
        public string X { get; set; }

        [JsonProperty("Y")]
        public string Y { get; set; }
    }
}