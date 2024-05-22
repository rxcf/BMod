

using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (OutgoingMessages), "SubmitGlobalChatMessage", new Type[] {typeof (ChatMessage)})]
  internal static class GlobalMSG_Block
  {
    private static bool Prefix(ChatMessage message)
    {
      if (!Globals.ytmode)
        return true;
      BluePopupUI.SetPopupValue((PopupMode) 0, "", "Nuh uh!", "You can't submit global/ham radio messages in ytmode!", "I understand", "", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
      Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
      return false;
    }
  }
}
