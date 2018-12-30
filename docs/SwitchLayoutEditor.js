/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2018
 * @compiler Bridge.NET 17.6.0
 */
Bridge.assembly("SwitchLayoutEditor", function ($asm, globals) {
    "use strict";

    Bridge.define("SwitchLayoutEditor.App", {
        main: function Main () {
            var $t;
            var jsonBtn = ($t = document.createElement("button"), $t.innerHTML = "Upload JSON", $t.onclick = function (ev) {
                SwitchLayoutEditor.App.UploadLayoutBtn();
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

                    var layout = Newtonsoft.Json.JsonConvert.DeserializeObject(json, SwitchLayoutEditor.Layout);
                    var RdtBase = SwitchLayoutEditor.App.getFileByName(layout, "RdtBase");
                    if (RdtBase != null) {
                        SwitchLayoutEditor.App.SetDefaultsByFile(RdtBase, "RdtBase");
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
                        var defaultPatches = Newtonsoft.Json.JsonConvert.DeserializeObject(DownloadRes, System.Collections.Generic.List$1(SwitchLayoutEditor.Patches));
                        DownloadRes = null;
                        var patches = RdtBase.Patches;

                        for (var i = 0; i <= ((System.Linq.Enumerable.from(defaultPatches).count() - 1) | 0); i = (i + 1) | 0) {
                            var patchNb = SwitchLayoutEditor.App.PatchExists(defaultPatches.getItem(i), patches);
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

                        SwitchLayoutEditor.App.StartEditor(patches);
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

    Bridge.define("SwitchLayoutEditor.Layout", {
        fields: {
            Files: null
        }
    });

    Bridge.define("SwitchLayoutEditor.LayoutFiles", {
        fields: {
            FileName: null,
            Patches: null
        }
    });

    Bridge.define("SwitchLayoutEditor.PanePosition", {
        fields: {
            X: null,
            Y: null,
            Z: null
        }
    });

    Bridge.define("SwitchLayoutEditor.PaneScale", {
        fields: {
            X: null,
            Y: null
        }
    });

    Bridge.define("SwitchLayoutEditor.PaneSize", {
        fields: {
            X: null,
            Y: null
        }
    });

    Bridge.define("SwitchLayoutEditor.Patches", {
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

//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAiZmlsZSI6ICJTd2l0Y2hMYXlvdXRFZGl0b3IuanMiLAogICJzb3VyY2VSb290IjogIiIsCiAgInNvdXJjZXMiOiBbIkFwcC5jcyJdLAogICJuYW1lcyI6IFsiIl0sCiAgIm1hcHBpbmdzIjogIjs7Ozs7Ozs7Ozs7WUFpQllBLGNBQWNBLG1GQUdBQSxVQUFDQTtnQkFFUEE7Ozs7Ozs7Ozs7OztZQWFSQSxrREFBa0VBOzs7OztvQkFNbEVBO29CQUNBQTs7O29CQUtBQTtvQkFDQUE7O3NDQUcwQkEsS0FBZ0JBO29CQUUxQ0EsVUFBMkJBO29CQUMzQkEsV0FBY0EsY0FBY0EsNEJBQXFDQTs7b0JBRWpFQSxhQUFhQSw4Q0FBc0NBLE1BQVJBO29CQUMzQ0EsY0FBc0JBLHFDQUFjQTtvQkFDcENBLElBQUlBLFdBQVdBO3dCQUNYQSx5Q0FBa0JBOztvQkFDdEJBOztxQ0FHeUJBLEtBQWdCQTtvQkFFekNBOzt1Q0FHMEJBLGdCQUF3QkE7b0JBRWxEQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLDRCQUFjQTt3QkFFckVBLElBQUlBLGdEQUEyQkEsZ0JBQVFBOzRCQUNuQ0EsT0FBT0E7OztvQkFFZkEsT0FBT0E7OzZDQUcwQkEsU0FBcUJBO29CQUV0REEsVUFBcUJBLElBQUlBO29CQUN6QkEsbUJBQW1CQTtvQkFDbkJBLHlCQUF5QkE7d0JBRXJCQSxJQUFJQSxtQkFBa0JBOzRCQUFxQkE7O3dCQUMzQ0Esa0JBQXFCQTt3QkFDckJBLElBQUlBLGVBQWVBLFFBQVFBOzRCQUV2QkE7NEJBQ0FBOzt3QkFFSkEscUJBQStCQSw4Q0FBNkNBLGFBQWZBO3dCQUM3REEsY0FBY0E7d0JBQ2RBLGNBQXdCQTs7d0JBRXhCQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLG1DQUFxQkE7NEJBRTVFQSxjQUFjQSxtQ0FBWUEsdUJBQWVBLElBQUlBOzRCQUM3Q0EsSUFBSUE7Z0NBQ0FBLFlBQVlBLHVCQUFlQTs7Z0NBRzNCQSxJQUFJQSxnQkFBUUEscUJBQXFCQTtvQ0FFN0JBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOzs7b0NBRWhEQSxnQkFBUUEsb0JBQW9CQSx1QkFBZUE7OztnQ0FFaERBLElBQUlBLGdCQUFRQSxpQkFBaUJBO29DQUV6QkEsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7O29DQUM3Q0EsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7OztvQ0FFNUNBLGdCQUFRQSxnQkFBZ0JBLHVCQUFlQTs7O2dDQUU1Q0EsSUFBSUEsZ0JBQVFBLGtCQUFrQkE7b0NBRTFCQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7b0NBQzlDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7O29DQUU3Q0EsZ0JBQVFBLGlCQUFpQkEsdUJBQWVBOzs7Z0NBRTdDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTtvQ0FBc0JBLGdCQUFRQSxzQkFBc0JBLHVCQUFlQTs7Ozs7d0JBSTVHQSxtQ0FBWUE7O29CQUVoQkEsZ0JBQWdCQSxlQUFjQTtvQkFDOUJBOzt5Q0FHb0NBLFFBQWVBO29CQUVuREEsZUFBa0JBLFdBQVVBOztvQkFFNUJBLEtBQUtBLFdBQVdBLEtBQUtBLDhCQUEwQ0EsaUNBQW1CQTt3QkFFOUVBLElBQUlBLDRDQUFhQSxhQUFlQTs0QkFDNUJBLE9BQU9BLENBQUNBLHFCQUFhQTs7O29CQUU3QkEsT0FBT0E7O3VDQUdvQkE7b0JBRTNCQSxpQkFBaUJBO29CQUNqQkEsaUJBQWlCQTtvQkFDakJBO29CQUNBQTs7b0JBRUFBLFdBQTBCQSw0Q0FBNEJBIiwKICAic291cmNlc0NvbnRlbnQiOiBbInVzaW5nIEJyaWRnZTtcclxudXNpbmcgQnJpZGdlLkh0bWw1O1xyXG51c2luZyBCcmlkZ2UualF1ZXJ5MjtcclxudXNpbmcgTmV3dG9uc29mdC5Kc29uO1xyXG51c2luZyBTeXN0ZW07XHJcbnVzaW5nIFN5c3RlbS5Db2xsZWN0aW9ucztcclxudXNpbmcgU3lzdGVtLkxpbnE7XHJcbnVzaW5nIFN5c3RlbS5UZXh0O1xyXG51c2luZyBTeXN0ZW0uVGhyZWFkaW5nLlRhc2tzO1xyXG51c2luZyBTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYztcclxuXHJcbm5hbWVzcGFjZSBTd2l0Y2hMYXlvdXRFZGl0b3Jcclxue1xyXG4gICAgcHVibGljIGNsYXNzIEFwcFxyXG4gICAge1xyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBNYWluKClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHZhciBqc29uQnRuID0gbmV3IEhUTUxCdXR0b25FbGVtZW50XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIElubmVySFRNTCA9IFwiVXBsb2FkIEpTT05cIixcclxuICAgICAgICAgICAgICAgIE9uQ2xpY2sgPSAoZXYpID0+XHJcbiAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgVXBsb2FkTGF5b3V0QnRuKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH07XHJcblxyXG4gICAgICAgICAgICAvKiB2YXIgYnV0dG9uID0gbmV3IEhUTUxCdXR0b25FbGVtZW50XHJcbiAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgSW5uZXJIVE1MID0gXCJVcGxvYWQgQ1NaXCIsXHJcbiAgICAgICAgICAgICAgICAgT25DbGljayA9IChldikgPT5cclxuICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgIFVwbG9hZFNaU0J0bigpO1xyXG4gICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgIH07Ki9cclxuXHJcbiAgICAgICAgICAgIERvY3VtZW50LkdldEVsZW1lbnRCeUlkPEhUTUxEaXZFbGVtZW50PihcInVwbG9hZEZpbGVcIikuQXBwZW5kQ2hpbGQoanNvbkJ0bik7XHJcbiAgICAgICAgICAgIC8vRG9jdW1lbnQuQm9keS5BcHBlbmRDaGlsZChidXR0b24pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFVwbG9hZExheW91dEJ0bigpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MSW5wdXRFbGVtZW50PihcIkpzb25VcGxvYWRlclwiKS5DbGljaygpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgVXBsb2FkU1pTQnRuKClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIERvY3VtZW50LkdldEVsZW1lbnRCeUlkPEhUTUxJbnB1dEVsZW1lbnQ+KFwiU1pTVXBsb2FkZXJcIikuQ2xpY2soKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFVwbG9hZEpTT04oVWludDhBcnJheSBhcnIsIHN0cmluZyBmaWxlTmFtZSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIFN5c3RlbS5UZXh0LkVuY29kaW5nIGVuYyA9IFN5c3RlbS5UZXh0LkVuY29kaW5nLkFTQ0lJO1xyXG4gICAgICAgICAgICBzdHJpbmcganNvbiA9IGVuYy5HZXRTdHJpbmcoU3lzdGVtLkxpbnEuRW51bWVyYWJsZS5Ub0FycmF5PGJ5dGU+KGFycikpO1xyXG5cclxuICAgICAgICAgICAgdmFyIGxheW91dCA9IEpzb25Db252ZXJ0LkRlc2VyaWFsaXplT2JqZWN0PExheW91dD4oanNvbik7XHJcbiAgICAgICAgICAgIExheW91dEZpbGVzIFJkdEJhc2UgPSBnZXRGaWxlQnlOYW1lKGxheW91dCwgXCJSZHRCYXNlXCIpO1xyXG4gICAgICAgICAgICBpZiAoUmR0QmFzZSAhPSBudWxsKVxyXG4gICAgICAgICAgICAgICAgU2V0RGVmYXVsdHNCeUZpbGUoUmR0QmFzZSwgXCJSZHRCYXNlXCIpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgVXBsb2FkU1pTKFVpbnQ4QXJyYXkgYXJyLCBzdHJpbmcgZmlsZU5hbWUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIGludCBQYXRjaEV4aXN0cyhQYXRjaGVzIGRlZmF1bHRQYXRjaGVzLCBMaXN0PFBhdGNoZXM+IHBhdGNoZXMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBmb3IgKGludCBpID0gMDsgaSA8PSBTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLkNvdW50PFBhdGNoZXM+KHBhdGNoZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZGVmYXVsdFBhdGNoZXMuUGFuZU5hbWUgPT0gcGF0Y2hlc1tpXS5QYW5lTmFtZSlcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gaTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICByZXR1cm4gLTQyO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFNldERlZmF1bHRzQnlGaWxlKExheW91dEZpbGVzIFJkdEJhc2UsIHN0cmluZyBuYW1lKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgWE1MSHR0cFJlcXVlc3QgcmVxID0gbmV3IFhNTEh0dHBSZXF1ZXN0KCk7XHJcbiAgICAgICAgICAgIHJlcS5SZXNwb25zZVR5cGUgPSBYTUxIdHRwUmVxdWVzdFJlc3BvbnNlVHlwZS5TdHJpbmc7XHJcbiAgICAgICAgICAgIHJlcS5PblJlYWR5U3RhdGVDaGFuZ2UgPSAoKSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAocmVxLlJlYWR5U3RhdGUgIT0gQWpheFJlYWR5U3RhdGUuRG9uZSkgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgc3RyaW5nIERvd25sb2FkUmVzID0gcmVxLlJlc3BvbnNlIGFzIHN0cmluZztcclxuICAgICAgICAgICAgICAgIGlmIChEb3dubG9hZFJlcyA9PSBudWxsIHx8IERvd25sb2FkUmVzLkxlbmd0aCA9PSAwKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIENvbnNvbGUuV3JpdGVMaW5lKFwiRXJyb3IgZ2V0dGluZyBkZWZhdWx0IHZhbHVlc1wiKTtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBMaXN0PFBhdGNoZXM+IGRlZmF1bHRQYXRjaGVzID0gSnNvbkNvbnZlcnQuRGVzZXJpYWxpemVPYmplY3Q8TGlzdDxQYXRjaGVzPj4oRG93bmxvYWRSZXMpO1xyXG4gICAgICAgICAgICAgICAgRG93bmxvYWRSZXMgPSBudWxsO1xyXG4gICAgICAgICAgICAgICAgTGlzdDxQYXRjaGVzPiBwYXRjaGVzID0gUmR0QmFzZS5QYXRjaGVzO1xyXG5cclxuICAgICAgICAgICAgICAgIGZvciAoaW50IGkgPSAwOyBpIDw9IFN5c3RlbS5MaW5xLkVudW1lcmFibGUuQ291bnQ8UGF0Y2hlcz4oZGVmYXVsdFBhdGNoZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIGludCBwYXRjaE5iID0gUGF0Y2hFeGlzdHMoZGVmYXVsdFBhdGNoZXNbaV0sIHBhdGNoZXMpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaE5iIDwgMClcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlcy5BZGQoZGVmYXVsdFBhdGNoZXNbaV0pO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uICE9IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlgpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWCA9IGRlZmF1bHRQYXRjaGVzW2ldLlBvc2l0aW9uLlg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5ZKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlkgPSBkZWZhdWx0UGF0Y2hlc1tpXS5Qb3NpdGlvbi5ZO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWikpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5aID0gZGVmYXVsdFBhdGNoZXNbaV0uUG9zaXRpb24uWjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24gPSBkZWZhdWx0UGF0Y2hlc1tpXS5Qb3NpdGlvbjtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaGVzW3BhdGNoTmJdLlNpemUgIT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uU2l6ZS5YKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlNpemUuWCA9IGRlZmF1bHRQYXRjaGVzW2ldLlNpemUuWDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNpemUuWSkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5TaXplLlkgPSBkZWZhdWx0UGF0Y2hlc1tpXS5TaXplLlk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxzZSBwYXRjaGVzW3BhdGNoTmJdLlNpemUgPSBkZWZhdWx0UGF0Y2hlc1tpXS5TaXplO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUgIT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUuWCkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5TY2FsZS5YID0gZGVmYXVsdFBhdGNoZXNbaV0uU2NhbGUuWDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNjYWxlLlkpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUuWSA9IGRlZmF1bHRQYXRjaGVzW2ldLlNjYWxlLlk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxzZSBwYXRjaGVzW3BhdGNoTmJdLlNjYWxlID0gZGVmYXVsdFBhdGNoZXNbaV0uU2NhbGU7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5QYXJlbnROYW1lKSkgcGF0Y2hlc1twYXRjaE5iXS5QYXJlbnROYW1lID0gZGVmYXVsdFBhdGNoZXNbaV0uUGFyZW50TmFtZTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgU3RhcnRFZGl0b3IocGF0Y2hlcyk7XHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgICAgIHJlcS5PcGVuKFwiR0VUXCIsIFwiZGVmYXVsdHMvXCIgKyBuYW1lICsgXCIuanNvblwiLCB0cnVlKTtcclxuICAgICAgICAgICAgcmVxLlNlbmQoKTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgTGF5b3V0RmlsZXMgZ2V0RmlsZUJ5TmFtZShMYXlvdXQgbGF5b3V0LCBzdHJpbmcgZmlsZU5hbWUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBzdHJpbmcgZnVsbE5hbWUgPSBcImJseXQvXCIgKyBmaWxlTmFtZSArIFwiLmJmbHl0XCI7XHJcblxyXG4gICAgICAgICAgICBmb3IgKHZhciBpID0gMDsgaSA8PSBTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLkNvdW50PExheW91dEZpbGVzPihsYXlvdXQuRmlsZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAobGF5b3V0LkZpbGVzW2ldLkZpbGVOYW1lID09IGZ1bGxOYW1lKVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAobGF5b3V0LkZpbGVzW2ldKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBTdGFydEVkaXRvcihMaXN0PFBhdGNoZXM+IHBhdGNoKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdmFyIGVkaXRvclZpZXcgPSBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MRGl2RWxlbWVudD4oXCJlZGl0b3Jfdmlld1wiKTtcclxuICAgICAgICAgICAgdmFyIHVwbG9hZEZpbGUgPSBEb2N1bWVudC5HZXRFbGVtZW50QnlJZDxIVE1MRGl2RWxlbWVudD4oXCJ1cGxvYWRGaWxlXCIpO1xyXG4gICAgICAgICAgICBlZGl0b3JWaWV3LkhpZGRlbiA9IGZhbHNlO1xyXG4gICAgICAgICAgICB1cGxvYWRGaWxlLkhpZGRlbiA9IHRydWU7XHJcblxyXG4gICAgICAgICAgICBTY3JpcHQuQ2FsbChcImNoZWNrQ2xpY2tcIiwgSnNvbkNvbnZlcnQuU2VyaWFsaXplT2JqZWN0KHBhdGNoKSk7XHJcbiAgICAgICAgfVxyXG5cclxuXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIExheW91dFxyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJGaWxlc1wiKV1cclxuICAgICAgICBwdWJsaWMgTGlzdDxMYXlvdXRGaWxlcz4gRmlsZXMgeyBnZXQ7IHNldDsgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBjbGFzcyBMYXlvdXRGaWxlc1xyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJGaWxlTmFtZVwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIEZpbGVOYW1lIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlBhdGNoZXNcIildXHJcbiAgICAgICAgcHVibGljIExpc3Q8UGF0Y2hlcz4gUGF0Y2hlcyB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIFBhdGNoZXNcclxuICAgIHtcclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiUGFuZU5hbWVcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBQYW5lTmFtZSB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJQb3NpdGlvblwiKV1cclxuICAgICAgICBwdWJsaWMgUGFuZVBvc2l0aW9uIFBvc2l0aW9uIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlNjYWxlXCIpXVxyXG4gICAgICAgIHB1YmxpYyBQYW5lU2NhbGUgU2NhbGUgeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiU2l6ZVwiKV1cclxuICAgICAgICBwdWJsaWMgUGFuZVNpemUgU2l6ZSB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJQYXJlbnRcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBQYXJlbnROYW1lIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlZpc2libGVcIildXHJcbiAgICAgICAgcHVibGljIGJvb2wgVmlzaWJsZSB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIFBhbmVQb3NpdGlvblxyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJYXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgWCB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJZXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgWSB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJaXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgWiB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIFBhbmVTY2FsZVxyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJYXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgWCB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJZXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgWSB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIFBhbmVTaXplXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlhcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBYIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIllcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBZIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxufSJdCn0K
