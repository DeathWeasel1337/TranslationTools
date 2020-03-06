﻿using System.Collections.Generic;
using System.Linq;
#if !HS
using ADV;
#endif

namespace IllusionMods
{
    public class TextResourceHelper
    {
#if !HS
        protected static readonly string ChoiceDelimiter = ",";
        protected static readonly string SpecializedKeyDelimiter = ":";

        // Contains strings that should not be replaced in `Command.Text` based resources
        public HashSet<string> TextKeysBlacklist { get; protected set; }
        // Only dump `Command.Calc` strings if `params.Args[0]` listed here
        public HashSet<string> CalcKeys { get; protected set; }
        // Only dump `Command.Format` strings if `params.Args[0]` listed here
        public HashSet<string> FormatKeys { get; protected set; }

        protected Dictionary<Command, bool> SupportedCommands = new Dictionary<Command, bool>() {
            { ADV.Command.Text, true }
        };

        public static bool ContainsNonAscii(string input) => input.ToCharArray().Any((c) => c > sbyte.MaxValue);

        public bool IsSupportedCommand(Command command)
        {
            if (SupportedCommands.TryGetValue(command, out bool result))
            {
                return result;
            }
            // default to false
            return false;
        }

        // Certain commands encode multiple pieces of data into their strings
        // we only want to expose the part that should be translated
        // use prefixing to signal to resource replacement when this is needed
        public string GetSpecializedKey(ScenarioData.Param param, int i, out string toTranslate)
        {
            string key = toTranslate = param.Args[i];
            if (key.IsNullOrEmpty() || param.Command == Command.Text)
            {
                return key;
            }

            if (param.Command == Command.Choice)
            {
                if (!key.Contains(ChoiceDelimiter))
                {
                    // only choices that contain delimiters need translation
                    return string.Empty;
                }
                toTranslate = key.Split(ChoiceDelimiter.ToCharArray())[0];
                if (!ContainsNonAscii(toTranslate))
                {
                    return string.Empty;
                }
            }
            else
            {
                // does not used specialized key
                return key;
            }
            return string.Join(SpecializedKeyDelimiter, new string[] { param.Command.ToString().ToUpperInvariant(), toTranslate });
        }

        public string GetSpecializedKey(ScenarioData.Param param, int i) => GetSpecializedKey(param, i, out string _);

        // For commands that encode multiple pieces of data into their strings
        // keep all the extra data from the asset file and only replace the         
        // displayed section (otherwise just returns the passed translation)
        public string GetSpecializedTranslation(ScenarioData.Param param, int i, string translation)
        {
            if (param.Command == Command.Choice)
            {
                try
                {
                    return string.Join(ChoiceDelimiter,
                                new string[] { translation, param.Args[i].Split(ChoiceDelimiter.ToCharArray(), 2)[1] });
                }
                catch
                {
                    // something went wrong, return original below
                }
            }

            return translation;
        }

        virtual public bool IsReplacement(ScenarioData.Param param) => false;

        virtual public  Dictionary<string, string> BuildReplacements(IEnumerable<ScenarioData.Param> assetList)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (ScenarioData.Param param in assetList)
            {
                if (IsReplacement(param) && param.Args.Length > 2 && param.Args[0].StartsWith("sel"))
                {
                    result.Add(param.Args[1], param.Args[2]);
                }
            }
            return result;

        }
#endif

    }
}
