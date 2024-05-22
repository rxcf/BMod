

using BMod.Auto;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (NetworkClient), "DoHardReconnect", new Type[] {typeof (HardReconnectReason), typeof (int)})]
  internal static class NC_DoHardReconnect
  {
    private static void Prefix(HardReconnectReason reason, int reasonParam = -1)
    {
      if (!MineBot.active)
        return;
      string str;
      switch (reason - 1)
      {
        case 0:
          str = "path issues/far tp/noclip";
          break;
        case 1:
          str = "out of range teleport/speedhack";
          break;
        case 2:
          str = "fly hack (no jetpack)";
          break;
        case 3:
          str = "lag and disconnect";
          break;
        case 4:
          str = "fast fist hack";
          break;
        default:
          str = "(undefined)";
          break;
      }
      if (MineBot.useWebhook)
        Globals.mineBot.SendErrorFatal("Disconnected", string.Format("the server sent disconnect message: {0} ({1})\nProbably {2}", (object) reason, (object) Globals.world.worldName, (object) str), Globals.mineBot.GenerateGridMapAsJSON((short) 2));
      MelonLogger.Warning(string.Format("Recieved disconnect message: {0}, probably {1}", (object) reason, (object) str));
      if (Globals.world != null && Globals.world.worldName == "MINEWORLD")
      {
        SummonTimer.Run(0, 0, 30f, BSON.SummonTimerAction.BotWait, "MBCC");
        Globals.mineBot.state = MineBot.BotState.Reconnecting;
        NetworkClient.currentWorld = "";
        NetworkClient.joinWorldOnConnect1 = "";
        NetworkClient.joinWorldOnConnect2 = "";
        Globals.mineBot.dcReason = reason;
        Globals.mineBot.dcRealReason = str;
        Globals.mineBot.reconnecting = true;
      }
      else
        MineBot.Stop("Can't reconnect because we were not in MINEWORLD!");
    }
  }
}
