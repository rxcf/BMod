
using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (PlayerData), "HasUnlockedRecipe")]
  internal static class PD_HasUnlockedRecipe
  {
    private static bool Prefix(ref bool __result)
    {
      if (Globals.recipesHack)
        __result = true;
      return !Globals.recipesHack;
    }
  }
}
