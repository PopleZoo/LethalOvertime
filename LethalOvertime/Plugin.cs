using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;


namespace LethalOvertime
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LethalOvertimeModBase : BaseUnityPlugin
    {
        private const string modGUID = "PopleZoo.LethalOvertime";
        private const string modName = "Lethal Overtime";
        private const string modVersion = "0.0.0.1";
        private readonly Harmony harmony = new Harmony(modGUID);
        public static LethalOvertimeModBase Instance;
        public ManualLogSource mls;


        public static ConfigEntry<float> AdjustScreenPositionXaxis;
        public static ConfigEntry<float> AdjustScreenPositionYaxis;
        public static ConfigEntry<string> TextColorHex;
        public static ConfigEntry<float> FontSize;


        private void Awake()
        {
            if (LethalOvertimeModBase.Instance == null) { LethalOvertimeModBase.Instance = this; };
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Plugin LethalOvertime is loaded!");
            this.LoadConfigs();
            harmony.PatchAll();
        }
        private void LoadConfigs()
        {
            LethalOvertimeModBase.FontSize = Config.Bind("Settings", "FontSize", 20f, "The default font size is 20, to make/see this change you may have to do this manually in the config file itself in the bepinex folder");
            LethalOvertimeModBase.TextColorHex = Config.Bind("Settings", "TextColorHex", "#78FFAE", "The default text color for LethalOvertime text, value must be a hexadecimal");
            LethalOvertimeModBase.AdjustScreenPositionXaxis = Config.Bind("Settings", "AdjustScreenPositionXaxis", 0f, "The default value is 0, you will add or take away from its original position");
            LethalOvertimeModBase.AdjustScreenPositionYaxis = Config.Bind("Settings", "AdjustScreenPositionYaxis", 0f, "The default value is 0, you will add or take away from its original position");
        }
    }
}
