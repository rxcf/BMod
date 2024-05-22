

using BMod.Auto;
using HarmonyLib;
using Il2CppPrime31;
using UnityEngine;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (CharacterController2D), "move")]
  internal static class CharacterController2D_move
  {
    private static bool Prefix(Vector3 deltaMovement, bool ignoreOneway = false)
    {
      return !Globals.isTeleporting && !FarmBot.active && !MineBot.active;
    }
  }
}
