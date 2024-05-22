

using HarmonyLib;
using Il2Cpp;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (ConfigData), "IsBlockRecallingAllowed", new Type[] {typeof (World.BlockType), typeof (World), typeof (PlayerData), typeof (int), typeof (int)})]
  internal static class CD_Recall
  {
    private static bool Prefix(
      ref bool __result,
      World.BlockType blockType,
      World world,
      PlayerData playerData,
      int x,
      int y)
    {
      __result = true;
      return false;
    }
  }
}
