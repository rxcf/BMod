
using BMod.Auto;
using HarmonyLib;
using Il2Cpp;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (SceneLoader), "GoFromWorldToMainMenu")]
  internal static class SL_LeaveWorld
  {
    private static bool Prefix()
    {
      if (!FarmBot.active && !FishBot.active)
        return true;
      Utils.Warning("LeaveWorld message received");
      double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 111, 0.0f, -1);
      return false;
    }
  }
}
