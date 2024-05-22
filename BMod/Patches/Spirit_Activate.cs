

using HarmonyLib;
using Il2Cpp;
using System;
using UnityEngine;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (Spirit), "Activate", new Type[] {typeof (Vector2)})]
  internal static class Spirit_Activate
  {
    private static void Postfix(Vector2 worldPos)
    {
      Globals.current_spirit = Object.FindObjectOfType<Spirit>();
    }
  }
}
