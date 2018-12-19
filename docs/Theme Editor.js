/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2018
 * @compiler Bridge.NET 17.5.0
 */
Bridge.assembly("Theme Editor", function ($asm, globals) {
    "use strict";

    Bridge.define("Theme_Editor.App", {
        main: function Main () {
            var $t;
            var jsonBtn = ($t = document.createElement("button"), $t.innerHTML = "Upload JSON", $t.onclick = function (ev) {
                Theme_Editor.App.UploadLayoutBtn();
            }, $t);

            /* var button = new HTMLButtonElement
            {
                InnerHTML = "Upload CSZ",
                OnClick = (ev) =>
                {
                    UploadSZSBtn();
                }
            };*/

            document.getElementById("uploadFile").appendChild(jsonBtn);
        },
        statics: {
            methods: {
                UploadLayoutBtn: function () {
                    document.getElementById("JsonUploader").click();
                    return;
                },
                UploadSZSBtn: function () {
                    document.getElementById("SZSUploader").click();
                    return;
                },
                UploadJSON: function (arr, fileName) {
                    var enc = System.Text.Encoding.ASCII;
                    var json = enc.GetString(System.Linq.Enumerable.from(arr).ToArray());

                    var layout = Newtonsoft.Json.JsonConvert.DeserializeObject(json, Theme_Editor.Layout);
                    var RdtBase = Theme_Editor.App.getFileByName(layout, "RdtBase");
                    if (RdtBase != null) {
                        Theme_Editor.App.SetDefaultsByFile(RdtBase, "RdtBase");
                    }
                    return;
                },
                UploadSZS: function (arr, fileName) {
                    return;
                },
                PatchExists: function (defaultPatches, patches) {
                    for (var i = 0; i <= ((System.Linq.Enumerable.from(patches).count() - 1) | 0); i = (i + 1) | 0) {
                        if (Bridge.referenceEquals(defaultPatches.PaneName, patches.getItem(i).PaneName)) {
                            return i;
                        }
                    }
                    return -42;
                },
                SetDefaultsByFile: function (RdtBase, name) {
                    var req = new XMLHttpRequest();
                    req.responseType = "String";
                    req.onreadystatechange = function () {
                        if (req.readyState !== 4) {
                            return;
                        }
                        var DownloadRes = Bridge.as(req.response, System.String);
                        if (DownloadRes == null || DownloadRes.length === 0) {
                            System.Console.WriteLine("Error getting default values");
                            return;
                        }
                        var defaultPatches = Newtonsoft.Json.JsonConvert.DeserializeObject(DownloadRes, System.Collections.Generic.List$1(Theme_Editor.Patches));
                        DownloadRes = null;
                        var patches = RdtBase.Patches;

                        for (var i = 0; i <= ((System.Linq.Enumerable.from(defaultPatches).count() - 1) | 0); i = (i + 1) | 0) {
                            var patchNb = Theme_Editor.App.PatchExists(defaultPatches.getItem(i), patches);
                            if (patchNb < 0) {
                                patches.add(defaultPatches.getItem(i));
                            } else {
                                if (patches.getItem(patchNb).Position != null) {
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Position.X)) {
                                        patches.getItem(patchNb).Position.X = defaultPatches.getItem(i).Position.X;
                                    }
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Position.Y)) {
                                        patches.getItem(patchNb).Position.Y = defaultPatches.getItem(i).Position.Y;
                                    }
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Position.Z)) {
                                        patches.getItem(patchNb).Position.Z = defaultPatches.getItem(i).Position.Z;
                                    }
                                } else {
                                    patches.getItem(patchNb).Position = defaultPatches.getItem(i).Position;
                                }

                                if (patches.getItem(patchNb).Size != null) {
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Size.X)) {
                                        patches.getItem(patchNb).Size.X = defaultPatches.getItem(i).Size.X;
                                    }
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Size.Y)) {
                                        patches.getItem(patchNb).Size.Y = defaultPatches.getItem(i).Size.Y;
                                    }
                                } else {
                                    patches.getItem(patchNb).Size = defaultPatches.getItem(i).Size;
                                }

                                if (patches.getItem(patchNb).Scale != null) {
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Scale.X)) {
                                        patches.getItem(patchNb).Scale.X = defaultPatches.getItem(i).Scale.X;
                                    }
                                    if (System.String.isNullOrEmpty(patches.getItem(patchNb).Scale.Y)) {
                                        patches.getItem(patchNb).Scale.Y = defaultPatches.getItem(i).Scale.Y;
                                    }
                                } else {
                                    patches.getItem(patchNb).Scale = defaultPatches.getItem(i).Scale;
                                }

                                if (System.String.isNullOrEmpty(patches.getItem(patchNb).ParentName)) {
                                    patches.getItem(patchNb).ParentName = defaultPatches.getItem(i).ParentName;
                                }
                            }
                        }

                        Theme_Editor.App.StartEditor(patches);
                    };
                    req.open("GET", "defaults/" + (name || "") + ".json", true);
                    req.send();
                },
                getFileByName: function (layout, fileName) {
                    var fullName = "blyt/" + (fileName || "") + ".bflyt";

                    for (var i = 0; i <= ((System.Linq.Enumerable.from(layout.Files).count() - 1) | 0); i = (i + 1) | 0) {
                        if (Bridge.referenceEquals(layout.Files.getItem(i).FileName, fullName)) {
                            return (layout.Files.getItem(i));
                        }
                    }
                    return null;
                },
                StartEditor: function (patch) {
                    var editorView = document.getElementById("editor_view");
                    var uploadFile = document.getElementById("uploadFile");
                    editorView.hidden = false;
                    uploadFile.hidden = true;

                    checkClick(Newtonsoft.Json.JsonConvert.SerializeObject(patch));
                }
            }
        }
    });

    Bridge.define("Theme_Editor.Layout", {
        fields: {
            Files: null
        }
    });

    Bridge.define("Theme_Editor.LayoutFiles", {
        fields: {
            FileName: null,
            Patches: null
        }
    });

    Bridge.define("Theme_Editor.PanePosition", {
        fields: {
            X: null,
            Y: null,
            Z: null
        }
    });

    Bridge.define("Theme_Editor.PaneScale", {
        fields: {
            X: null,
            Y: null
        }
    });

    Bridge.define("Theme_Editor.PaneSize", {
        fields: {
            X: null,
            Y: null
        }
    });

    Bridge.define("Theme_Editor.Patches", {
        fields: {
            PaneName: null,
            Position: null,
            Scale: null,
            Size: null,
            ParentName: null,
            Visible: false
        }
    });
});

