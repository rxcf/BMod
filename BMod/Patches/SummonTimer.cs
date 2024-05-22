
using BMod.Auto;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

 
namespace BMod.Patches
{
  internal class SummonTimer : MelonMod
  {
    public static bool doNow = false;
    public static float _time = 6f;
    private static int _PX = 1;
    private static int _PY = 1;
    public static BSON.SummonTimerAction lastAction = BSON.SummonTimerAction.Summon;
    public static string _worldToWarp = "";
    public static bool active = false;
    public Action onTimerAction = (Action) null;

    public static void Run(
      int PX,
      int PY,
      float time = 6f,
      BSON.SummonTimerAction action = BSON.SummonTimerAction.Summon,
      string worldToWarp = "BMOD")
    {
      if (SummonTimer.active)
        return;
      SummonTimer._time = time;
      SummonTimer._PX = PX;
      SummonTimer._PY = PY;
      SummonTimer.doNow = true;
      SummonTimer.lastAction = action;
      SummonTimer._worldToWarp = worldToWarp;
      SummonTimer.active = true;
      if (action == BSON.SummonTimerAction.Summon)
      {
        Utils.Msg("Someone just summoned you, Warp in 6s!");
      }
      else
      {
        if (action != BSON.SummonTimerAction.WarpOwnerGM)
          return;
        Utils.Msg("OWNER just intersummoned everyone, Warp in 30s!");
      }
    }

    public static void OnTimer()
    {
      SummonTimer.active = false;
      switch (SummonTimer.lastAction)
      {
        case BSON.SummonTimerAction.Summon:
          Globals.player.WarpPlayer(SummonTimer._PX, SummonTimer._PY);
          InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("Summoned"), "Time is Up");
          InfoPopupUI.ForceShowMenu();
          break;
        case BSON.SummonTimerAction.WarpOwnerGM:
          if (string.IsNullOrEmpty(StaticPlayer.lastWorldBeforeDisconnect))
          {
            SceneLoader.GoFromMainMenuToWorld(SummonTimer._worldToWarp, "default");
            break;
          }
          ControllerHelper.chatUI.Submit("/w " + SummonTimer._worldToWarp + " default");
          break;
        case BSON.SummonTimerAction.AutoFlag:
          Globals.chatUI.Submit(SummonTimer._worldToWarp);
          Object.FindObjectOfType<PlayerNamesManager>().UpdateMyStatusIcon((PlayerNamesManager.StatusIconType) 0);
          break;
        case BSON.SummonTimerAction.PlayAudio:
          OutgoingMessages.SendPlayPlayerAudioMessage(18, 2871);
          string str1 = "";
          string str2 = "";
          if (Globals.playerData.clanFaction > 0)
            str1 = Globals.playerData.clanFaction == 1 ? "<#56C6F4>[" + Globals.playerData.clanTag + "]" : "<#D868F8>[" + Globals.playerData.clanTag + "]";
          short countryCode = Globals.playerData.countryCode;
          string str3 = countryCode.ToString();
          if (countryCode.ToString().Length == 1)
            str3 = "00" + countryCode.ToString();
          else if (countryCode.ToString().Length == 2)
            str3 = "0" + countryCode.ToString();
          if (Globals.world.lockWorldDataHelper != null)
          {
            bool flag1 = Globals.world.lockWorldDataHelper.DoesPlayerHaveAccessToLock(Globals.playerData.playerId, Globals.world.clanID != 0 && Globals.world.clanID == Globals.playerData.clanId, Globals.playerData.clanMemberRank);
            bool flag2 = Globals.world.lockWorldDataHelper.DoesPlayerHaveMinorAccessToLock(Globals.playerData.playerId, Globals.world.clanID != 0 && Globals.world.clanID == Globals.playerData.clanId, Globals.playerData.clanMemberRank);
            bool flag3 = Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockId() == Globals.playerData.playerId;
            if (flag1)
              str2 = "<sprite=\"FlagAtlas\" name=\"rmaj\">";
            if (flag2)
              str2 = "<sprite=\"FlagAtlas\" name=\"rmin\">";
            if (flag3)
              str2 = "<sprite=\"FlagAtlas\" name=\"rown\">";
          }
          ((TMP_Text) Globals.player.playerNameTextMeshPro).text = str2 + "<sprite=\"Flagatlas\" name=\"" + str3 + "\"><#ff8000>" + StaticPlayer.theRealPlayername + " " + str1;
          break;
        case BSON.SummonTimerAction.BotWait:
          try
          {
            switch (SummonTimer._worldToWarp)
            {
              case "AfterPlace":
                FarmBot.ValidatePlace();
                break;
              case "BufferWorldForwarding":
                Globals.mineBot.JoinMine(Globals.mineBot.GetMineLevelPriority());
                break;
              case "BufferWorldJoined":
                Globals.mineBot.CheckOverload();
                SummonTimer.Run(0, 0, 2f, BSON.SummonTimerAction.BotWait, "BufferWorldForwarding");
                break;
              case "FarmWarpMP":
                Main.GoToMP(new Vector2i(SummonTimer._PX, SummonTimer._PY));
                Globals.giveawayMode = true;
                SummonTimer.Run(SummonTimer._PX, SummonTimer._PY, 0.5f, BSON.SummonTimerAction.BotWait, "WarpSMP");
                break;
              case "FishOff":
                FishBot.sFishOn = true;
                break;
              case "MBCC":
                MainMenuLogic objectOfType = Object.FindObjectOfType<MainMenuLogic>();
                if (Object.op_Inequality((Object) objectOfType, (Object) null))
                {
                  Globals.mineBot.state = MineBot.BotState.JoinBufferWorld;
                  SceneLoader.CheckIfWeCanGoFromWorldToWorld(Globals.mineBot.bufferWorld, Globals.mineBot.bufferEntry, Action<WorldJoinResult>.op_Implicit(new Action<WorldJoinResult>(Globals.mineBot.WorldRejoinFail)), false, (Action<WorldJoinResult>) null);
                  objectOfType.ShowLoading("Loading buffer world", true, Globals.mineBot.bufferWorld + "," + Globals.mineBot.bufferEntry, true);
                  break;
                }
                MineBot.Stop(string.Format("Recieved disconnect message: {0}, probably {1}", (object) Globals.mineBot.dcReason, (object) Globals.mineBot.dcRealReason));
                break;
              case "NextHit":
                FarmBot.Break();
                break;
              case "NextLoop":
                FarmBot.NextLoop();
                break;
              case "Rejoin":
                FarmBot.NextLoop();
                break;
              case "WarpSMP":
                FarmBot.NextLoop();
                break;
              default:
                FarmBot.Stop("UNIVERSAL REASON (NOT JUST FARMBOT) : ERROR_TIMER_ENDED_INVALID_OPTION");
                break;
            }
          }
          catch
          {
            FarmBot.Stop("UNIVERSAL REASON (NOT JUST FARMBOT) : ERROR_TIMER_ENDED_DURING_INVOKE");
          }
          break;
      }
    }

    public async void OnTimerAction(int waitMilliseconds, Action onTimer)
    {
      await Task.Run((Action) (() =>
      {
        Thread.Sleep(waitMilliseconds);
        onTimer();
      }));
    }
  }
}
