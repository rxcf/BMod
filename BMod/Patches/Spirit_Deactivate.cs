

using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (Spirit), "Deactivate")]
  internal static class Spirit_Deactivate
  {
    private static void Postfix(bool playDyingAudio, bool doDestroyParticle)
    {
      Globals.current_spirit = (Spirit) null;
    }
  }
}
