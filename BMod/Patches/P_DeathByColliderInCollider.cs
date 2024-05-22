

using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (Player), "DeathByColliderInCollider", new Type[] {typeof (Vector2i)})]
  internal static class P_DeathByColliderInCollider
  {
    private static bool Prefix(Vector2i mapPoint) => !Globals.noBlockKill;
  }
}
