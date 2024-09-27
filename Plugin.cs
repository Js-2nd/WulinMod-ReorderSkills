using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine.Events;
using UnityEngine.UI;
using WuLin;

namespace ReorderSkills;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Wulin.exe")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(UILearnedSkillPanel), "Awake")]
    [HarmonyPostfix]
    private static void PostUILearnedSkillPanelAwake(UILearnedSkillPanel __instance)
    {
        Log.LogInfo($"Patch UILearnedSkillPanel.Awake {__instance.name}");
        var button = __instance.icon.gameObject.AddComponent<Button>();
        button.onClick.AddListener((UnityAction)(() => { Reorder(__instance.data); }));
    }

    private static void Reorder(KungfuInstance kungfu)
    {
        if (kungfu == null)
        {
            return;
        }

        var chara = kungfu.GameCharacterInstance;
        chara.KungfuInstances.Remove(kungfu);
        chara.kungfuInstances.Add(kungfu);
        Refresh();
    }

    private static UIKongfuPanel _uiKongfuPanel;

    private static void Refresh()
    {
        if (_uiKongfuPanel == null)
        {
            _uiKongfuPanel = UIRoot.Instance.GetRoot(UIRoot.GameUiLayer.Window).GetComponentInChildren<UIKongfuPanel>();
            if (_uiKongfuPanel == null)
            {
                return;
            }
        }

        _uiKongfuPanel.Refresh();
    }
}