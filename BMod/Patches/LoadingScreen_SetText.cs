

using HarmonyLib;
using Il2Cpp;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (LoadingScreen), "SetText")]
  internal static class LoadingScreen_SetText
  {
    private static void Prefix(LoadingScreen __instance, ref string text, bool addDots = true)
    {
      text = text + "\n\n" + Utils.GetLoadingTip(DateTime.Now.Minute);
    }
  }
}