//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAiZmlsZSI6ICJUaGVtZSBFZGl0b3IuanMiLAogICJzb3VyY2VSb290IjogIiIsCiAgInNvdXJjZXMiOiBbIkFwcC5jcyJdLAogICJuYW1lcyI6IFsiIl0sCiAgIm1hcHBpbmdzIjogIjs7Ozs7Ozs7Ozs7WUFpQllBLGNBQWNBLG1GQUdBQSxVQUFDQTtnQkFFUEE7Ozs7Ozs7Ozs7OztZQWFSQSxrREFBa0VBOzs7OztvQkFNbEVBO29CQUNBQTs7O29CQUtBQTtvQkFDQUE7O3NDQUcwQkEsS0FBZ0JBO29CQUUxQ0EsVUFBMkJBO29CQUMzQkEsV0FBY0EsY0FBY0EsNEJBQXFDQTs7b0JBRWpFQSxhQUFhQSw4Q0FBc0NBLE1BQVJBO29CQUMzQ0EsY0FBc0JBLCtCQUFjQTtvQkFDcENBLElBQUlBLFdBQVdBO3dCQUNYQSxtQ0FBa0JBOztvQkFDdEJBOztxQ0FHeUJBLEtBQWdCQTtvQkFFekNBOzt1Q0FHMEJBLGdCQUF3QkE7b0JBRWxEQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLDRCQUFjQTt3QkFFckVBLElBQUlBLGdEQUEyQkEsZ0JBQVFBOzRCQUNuQ0EsT0FBT0E7OztvQkFFZkEsT0FBT0E7OzZDQUcwQkEsU0FBcUJBO29CQUV0REEsVUFBcUJBLElBQUlBO29CQUN6QkEsbUJBQW1CQTtvQkFDbkJBLHlCQUF5QkE7d0JBRXJCQSxJQUFJQSxtQkFBa0JBOzRCQUFxQkE7O3dCQUMzQ0Esa0JBQXFCQTt3QkFDckJBLElBQUlBLGVBQWVBLFFBQVFBOzRCQUV2QkE7NEJBQ0FBOzt3QkFFSkEscUJBQStCQSw4Q0FBNkNBLGFBQWZBO3dCQUM3REEsY0FBY0E7d0JBQ2RBLGNBQXdCQTs7d0JBRXhCQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLG1DQUFxQkE7NEJBRTVFQSxjQUFjQSw2QkFBWUEsdUJBQWVBLElBQUlBOzRCQUM3Q0EsSUFBSUE7Z0NBQ0FBLFlBQVlBLHVCQUFlQTs7Z0NBRzNCQSxJQUFJQSxnQkFBUUEscUJBQXFCQTtvQ0FFN0JBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOzs7b0NBRWhEQSxnQkFBUUEsb0JBQW9CQSx1QkFBZUE7OztnQ0FFaERBLElBQUlBLGdCQUFRQSxpQkFBaUJBO29DQUV6QkEsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7O29DQUM3Q0EsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7OztvQ0FDMUNBLGdCQUFRQSxnQkFBZ0JBLHVCQUFlQTs7O2dDQUU5Q0EsSUFBSUEsZ0JBQVFBLGtCQUFrQkE7b0NBRTFCQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7b0NBQzlDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7O29DQUU3Q0EsZ0JBQVFBLGlCQUFpQkEsdUJBQWVBOzs7Z0NBRTdDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTtvQ0FBc0JBLGdCQUFRQSxzQkFBc0JBLHVCQUFlQTs7Ozs7d0JBSTVHQSw2QkFBWUE7O29CQUVoQkEsZ0JBQWdCQSxlQUFjQTtvQkFDOUJBOzt5Q0FHb0NBLFFBQWVBO29CQUVuREEsZUFBa0JBLFdBQVVBOztvQkFFNUJBLEtBQUtBLFdBQVdBLEtBQUtBLDhCQUEwQ0EsaUNBQW1CQTt3QkFFOUVBLElBQUlBLDRDQUFhQSxhQUFlQTs0QkFDNUJBLE9BQU9BLENBQUNBLHFCQUFhQTs7O29CQUU3QkEsT0FBT0E7O3VDQUdvQkE7b0JBRTNCQSxpQkFBaUJBO29CQUNqQkEsaUJBQWlCQTtvQkFDakJBO29CQUNBQTs7b0JBRUFBLFdBQTBCQSw0Q0FBNEJBIiwKICAic291cmNlc0NvbnRlbnQiOiBbInVzaW5nIEJyaWRnZTtcclxudXNpbmcgQnJpZGdlLkh0bWw1O1xyXG51c2luZyBCcmlkZ2UualF1ZXJ5MjtcclxudXNpbmcgTmV3dG9uc29mdC5Kc29uO1xyXG51c2luZyBTeXN0ZW07XHJcbnVzaW5nIFN5c3RlbS5Db2xsZWN0aW9ucztcclxudXNpbmcgU3lzdGVtLkxpbnE7XHJcbnVzaW5nIFN5c3RlbS5UZXh0O1xyXG51c2luZyBTeXN0ZW0uVGhyZWFkaW5nLlRhc2tzO1xyXG51c2luZyBTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYztcclxuXHJcbm5hbWVzcGFjZSBUaGVtZV9FZGl0b3Jcclxue1xyXG4gICAgcHVibGljIGNsYXNzIEFwcFxyXG4gICAge1xyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhciBqc29uQnRuID0gbmV3IEhUTUxCdXR0b25FbGVtZW50XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIElubmVySFRNTCA9IFwiVXBsb2FkIEpTT05cIixcclxuICAgICAgICAgICAgICAgIE9uQ2xpY2sgPSAoZXYpID0+XHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgVXBsb2FkTGF5b3V0QnRuKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH07XHJcblxyXG4gICAgICAgICAgIC8qIHZhciBidXR0b24gPSBuZXcgSFRNTEJ1dHRvbkVsZW1lbnRcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgSW5uZXJIVE1MID0gXCJVcGxvYWQgQ1NaXCIsXHJcbiAgICAgICAgICAgICAgICBPbkNsaWNrID0gKGV2KSA9PlxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIFVwbG9hZFNaU0J0bigpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9OyovXHJcbiAgICAgICAgICAgIFxyXG4gICAgICAgICAgICBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MRGl2RWxlbWVudD4oXCJ1cGxvYWRGaWxlXCIpLkFwcGVuZENoaWxkKGpzb25CdG4pO1xyXG4gICAgICAgICAgICAvL0RvY3VtZW50LkJvZHkuQXBwZW5kQ2hpbGQoYnV0dG9uKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBVcGxvYWRMYXlvdXRCdG4oKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgRG9jdW1lbnQuR2V0RWxlbWVudEJ5SWQ8SFRNTElucHV0RWxlbWVudD4oXCJKc29uVXBsb2FkZXJcIikuQ2xpY2soKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFVwbG9hZFNaU0J0bigpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MSW5wdXRFbGVtZW50PihcIlNaU1VwbG9hZGVyXCIpLkNsaWNrKCk7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBVcGxvYWRKU09OKFVpbnQ4QXJyYXkgYXJyLCBzdHJpbmcgZmlsZU5hbWUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBTeXN0ZW0uVGV4dC5FbmNvZGluZyBlbmMgPSBTeXN0ZW0uVGV4dC5FbmNvZGluZy5BU0NJSTtcclxuICAgICAgICAgICAgc3RyaW5nIGpzb24gPSBlbmMuR2V0U3RyaW5nKFN5c3RlbS5MaW5xLkVudW1lcmFibGUuVG9BcnJheTxieXRlPihhcnIpKTtcclxuXHJcbiAgICAgICAgICAgIHZhciBsYXlvdXQgPSBKc29uQ29udmVydC5EZXNlcmlhbGl6ZU9iamVjdDxMYXlvdXQ+KGpzb24pO1xyXG4gICAgICAgICAgICBMYXlvdXRGaWxlcyBSZHRCYXNlID0gZ2V0RmlsZUJ5TmFtZShsYXlvdXQsIFwiUmR0QmFzZVwiKTtcclxuICAgICAgICAgICAgaWYgKFJkdEJhc2UgIT0gbnVsbClcclxuICAgICAgICAgICAgICAgIFNldERlZmF1bHRzQnlGaWxlKFJkdEJhc2UsIFwiUmR0QmFzZVwiKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFVwbG9hZFNaUyhVaW50OEFycmF5IGFyciwgc3RyaW5nIGZpbGVOYW1lKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyBpbnQgUGF0Y2hFeGlzdHMoUGF0Y2hlcyBkZWZhdWx0UGF0Y2hlcywgTGlzdDxQYXRjaGVzPiBwYXRjaGVzKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgZm9yIChpbnQgaSA9IDA7IGkgPD0gU3lzdGVtLkxpbnEuRW51bWVyYWJsZS5Db3VudDxQYXRjaGVzPihwYXRjaGVzKSAtIDE7IGkrKylcclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgaWYgKGRlZmF1bHRQYXRjaGVzLlBhbmVOYW1lID09IHBhdGNoZXNbaV0uUGFuZU5hbWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIGk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgcmV0dXJuIC00MjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBTZXREZWZhdWx0c0J5RmlsZShMYXlvdXRGaWxlcyBSZHRCYXNlLCBzdHJpbmcgbmFtZSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIFhNTEh0dHBSZXF1ZXN0IHJlcSA9IG5ldyBYTUxIdHRwUmVxdWVzdCgpO1xyXG4gICAgICAgICAgICByZXEuUmVzcG9uc2VUeXBlID0gWE1MSHR0cFJlcXVlc3RSZXNwb25zZVR5cGUuU3RyaW5nO1xyXG4gICAgICAgICAgICByZXEuT25SZWFkeVN0YXRlQ2hhbmdlID0gKCkgPT5cclxuICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgaWYgKHJlcS5SZWFkeVN0YXRlICE9IEFqYXhSZWFkeVN0YXRlLkRvbmUpIHJldHVybjtcclxuICAgICAgICAgICAgICAgIHN0cmluZyBEb3dubG9hZFJlcyA9IHJlcS5SZXNwb25zZSBhcyBzdHJpbmc7XHJcbiAgICAgICAgICAgICAgICBpZiAoRG93bmxvYWRSZXMgPT0gbnVsbCB8fCBEb3dubG9hZFJlcy5MZW5ndGggPT0gMClcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICBDb25zb2xlLldyaXRlTGluZShcIkVycm9yIGdldHRpbmcgZGVmYXVsdCB2YWx1ZXNcIik7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgTGlzdDxQYXRjaGVzPiBkZWZhdWx0UGF0Y2hlcyA9IEpzb25Db252ZXJ0LkRlc2VyaWFsaXplT2JqZWN0PExpc3Q8UGF0Y2hlcz4+KERvd25sb2FkUmVzKTtcclxuICAgICAgICAgICAgICAgIERvd25sb2FkUmVzID0gbnVsbDtcclxuICAgICAgICAgICAgICAgIExpc3Q8UGF0Y2hlcz4gcGF0Y2hlcyA9IFJkdEJhc2UuUGF0Y2hlcztcclxuXHJcbiAgICAgICAgICAgICAgICBmb3IgKGludCBpID0gMDsgaSA8PSBTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLkNvdW50PFBhdGNoZXM+KGRlZmF1bHRQYXRjaGVzKSAtIDE7IGkrKylcclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICBpbnQgcGF0Y2hOYiA9IFBhdGNoRXhpc3RzKGRlZmF1bHRQYXRjaGVzW2ldLCBwYXRjaGVzKTtcclxuICAgICAgICAgICAgICAgICAgICBpZiAocGF0Y2hOYiA8IDApXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXMuQWRkKGRlZmF1bHRQYXRjaGVzW2ldKTtcclxuICAgICAgICAgICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbiAhPSBudWxsKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5YKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlggPSBkZWZhdWx0UGF0Y2hlc1tpXS5Qb3NpdGlvbi5YO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWSkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5ZID0gZGVmYXVsdFBhdGNoZXNbaV0uUG9zaXRpb24uWTtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlopKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWiA9IGRlZmF1bHRQYXRjaGVzW2ldLlBvc2l0aW9uLlo7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxzZSBwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uID0gZGVmYXVsdFBhdGNoZXNbaV0uUG9zaXRpb247XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAocGF0Y2hlc1twYXRjaE5iXS5TaXplICE9IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNpemUuWCkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5TaXplLlggPSBkZWZhdWx0UGF0Y2hlc1tpXS5TaXplLlg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5TaXplLlkpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uU2l6ZS5ZID0gZGVmYXVsdFBhdGNoZXNbaV0uU2l6ZS5ZO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9IGVsc2UgcGF0Y2hlc1twYXRjaE5iXS5TaXplID0gZGVmYXVsdFBhdGNoZXNbaV0uU2l6ZTtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaGVzW3BhdGNoTmJdLlNjYWxlICE9IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNjYWxlLlgpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUuWCA9IGRlZmF1bHRQYXRjaGVzW2ldLlNjYWxlLlg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5TY2FsZS5ZKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlNjYWxlLlkgPSBkZWZhdWx0UGF0Y2hlc1tpXS5TY2FsZS5ZO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGVsc2UgcGF0Y2hlc1twYXRjaE5iXS5TY2FsZSA9IGRlZmF1bHRQYXRjaGVzW2ldLlNjYWxlO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uUGFyZW50TmFtZSkpIHBhdGNoZXNbcGF0Y2hOYl0uUGFyZW50TmFtZSA9IGRlZmF1bHRQYXRjaGVzW2ldLlBhcmVudE5hbWU7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgXHJcbiAgICAgICAgICAgICAgICBTdGFydEVkaXRvcihwYXRjaGVzKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICAgICAgcmVxLk9wZW4oXCJHRVRcIiwgXCJkZWZhdWx0cy9cIiArIG5hbWUgKyBcIi5qc29uXCIsIHRydWUpOyBcclxuICAgICAgICAgICAgcmVxLlNlbmQoKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgTGF5b3V0RmlsZXMgZ2V0RmlsZUJ5TmFtZShMYXlvdXQgbGF5b3V0LCBzdHJpbmcgZmlsZU5hbWUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBzdHJpbmcgZnVsbE5hbWUgPSBcImJseXQvXCIgKyBmaWxlTmFtZSArIFwiLmJmbHl0XCI7XHJcblxyXG4gICAgICAgICAgICBmb3IgKHZhciBpID0gMDsgaSA8PSBTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLkNvdW50PExheW91dEZpbGVzPihsYXlvdXQuRmlsZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAobGF5b3V0LkZpbGVzW2ldLkZpbGVOYW1lID09IGZ1bGxOYW1lKVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAobGF5b3V0LkZpbGVzW2ldKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBTdGFydEVkaXRvcihMaXN0PFBhdGNoZXM+IHBhdGNoKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFyIGVkaXRvclZpZXcgPSBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MRGl2RWxlbWVudD4oXCJlZGl0b3Jfdmlld1wiKTtcclxuICAgICAgICAgICAgdmFyIHVwbG9hZEZpbGUgPSBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MRGl2RWxlbWVudD4oXCJ1cGxvYWRGaWxlXCIpO1xyXG4gICAgICAgICAgICBlZGl0b3JWaWV3LkhpZGRlbiA9IGZhbHNlO1xyXG4gICAgICAgICAgICB1cGxvYWRGaWxlLkhpZGRlbiA9IHRydWU7XHJcblxyXG4gICAgICAgICAgICBTY3JpcHQuQ2FsbChcImNoZWNrQ2xpY2tcIiwgSnNvbkNvbnZlcnQuU2VyaWFsaXplT2JqZWN0KHBhdGNoKSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgTGF5b3V0XHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIkZpbGVzXCIpXVxyXG4gICAgICAgIHB1YmxpYyBMaXN0PExheW91dEZpbGVzPiBGaWxlcyB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIExheW91dEZpbGVzXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIkZpbGVOYW1lXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgRmlsZU5hbWUgeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiUGF0Y2hlc1wiKV1cclxuICAgICAgICBwdWJsaWMgTGlzdDxQYXRjaGVzPiBQYXRjaGVzIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGF0Y2hlc1xyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJQYW5lTmFtZVwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFBhbmVOYW1lIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlBvc2l0aW9uXCIpXVxyXG4gICAgICAgIHB1YmxpYyBQYW5lUG9zaXRpb24gUG9zaXRpb24geyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiU2NhbGVcIildXHJcbiAgICAgICAgcHVibGljIFBhbmVTY2FsZSBTY2FsZSB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJTaXplXCIpXVxyXG4gICAgICAgIHB1YmxpYyBQYW5lU2l6ZSBTaXplIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlBhcmVudFwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFBhcmVudE5hbWUgeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiVmlzaWJsZVwiKV1cclxuICAgICAgICBwdWJsaWMgYm9vbCBWaXNpYmxlIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVBvc2l0aW9uXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlhcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBYIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIllcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBZIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlpcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBaIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVNjYWxlXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlhcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBYIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIllcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBZIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVNpemVcclxuICAgIHtcclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiWFwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFggeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiWVwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFkgeyBnZXQ7IHNldDsgfVxyXG4gICAgfVxyXG59Il0KfQo=
