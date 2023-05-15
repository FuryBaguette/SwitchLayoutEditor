using Newtonsoft.Json;
using SwitchThemes.Common.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SwitchThemes.Common.Bflan.Pai1Section;

namespace BflytPreview.Automation
{
    public enum BflanTemplateKind
    {
        Pai1, // then FileName Contains PaiEntry[] 
        PaiEntry, // ... contains PaiTag[]
        PaiTag, // ... contains PaiTagEntry[]
        PaiTagEntry, // ... contains Keyframe[]
    }

    public enum ReplaceValueKind 
    {
        String,
        Int,
        Float
    }

    public class ValueReplacement 
    {
        public ReplaceValueKind Kind;
        public string Keyword;
        public string Name;
        public string Default;
    }

    public class BflanTemplate
    {
        public string Name;
        public string FileName;
        public BflanTemplateKind Target;
        public List<ValueReplacement> Parameters;

        public object[] DeserializeContent(string json) 
        {
            switch (Target)
            {
                case BflanTemplateKind.Pai1:
                    return JsonConvert.DeserializeObject<PaiEntrySerializer[]>(json).Select(x => x.Deserialize()).ToArray();
                case BflanTemplateKind.PaiEntry:
                    return JsonConvert.DeserializeObject<PaiTagSerializer[]>(json).Select(x => x.Deserialize()).ToArray();
                case BflanTemplateKind.PaiTag:
                    return JsonConvert.DeserializeObject<PaiTagEntrySerializer[]>(json).Select(x => x.Deserialize()).ToArray();
                case BflanTemplateKind.PaiTagEntry:
                    return JsonConvert.DeserializeObject<KeyFrameSerializer[]>(json).Select(x => x.Deserialize()).ToArray();
                default:
                    throw new Exception("Unknown BflanTemplateKind");
            }
        }
    }
}
