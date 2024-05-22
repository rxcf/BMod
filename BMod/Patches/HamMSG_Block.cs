

using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (OutgoingMessages), "SubmitMessageAtFrequency", new Type[] {typeof (Vector2i), typeof (int), typeof (string)})]
  internal static class HamMSG_Block
  {
    private static bool Prefix(Vector2i mapPoint, int frequency, string message)
    {
      if (!Globals.ytmode)
        return true;
      BluePopupUI.SetPopupValue((PopupMode) 0, "", "Nuh uh!", "You can't submit global/ham radio messages in ytmode!", "I understand", "", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
      Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
      return false;
    }
  }
}
