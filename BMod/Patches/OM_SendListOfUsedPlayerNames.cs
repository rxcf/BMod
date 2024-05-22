

using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (OutgoingMessages), "SendListOfUsedPlayerNames")]
  internal static class OM_SendListOfUsedPlayerNames
  {
    private static bool Prefix() => false;
  }
}
