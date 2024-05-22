
using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (ProfanityFilter), "Censor")]
  internal static class ProfanityFuck
  {
    private static bool Prefix(ref string __result, ref string str)
    {
      __result = str;
      return !Globals.noProfanityFilter;
    }
  }
}
