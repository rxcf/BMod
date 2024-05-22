

using HarmonyLib;
using Il2Cpp;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (WorldController), "InstantiateMannequin", new Type[] {typeof (int), typeof (int), typeof (World.BlockType)})]
  internal static class WC_MannequinALagger
  {
    private static bool Prefix(int x, int y, World.BlockType blockType)
    {
      return Globals.world.GetBlockCountInWorld(blockType) <= 200;
    }
  }
}
