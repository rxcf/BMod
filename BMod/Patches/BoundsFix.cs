

using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (KukouriCamera), "CheckCameraBounds")]
  internal static class BoundsFix
  {
    private static bool Prefix() => !(Globals.world.worldName == "MINEWORLD");
  }
}
