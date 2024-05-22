
using BMod.Auto;
using HarmonyLib;
using Il2Cpp;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (PlayerData), "GiveExperience", new Type[] {typeof (int)})]
  internal static class PD_AddXP
  {
    private static void Prefix(int amount)
    {
      if (!MineBot.active)
        return;
      Globals.mineBot.xp += amount;
    }
  }
}
