
using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (WorldController), "KickPlayerByInactivity")]
  internal static class WC_AfkKick
  {
    private static bool Prefix(string deltaParameter) => !Globals.noAFK;
  }
}
