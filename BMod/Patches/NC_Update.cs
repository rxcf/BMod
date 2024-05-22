

using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (NetworkClient), "Update")]
  internal static class NC_Update
  {
    private static bool Prefix() => !Globals.lagHack && !Globals.isTeleporting;
  }
}
