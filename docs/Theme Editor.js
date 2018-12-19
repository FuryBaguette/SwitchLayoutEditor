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

            document.body.appendChild(jsonBtn);
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
                    editorView.hidden = false;

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

//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAiZmlsZSI6ICJUaGVtZSBFZGl0b3IuanMiLAogICJzb3VyY2VSb290IjogIiIsCiAgInNvdXJjZXMiOiBbIkFwcC5jcyJdLAogICJuYW1lcyI6IFsiIl0sCiAgIm1hcHBpbmdzIjogIjs7Ozs7Ozs7Ozs7WUFpQllBLGNBQWNBLG1GQUdBQSxVQUFDQTtnQkFFUEE7Ozs7Ozs7Ozs7OztZQWFSQSwwQkFBMEJBOzs7OztvQkFNMUJBO29CQUNBQTs7O29CQUtBQTtvQkFDQUE7O3NDQUcwQkEsS0FBZ0JBO29CQUUxQ0EsVUFBMkJBO29CQUMzQkEsV0FBY0EsY0FBY0EsNEJBQXFDQTs7b0JBRWpFQSxhQUFhQSw4Q0FBc0NBLE1BQVJBO29CQUMzQ0EsY0FBc0JBLCtCQUFjQTtvQkFDcENBLElBQUlBLFdBQVdBO3dCQUVYQSxtQ0FBa0JBOztvQkFHdEJBOztxQ0FHeUJBLEtBQWdCQTtvQkFFekNBOzt1Q0FHMEJBLGdCQUF3QkE7b0JBRWxEQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLDRCQUFjQTt3QkFFckVBLElBQUlBLGdEQUEyQkEsZ0JBQVFBOzRCQUNuQ0EsT0FBT0E7OztvQkFFZkEsT0FBT0E7OzZDQUcwQkEsU0FBcUJBO29CQUV0REEsVUFBcUJBLElBQUlBO29CQUN6QkEsbUJBQW1CQTtvQkFDbkJBLHlCQUF5QkE7d0JBRXJCQSxJQUFJQSxtQkFBa0JBOzRCQUFxQkE7O3dCQUMzQ0Esa0JBQXFCQTt3QkFDckJBLElBQUlBLGVBQWVBLFFBQVFBOzRCQUV2QkE7NEJBQ0FBOzt3QkFFSkEscUJBQStCQSw4Q0FBNkNBLGFBQWZBO3dCQUM3REEsY0FBY0E7d0JBQ2RBLGNBQXdCQTs7d0JBRXhCQSxLQUFLQSxXQUFXQSxLQUFLQSw4QkFBc0NBLG1DQUFxQkE7NEJBRTVFQSxjQUFjQSw2QkFBWUEsdUJBQWVBLElBQUlBOzRCQUM3Q0EsSUFBSUE7Z0NBQ0FBLFlBQVlBLHVCQUFlQTs7Z0NBRzNCQSxJQUFJQSxnQkFBUUEscUJBQXFCQTtvQ0FFN0JBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOztvQ0FDakRBLElBQUlBLDRCQUFxQkEsZ0JBQVFBO3dDQUM3QkEsZ0JBQVFBLHNCQUFzQkEsdUJBQWVBOzs7b0NBRWhEQSxnQkFBUUEsb0JBQW9CQSx1QkFBZUE7OztnQ0FFaERBLElBQUlBLGdCQUFRQSxpQkFBaUJBO29DQUV6QkEsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7O29DQUM3Q0EsSUFBSUEsNEJBQXFCQSxnQkFBUUE7d0NBQzdCQSxnQkFBUUEsa0JBQWtCQSx1QkFBZUE7OztvQ0FDMUNBLGdCQUFRQSxnQkFBZ0JBLHVCQUFlQTs7O2dDQUU5Q0EsSUFBSUEsZ0JBQVFBLGtCQUFrQkE7b0NBRTFCQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7b0NBQzlDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTt3Q0FDN0JBLGdCQUFRQSxtQkFBbUJBLHVCQUFlQTs7O29DQUU3Q0EsZ0JBQVFBLGlCQUFpQkEsdUJBQWVBOzs7Z0NBRTdDQSxJQUFJQSw0QkFBcUJBLGdCQUFRQTtvQ0FBc0JBLGdCQUFRQSxzQkFBc0JBLHVCQUFlQTs7Ozs7d0JBSTVHQSw2QkFBWUE7O29CQUVoQkEsZ0JBQWdCQSxlQUFjQTtvQkFDOUJBOzt5Q0FHb0NBLFFBQWVBO29CQUVuREEsZUFBa0JBLFdBQVVBOztvQkFFNUJBLEtBQUtBLFdBQVdBLEtBQUtBLDhCQUEwQ0EsaUNBQW1CQTt3QkFFOUVBLElBQUlBLDRDQUFhQSxhQUFlQTs0QkFDNUJBLE9BQU9BLENBQUNBLHFCQUFhQTs7O29CQUU3QkEsT0FBT0E7O3VDQUdvQkE7b0JBRTNCQSxpQkFBaUJBO29CQUNqQkE7O29CQUVBQSxXQUEwQkEsNENBQTRCQSIsCiAgInNvdXJjZXNDb250ZW50IjogWyJ1c2luZyBCcmlkZ2U7XHJcbnVzaW5nIEJyaWRnZS5IdG1sNTtcclxudXNpbmcgQnJpZGdlLmpRdWVyeTI7XHJcbnVzaW5nIE5ld3RvbnNvZnQuSnNvbjtcclxudXNpbmcgU3lzdGVtO1xyXG51c2luZyBTeXN0ZW0uQ29sbGVjdGlvbnM7XHJcbnVzaW5nIFN5c3RlbS5MaW5xO1xyXG51c2luZyBTeXN0ZW0uVGV4dDtcclxudXNpbmcgU3lzdGVtLlRocmVhZGluZy5UYXNrcztcclxudXNpbmcgU3lzdGVtLkNvbGxlY3Rpb25zLkdlbmVyaWM7XHJcblxyXG5uYW1lc3BhY2UgVGhlbWVfRWRpdG9yXHJcbntcclxuICAgIHB1YmxpYyBjbGFzcyBBcHBcclxuICAgIHtcclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgTWFpbigpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YXIganNvbkJ0biA9IG5ldyBIVE1MQnV0dG9uRWxlbWVudFxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBJbm5lckhUTUwgPSBcIlVwbG9hZCBKU09OXCIsXHJcbiAgICAgICAgICAgICAgICBPbkNsaWNrID0gKGV2KSA9PlxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIFVwbG9hZExheW91dEJ0bigpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9O1xyXG5cclxuICAgICAgICAgICAvKiB2YXIgYnV0dG9uID0gbmV3IEhUTUxCdXR0b25FbGVtZW50XHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIElubmVySFRNTCA9IFwiVXBsb2FkIENTWlwiLFxyXG4gICAgICAgICAgICAgICAgT25DbGljayA9IChldikgPT5cclxuICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICBVcGxvYWRTWlNCdG4oKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfTsqL1xyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgRG9jdW1lbnQuQm9keS5BcHBlbmRDaGlsZChqc29uQnRuKTtcclxuICAgICAgICAgICAgLy9Eb2N1bWVudC5Cb2R5LkFwcGVuZENoaWxkKGJ1dHRvbik7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgVXBsb2FkTGF5b3V0QnRuKClcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIERvY3VtZW50LkdldEVsZW1lbnRCeUlkPEhUTUxJbnB1dEVsZW1lbnQ+KFwiSnNvblVwbG9hZGVyXCIpLkNsaWNrKCk7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHB1YmxpYyBzdGF0aWMgdm9pZCBVcGxvYWRTWlNCdG4oKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgRG9jdW1lbnQuR2V0RWxlbWVudEJ5SWQ8SFRNTElucHV0RWxlbWVudD4oXCJTWlNVcGxvYWRlclwiKS5DbGljaygpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgVXBsb2FkSlNPTihVaW50OEFycmF5IGFyciwgc3RyaW5nIGZpbGVOYW1lKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgU3lzdGVtLlRleHQuRW5jb2RpbmcgZW5jID0gU3lzdGVtLlRleHQuRW5jb2RpbmcuQVNDSUk7XHJcbiAgICAgICAgICAgIHN0cmluZyBqc29uID0gZW5jLkdldFN0cmluZyhTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLlRvQXJyYXk8Ynl0ZT4oYXJyKSk7XHJcblxyXG4gICAgICAgICAgICB2YXIgbGF5b3V0ID0gSnNvbkNvbnZlcnQuRGVzZXJpYWxpemVPYmplY3Q8TGF5b3V0Pihqc29uKTtcclxuICAgICAgICAgICAgTGF5b3V0RmlsZXMgUmR0QmFzZSA9IGdldEZpbGVCeU5hbWUobGF5b3V0LCBcIlJkdEJhc2VcIik7XHJcbiAgICAgICAgICAgIGlmIChSZHRCYXNlICE9IG51bGwpXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIFNldERlZmF1bHRzQnlGaWxlKFJkdEJhc2UsIFwiUmR0QmFzZVwiKTtcclxuICAgICAgICAgICAgICAgIC8vU3RhcnRFZGl0b3IobGF5b3V0KTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIHZvaWQgVXBsb2FkU1pTKFVpbnQ4QXJyYXkgYXJyLCBzdHJpbmcgZmlsZU5hbWUpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBwdWJsaWMgc3RhdGljIGludCBQYXRjaEV4aXN0cyhQYXRjaGVzIGRlZmF1bHRQYXRjaGVzLCBMaXN0PFBhdGNoZXM+IHBhdGNoZXMpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICBmb3IgKGludCBpID0gMDsgaSA8PSBTeXN0ZW0uTGlucS5FbnVtZXJhYmxlLkNvdW50PFBhdGNoZXM+KHBhdGNoZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZGVmYXVsdFBhdGNoZXMuUGFuZU5hbWUgPT0gcGF0Y2hlc1tpXS5QYW5lTmFtZSlcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gaTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICByZXR1cm4gLTQyO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFNldERlZmF1bHRzQnlGaWxlKExheW91dEZpbGVzIFJkdEJhc2UsIHN0cmluZyBuYW1lKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgWE1MSHR0cFJlcXVlc3QgcmVxID0gbmV3IFhNTEh0dHBSZXF1ZXN0KCk7XHJcbiAgICAgICAgICAgIHJlcS5SZXNwb25zZVR5cGUgPSBYTUxIdHRwUmVxdWVzdFJlc3BvbnNlVHlwZS5TdHJpbmc7XHJcbiAgICAgICAgICAgIHJlcS5PblJlYWR5U3RhdGVDaGFuZ2UgPSAoKSA9PlxyXG4gICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICBpZiAocmVxLlJlYWR5U3RhdGUgIT0gQWpheFJlYWR5U3RhdGUuRG9uZSkgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgc3RyaW5nIERvd25sb2FkUmVzID0gcmVxLlJlc3BvbnNlIGFzIHN0cmluZztcclxuICAgICAgICAgICAgICAgIGlmIChEb3dubG9hZFJlcyA9PSBudWxsIHx8IERvd25sb2FkUmVzLkxlbmd0aCA9PSAwKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIENvbnNvbGUuV3JpdGVMaW5lKFwiRXJyb3IgZ2V0dGluZyBkZWZhdWx0IHZhbHVlc1wiKTtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBMaXN0PFBhdGNoZXM+IGRlZmF1bHRQYXRjaGVzID0gSnNvbkNvbnZlcnQuRGVzZXJpYWxpemVPYmplY3Q8TGlzdDxQYXRjaGVzPj4oRG93bmxvYWRSZXMpO1xyXG4gICAgICAgICAgICAgICAgRG93bmxvYWRSZXMgPSBudWxsO1xyXG4gICAgICAgICAgICAgICAgTGlzdDxQYXRjaGVzPiBwYXRjaGVzID0gUmR0QmFzZS5QYXRjaGVzO1xyXG5cclxuICAgICAgICAgICAgICAgIGZvciAoaW50IGkgPSAwOyBpIDw9IFN5c3RlbS5MaW5xLkVudW1lcmFibGUuQ291bnQ8UGF0Y2hlcz4oZGVmYXVsdFBhdGNoZXMpIC0gMTsgaSsrKVxyXG4gICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgIGludCBwYXRjaE5iID0gUGF0Y2hFeGlzdHMoZGVmYXVsdFBhdGNoZXNbaV0sIHBhdGNoZXMpO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaE5iIDwgMClcclxuICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlcy5BZGQoZGVmYXVsdFBhdGNoZXNbaV0pO1xyXG4gICAgICAgICAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgICAgICB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uICE9IG51bGwpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlgpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWCA9IGRlZmF1bHRQYXRjaGVzW2ldLlBvc2l0aW9uLlg7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5ZKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlBvc2l0aW9uLlkgPSBkZWZhdWx0UGF0Y2hlc1tpXS5Qb3NpdGlvbi5ZO1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24uWikpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5Qb3NpdGlvbi5aID0gZGVmYXVsdFBhdGNoZXNbaV0uUG9zaXRpb24uWjtcclxuICAgICAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgICAgICBlbHNlIHBhdGNoZXNbcGF0Y2hOYl0uUG9zaXRpb24gPSBkZWZhdWx0UGF0Y2hlc1tpXS5Qb3NpdGlvbjtcclxuXHJcbiAgICAgICAgICAgICAgICAgICAgICAgIGlmIChwYXRjaGVzW3BhdGNoTmJdLlNpemUgIT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uU2l6ZS5YKSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICBwYXRjaGVzW3BhdGNoTmJdLlNpemUuWCA9IGRlZmF1bHRQYXRjaGVzW2ldLlNpemUuWDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNpemUuWSkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5TaXplLlkgPSBkZWZhdWx0UGF0Y2hlc1tpXS5TaXplLlk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH0gZWxzZSBwYXRjaGVzW3BhdGNoTmJdLlNpemUgPSBkZWZhdWx0UGF0Y2hlc1tpXS5TaXplO1xyXG5cclxuICAgICAgICAgICAgICAgICAgICAgICAgaWYgKHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUgIT0gbnVsbClcclxuICAgICAgICAgICAgICAgICAgICAgICAge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgaWYgKFN0cmluZy5Jc051bGxPckVtcHR5KHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUuWCkpXHJcbiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgcGF0Y2hlc1twYXRjaE5iXS5TY2FsZS5YID0gZGVmYXVsdFBhdGNoZXNbaV0uU2NhbGUuWDtcclxuICAgICAgICAgICAgICAgICAgICAgICAgICAgIGlmIChTdHJpbmcuSXNOdWxsT3JFbXB0eShwYXRjaGVzW3BhdGNoTmJdLlNjYWxlLlkpKVxyXG4gICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIHBhdGNoZXNbcGF0Y2hOYl0uU2NhbGUuWSA9IGRlZmF1bHRQYXRjaGVzW2ldLlNjYWxlLlk7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICAgICAgZWxzZSBwYXRjaGVzW3BhdGNoTmJdLlNjYWxlID0gZGVmYXVsdFBhdGNoZXNbaV0uU2NhbGU7XHJcblxyXG4gICAgICAgICAgICAgICAgICAgICAgICBpZiAoU3RyaW5nLklzTnVsbE9yRW1wdHkocGF0Y2hlc1twYXRjaE5iXS5QYXJlbnROYW1lKSkgcGF0Y2hlc1twYXRjaE5iXS5QYXJlbnROYW1lID0gZGVmYXVsdFBhdGNoZXNbaV0uUGFyZW50TmFtZTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICBcclxuICAgICAgICAgICAgICAgIFN0YXJ0RWRpdG9yKHBhdGNoZXMpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgICAgICByZXEuT3BlbihcIkdFVFwiLCBcImRlZmF1bHRzL1wiICsgbmFtZSArIFwiLmpzb25cIiwgdHJ1ZSk7IFxyXG4gICAgICAgICAgICByZXEuU2VuZCgpO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyBMYXlvdXRGaWxlcyBnZXRGaWxlQnlOYW1lKExheW91dCBsYXlvdXQsIHN0cmluZyBmaWxlTmFtZSlcclxuICAgICAgICB7XHJcbiAgICAgICAgICAgIHN0cmluZyBmdWxsTmFtZSA9IFwiYmx5dC9cIiArIGZpbGVOYW1lICsgXCIuYmZseXRcIjtcclxuXHJcbiAgICAgICAgICAgIGZvciAodmFyIGkgPSAwOyBpIDw9IFN5c3RlbS5MaW5xLkVudW1lcmFibGUuQ291bnQ8TGF5b3V0RmlsZXM+KGxheW91dC5GaWxlcykgLSAxOyBpKyspXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIGlmIChsYXlvdXQuRmlsZXNbaV0uRmlsZU5hbWUgPT0gZnVsbE5hbWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIChsYXlvdXQuRmlsZXNbaV0pO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIHJldHVybiBudWxsO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgcHVibGljIHN0YXRpYyB2b2lkIFN0YXJ0RWRpdG9yKExpc3Q8UGF0Y2hlcz4gcGF0Y2gpXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgICB2YXIgZWRpdG9yVmlldyA9IERvY3VtZW50LkdldEVsZW1lbnRCeUlkPEhUTUxEaXZFbGVtZW50PihcImVkaXRvcl92aWV3XCIpO1xyXG4gICAgICAgICAgICBlZGl0b3JWaWV3LkhpZGRlbiA9IGZhbHNlO1xyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgU2NyaXB0LkNhbGwoXCJjaGVja0NsaWNrXCIsIEpzb25Db252ZXJ0LlNlcmlhbGl6ZU9iamVjdChwYXRjaCkpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgTGF5b3V0XHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIkZpbGVzXCIpXVxyXG4gICAgICAgIHB1YmxpYyBMaXN0PExheW91dEZpbGVzPiBGaWxlcyB7IGdldDsgc2V0OyB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGNsYXNzIExheW91dEZpbGVzXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIkZpbGVOYW1lXCIpXVxyXG4gICAgICAgIHB1YmxpYyBzdHJpbmcgRmlsZU5hbWUgeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiUGF0Y2hlc1wiKV1cclxuICAgICAgICBwdWJsaWMgTGlzdDxQYXRjaGVzPiBQYXRjaGVzIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGF0Y2hlc1xyXG4gICAge1xyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJQYW5lTmFtZVwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFBhbmVOYW1lIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlBvc2l0aW9uXCIpXVxyXG4gICAgICAgIHB1YmxpYyBQYW5lUG9zaXRpb24gUG9zaXRpb24geyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiU2NhbGVcIildXHJcbiAgICAgICAgcHVibGljIFBhbmVTY2FsZSBTY2FsZSB7IGdldDsgc2V0OyB9XHJcblxyXG4gICAgICAgIFtKc29uUHJvcGVydHkoXCJTaXplXCIpXVxyXG4gICAgICAgIHB1YmxpYyBQYW5lU2l6ZSBTaXplIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlBhcmVudFwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFBhcmVudE5hbWUgeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiVmlzaWJsZVwiKV1cclxuICAgICAgICBwdWJsaWMgYm9vbCBWaXNpYmxlIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVBvc2l0aW9uXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlhcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBYIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIllcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBZIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlpcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBaIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVNjYWxlXHJcbiAgICB7XHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIlhcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBYIHsgZ2V0OyBzZXQ7IH1cclxuXHJcbiAgICAgICAgW0pzb25Qcm9wZXJ0eShcIllcIildXHJcbiAgICAgICAgcHVibGljIHN0cmluZyBZIHsgZ2V0OyBzZXQ7IH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgY2xhc3MgUGFuZVNpemVcclxuICAgIHtcclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiWFwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFggeyBnZXQ7IHNldDsgfVxyXG5cclxuICAgICAgICBbSnNvblByb3BlcnR5KFwiWVwiKV1cclxuICAgICAgICBwdWJsaWMgc3RyaW5nIFkgeyBnZXQ7IHNldDsgfVxyXG4gICAgfVxyXG59Il0KfQo=
