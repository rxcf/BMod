

using HarmonyLib;
using Il2Cpp;
using UnityEngine;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (AdminToolsUI), "DoShowAnimation")]
  internal static class AdminToolsUI_ShowMenu
  {
    private static bool Prefix(bool doInstant = false)
    {
      if (Globals.playerData.playerAdminStatus != 0)
        return true;
      Object.FindObjectOfType<AdminToolsUI>().Close();
      ((UIDrawer) Object.FindObjectOfType<ChatControl>()).FullHide();
      return false;
    }
  }
}
