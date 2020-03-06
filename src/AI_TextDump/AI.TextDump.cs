﻿using BepInEx;

namespace IllusionMods
{
    /// <summary>
    /// Dumps untranslated text to .txt files
    /// </summary>
    [BepInProcess(Constants.StudioProcessName)]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class TextDump : BaseUnityPlugin
    {
        public const string PluginNameInternal = "AI_TextDump";

        private readonly TextResourceHelper textResourceHelper = new AI_TextResourceHelper();
    }
}
