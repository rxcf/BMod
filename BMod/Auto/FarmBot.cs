
using BMod.Patches;
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using System;
using System.Diagnostics;

namespace BMod.Auto
{
  internal class FarmBot
  {
    internal static FarmBot.LimitMode limit = FarmBot.LimitMode.None;
    private static PlayerData.InventoryItemType _itemType = (PlayerData.InventoryItemType) 0;
    internal static World.BlockType blockType = (World.BlockType) 0;
    private static PlayerData.InventoryKey _ik = new PlayerData.InventoryKey();
    private static PlayerData.InventoryKey _seedLK = new PlayerData.InventoryKey();
    internal static DateTime startTime = new DateTime();
    internal static DateTime finishTime = new DateTime();
    internal static DateTime timeSpent = new DateTime(0L);
    private static string startWorld = "";
    internal static string startTimeString = "n\\a";
    internal static string maxTimeString = "";
    public static string rejoinEntry = "";
    internal static bool active = false;
    internal static bool rejoining = false;
    internal static bool leaveEnd = false;
    internal static bool hibernateEnd = false;
    internal static bool use_ValidatePlace = true;
    internal static bool outAreaLegal = false;
    private static Vector2i _smp = new Vector2i();
    internal static List<Vector2i> mps = new List<Vector2i>();
    private static Vector2i moreBlocksMP = new Vector2i();
    internal static int defaultHitsLeft = 0;
    internal static int maxLoops = 0;
    internal static int maxBlocks = 30;
    private static short _cb = 0;
    private static int _hl = 0;
    internal static int destroyed = 0;
    internal static int gemsProfit = 0;
    internal static int seedsDropped = 0;
    internal static int loops = 0;
    internal static int vAttempt = 0;
    internal static int customHitsAmt = 0;
    internal static float wait_NextStep = 0.9f;
    internal static float wait_NextHit = 0.22f;

    private static Vector2i _cmp => Globals.player.currentPlayerMapPoint;

    private static bool CanWeFarmHere(List<Vector2i> workArea)
    {
      foreach (Vector2i vector2i in workArea)
      {
        if (!Globals.world.DoesPlayerHaveRightToModifyMapPoint(vector2i, Globals.playerData.playerId, false, Globals.world.GetBlockType(vector2i), Globals.playerData, true))
          return false;
      }
      return true;
    }

    internal static void Place()
    {
      // ISSUE: unable to decompile the method.
    }

    internal static void ValidatePlace()
    {
      // ISSUE: unable to decompile the method.
    }

    internal static void Break()
    {
      try
      {
        if (!FarmBot.active)
          return;
        if (Vector2i.op_Inequality(FarmBot._cmp, FarmBot._smp) && !FarmBot.outAreaLegal)
          FarmBot.Stop("Out of Farm Area");
        if (FarmBot._hl <= 0)
        {
          ++FarmBot._cb;
          FarmBot._hl = FarmBot.defaultHitsLeft;
          ++FarmBot.destroyed;
        }
        if (FarmBot._cb >= (short) 7 && FarmBot._hl == 0)
        {
          SummonTimer.Run(0, 0, FarmBot.wait_NextStep, BSON.SummonTimerAction.BotWait, "NextLoop");
          FarmBot._cb = (short) 0;
          FarmBot._hl = FarmBot.defaultHitsLeft;
        }
        else
        {
          Vector2i mp = FarmBot.mps[(int) FarmBot._cb];
          DateTime now = DateTime.Now;
          if (Globals.world.GetBlockType(mp) == null && Globals.world.GetBlockBackgroundType(mp) == null && Globals.world.GetBlockWaterType(mp) == 0 && Globals.world.GetBlockType(mp) == 0)
          {
            ++FarmBot._cb;
            FarmBot._hl = FarmBot.defaultHitsLeft;
            if (FarmBot._cb == (short) 8)
            {
              SummonTimer.Run(0, 0, FarmBot.wait_NextStep, BSON.SummonTimerAction.BotWait, "NextLoop");
              FarmBot._cb = (short) 0;
            }
            else
              SummonTimer.Run(0, 0, FarmBot.wait_NextHit, BSON.SummonTimerAction.BotWait, "NextHit");
          }
          else
          {
            switch ((int) FarmBot._itemType)
            {
              case 0:
                if (Globals.world.GetBlockType(mp) > 0)
                {
                  OutgoingMessages.SendHitBlockMessage(mp, now, false);
                  --FarmBot._hl;
                  break;
                }
                break;
              case 1:
                if (Globals.world.GetBlockBackgroundType(mp) > 0)
                {
                  OutgoingMessages.SendHitBlockBackgroundMessage(mp, now);
                  --FarmBot._hl;
                  break;
                }
                break;
              case 3:
                if (Globals.world.GetBlockWaterType(mp) > 0)
                {
                  OutgoingMessages.SendHitBlockWaterMessage(mp, now);
                  --FarmBot._hl;
                  break;
                }
                break;
              default:
                FarmBot.Stop("Failed to get BlockType while breaking");
                return;
            }
            if (FarmBot._cb >= (short) 7 && FarmBot._hl == 0)
            {
              SummonTimer.Run(0, 0, FarmBot.wait_NextStep, BSON.SummonTimerAction.BotWait, "NextLoop");
              FarmBot._cb = (short) 0;
              FarmBot._hl = FarmBot.defaultHitsLeft;
            }
            else
              SummonTimer.Run(0, 0, FarmBot.wait_NextHit, BSON.SummonTimerAction.BotWait, "NextHit");
          }
        }
      }
      catch
      {
        FarmBot.Stop("ERROR_BREAK_DURING_INVOKE");
      }
    }

    internal static void NextLoop()
    {
      if (!FarmBot.active)
        return;
      try
      {
        foreach (PlayerData.InventoryKey inventoryItem in (Il2CppArrayBase<PlayerData.InventoryKey>) Globals.gameplayUI.inventoryControl.GetInventoryItems())
        {
          if (inventoryItem.blockType == FarmBot.blockType && inventoryItem.itemType == 2)
          {
            FarmBot._seedLK = inventoryItem;
            if (Globals.gameplayUI.inventoryControl.GetAmountTextForInventoryKey(inventoryItem) >= 999)
            {
              World.BlockType blockType1 = Globals.world.GetBlockType(FarmBot._smp.x, FarmBot._smp.y - 2);
              World.BlockType blockType2 = Globals.world.GetBlockType(FarmBot._smp.x - 1, FarmBot._smp.y - 2);
              World.BlockType blockType3 = Globals.world.GetBlockType(FarmBot._smp.x + 1, FarmBot._smp.y - 2);
              if (blockType1 == null && blockType2 == null && blockType3 == 0)
              {
                Utils.Msg("Seeds stack full, trying to drop..");
                Console.WriteLine("FarmBot :: Seeds stack full, trying to drop");
                Globals.giveawayMode = false;
                Globals.player.WarpPlayer(FarmBot._smp.x, FarmBot._smp.y - 2);
                FarmBot.outAreaLegal = true;
                Globals.player.DropItems(inventoryItem, (short) Globals.gameplayUI.inventoryControl.GetAmountTextForInventoryKey(inventoryItem));
                SummonTimer.Run(FarmBot._smp.x, FarmBot._smp.y, 2f, BSON.SummonTimerAction.BotWait, "FarmWarpMP");
                return;
              }
              FarmBot.Stop("Seeds Stack Full");
              return;
            }
          }
          Vector2i moreBlocksMp = FarmBot.moreBlocksMP;
          if (inventoryItem.blockType == FarmBot.blockType && Globals.gameplayUI.inventoryControl.GetAmountTextForInventoryKey(FarmBot._ik) < 10)
          {
            foreach (CollectableData collectable in Globals.world.collectables)
            {
              if (Vector2i.op_Equality(collectable.mapPoint, FarmBot.moreBlocksMP) && collectable.inventoryItemType != 2 && collectable.blockType == FarmBot.blockType && !collectable.isGem)
              {
                Utils.Msg("Trying to get blocks from stock.");
                Console.WriteLine("FarmBot :: Trying to get blocks from stock");
                Globals.giveawayMode = false;
                Globals.player.WarpPlayer(FarmBot.moreBlocksMP.x, FarmBot.moreBlocksMP.y);
                FarmBot.outAreaLegal = true;
                SummonTimer.Run(FarmBot._smp.x, FarmBot._smp.y, 1f, BSON.SummonTimerAction.BotWait, "FarmWarpMP");
                return;
              }
            }
          }
        }
        ++FarmBot.loops;
        FarmBot._hl = FarmBot.GetHitsRequired();
        FarmBot.outAreaLegal = false;
        switch (FarmBot.limit)
        {
          case FarmBot.LimitMode.None:
            FarmBot.Place();
            break;
          case FarmBot.LimitMode.Loops:
            if (FarmBot.loops >= FarmBot.maxLoops)
            {
              FarmBot.Stop("Loops Passed");
              break;
            }
            FarmBot.Place();
            break;
          case FarmBot.LimitMode.Blocks:
            if (FarmBot.destroyed >= FarmBot.maxBlocks)
            {
              FarmBot.Stop("Blocks Destroyed");
              break;
            }
            FarmBot.Place();
            break;
          case FarmBot.LimitMode.Time:
            DateTime now = DateTime.Now;
            if (((DateTime) ref now).CompareTo(FarmBot.finishTime) > 0)
            {
              FarmBot.Stop("Time is Up");
              break;
            }
            FarmBot.Place();
            break;
        }
      }
      catch
      {
        FarmBot.Stop("ERROR_NEXTLOOP_DURING_INVOKE");
      }
    }

    internal static void Start()
    {
      try
      {
        FarmBot._itemType = Globals.gameplayUI.inventoryControl.GetCurrentSelection().itemType;
        FarmBot.blockType = Globals.gameplayUI.inventoryControl.GetCurrentSelection().blockType;
        FarmBot._ik = Globals.gameplayUI.inventoryControl.GetCurrentSelection();
        FarmBot._smp = Globals.player.currentPlayerMapPoint;
        FarmBot.mps = Utils.GetMapPointsGridInRange(1);
        if (FarmBot.mps == null)
          Utils.Error("You can't farm here (target mappoints are not exist)");
        else if (!FarmBot.CanWeFarmHere(FarmBot.mps))
        {
          Utils.Error("You can't autofarm here");
        }
        else
        {
          FarmBot.mps.Remove(FarmBot._smp);
          if (!ConfigData.IsBlockWater(Globals.world.GetBlockWaterType(FarmBot._smp)) && Globals.player.jumpMode != 4 && Globals.player.jumpMode != 7)
            Utils.Error("You will get disconnected if stay here while autofarming! Please place water/wear jetpack or mount");
          else if (FarmBot._itemType != 12 && FarmBot._itemType != 7 && FarmBot._itemType != 2 && FarmBot.blockType > 0)
          {
            Utils.Msg("Starting to autofarm " + TextManager.GetBlockTypeName(FarmBot.blockType));
            Globals.giveawayMode = true;
            Globals.antiBounce = true;
            Globals.godMode = true;
            FarmBot._hl = FarmBot.GetHitsRequired();
            FarmBot.defaultHitsLeft = FarmBot._hl;
            FarmBot._cb = (short) 0;
            Utils.Msg(string.Format("[{0}] BlockHealth: {1}, HitsRequired: {2},", (object) TextManager.GetBlockTypeName(FarmBot.blockType), (object) ConfigData.GetHitsRequired(FarmBot.blockType), (object) FarmBot._hl));
            FarmBot.destroyed = 0;
            FarmBot.gemsProfit = 0;
            FarmBot.seedsDropped = 0;
            FarmBot.loops = 0;
            DateTime now = DateTime.Now;
            FarmBot.startTime = now;
            FarmBot.startTimeString = now.ToString() + string.Format("{0}:{1}:{2}", (object) ((DateTime) ref now).Hour, (object) ((DateTime) ref now).Minute, (object) ((DateTime) ref now).Second);
            FarmBot.finishTime = DateTime.Now;
            FarmBot.active = true;
            FarmBot.startWorld = Globals.world.worldName;
            FarmBot.vAttempt = 0;
            if (FarmBot.limit == FarmBot.LimitMode.Time)
            {
              FarmBot.maxTimeString = FarmBot.maxTimeString.ToLower();
              try
              {
                if (FarmBot.maxTimeString.EndsWith("d") || FarmBot.maxTimeString.EndsWith("day") || FarmBot.maxTimeString.EndsWith("days"))
                {
                  FarmBot.maxTimeString = FarmBot.maxTimeString.Remove(FarmBot.maxTimeString.IndexOf('d'));
                  FarmBot.finishTime = ((DateTime) ref FarmBot.finishTime).AddDays((double) Convert.ToInt32(FarmBot.maxTimeString));
                }
                else if (FarmBot.maxTimeString.EndsWith("h") || FarmBot.maxTimeString.EndsWith("hour") || FarmBot.maxTimeString.EndsWith("hours"))
                {
                  FarmBot.maxTimeString = FarmBot.maxTimeString.Remove(FarmBot.maxTimeString.IndexOf('h'));
                  FarmBot.finishTime = ((DateTime) ref FarmBot.finishTime).AddHours((double) Convert.ToInt32(FarmBot.maxTimeString));
                }
                else if (FarmBot.maxTimeString.EndsWith("m") || FarmBot.maxTimeString.EndsWith("min") || FarmBot.maxTimeString.EndsWith("minute") || FarmBot.maxTimeString.EndsWith("minutes"))
                {
                  FarmBot.maxTimeString = FarmBot.maxTimeString.Remove(FarmBot.maxTimeString.IndexOf('m'));
                  FarmBot.finishTime = ((DateTime) ref FarmBot.finishTime).AddMinutes((double) Convert.ToInt32(FarmBot.maxTimeString));
                }
                else if (FarmBot.maxTimeString.EndsWith("s") || FarmBot.maxTimeString.EndsWith("sec") || FarmBot.maxTimeString.EndsWith("second") || FarmBot.maxTimeString.EndsWith("seconds"))
                {
                  FarmBot.maxTimeString = FarmBot.maxTimeString.Remove(FarmBot.maxTimeString.IndexOf('s'));
                  FarmBot.finishTime = ((DateTime) ref FarmBot.finishTime).AddSeconds((double) Convert.ToInt32(FarmBot.maxTimeString));
                }
              }
              catch
              {
                Utils.Error("Invalid time format, use /fbfm for help");
              }
            }
            if (ConfigData.IsBlockPortal(Globals.world.GetBlockType(FarmBot._cmp)))
            {
              BSONObject asBson = Globals.world.GetWorldItemData(FarmBot._cmp).GetAsBSON();
              if (!string.IsNullOrWhiteSpace(((BSONValue) asBson)["entryPointID"].stringValue))
              {
                FarmBot.rejoinEntry = ((BSONValue) asBson)["entryPointID"].stringValue;
                Utils.Msg("Portal found, rejoin entry: " + FarmBot.rejoinEntry);
              }
              else
              {
                Utils.Warning("Portal found but could not find rejoin entry!");
                FarmBot.rejoinEntry = string.Empty;
              }
            }
            ConfigData.playerInactivitySeconds = 999999;
            ConfigData.playerInactivitySecondsFishing = 999999;
            ConfigData.playerChangeToSleepSeconds = 999999;
            foreach (CollectableData collectable in Globals.world.collectables)
            {
              if (Vector2i.op_Equality(collectable.mapPoint, new Vector2i(FarmBot._smp.x, FarmBot._smp.y - 2)) && collectable.inventoryItemType != 2 && collectable.blockType == FarmBot.blockType && !collectable.isGem)
              {
                FarmBot.moreBlocksMP = collectable.mapPoint;
                Utils.Msg("Found extra blocks to take!");
              }
            }
            Vector2i moreBlocksMp = FarmBot.moreBlocksMP;
            if (false)
              Utils.Warning("No extra blocks found to take! Script will stop when all blocks destroy. You can add extra blocks by dropping them 2blocks down under farm area center");
            World.BlockType blockType1 = Globals.world.GetBlockType(FarmBot._smp.x, FarmBot._smp.y - 2);
            World.BlockType blockType2 = Globals.world.GetBlockType(FarmBot._smp.x - 1, FarmBot._smp.y - 2);
            World.BlockType blockType3 = Globals.world.GetBlockType(FarmBot._smp.x + 1, FarmBot._smp.y - 2);
            if (blockType1 == null && blockType2 == null && blockType3 == 0)
              Utils.Msg("Found mappoint for seeds");
            else
              Utils.Warning("No mappoint for seeds found, please clear area under your farm or bot will stop at 990seeds!");
            FarmBot.outAreaLegal = false;
            Globals.player.WarpPlayer(FarmBot._smp.x, FarmBot._smp.y);
            FarmBot.Place();
          }
          else
            Utils.Error("I can't farm " + TextManager.GetBlockTypeName(FarmBot.blockType) + " for you");
        }
      }
      catch
      {
        FarmBot.Stop("ERROR_START_DURING_INVOKE");
      }
    }

    internal static void Stop(string reason, bool byUser = false)
    {
      Globals.giveawayMode = false;
      Globals.antiBounce = false;
      Globals.godMode = false;
      FarmBot.active = false;
      ConfigData.playerInactivitySeconds = 600;
      ConfigData.playerInactivitySecondsFishing = 1800;
      ConfigData.playerChangeToSleepSeconds = 120;
      FarmBot.moreBlocksMP = new Vector2i();
      if (byUser)
      {
        Console.WriteLine("FarmBot :: was deactivated by user.");
      }
      else
      {
        Console.WriteLine("FarmBot :: " + reason);
        InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("farm bot"), "was deactivated due to\n" + reason);
        InfoPopupUI.ForceShowMenu();
        if (FarmBot.leaveEnd)
          SceneLoader.GoFromWorldToMainMenu();
        if (FarmBot.hibernateEnd)
          Process.Start(new ProcessStartInfo("cmd.exe", "/c shutdown /h"));
      }
    }

    private static int GetHitsRequired()
    {
      if (FarmBot.customHitsAmt >= 1)
        return FarmBot.customHitsAmt;
      int num1 = 0;
      int weaponAndBlockClass = ConfigData.GetHitForceFromWeaponAndBlockClass(FarmBot.blockType, Globals.player.GetTopArmBlockType(), Globals.playerData.GetDamageMultiplier());
      int num2 = ConfigData.GetHitsRequired(FarmBot.blockType) / weaponAndBlockClass;
      if ((double) ConfigData.GetHitsRequired(FarmBot.blockType) / (double) weaponAndBlockClass > (double) (ConfigData.GetHitsRequired(FarmBot.blockType) / weaponAndBlockClass))
        num1 = 1;
      return num2 + num1;
    }

    public static void RejoinFailed(WorldJoinResult worldJoinResult)
    {
      NetworkClient.currentWorld = "";
      ControllerHelper.networkClient.DoHardReconnect((HardReconnectReason) 11, -1);
      FarmBot.Stop("Rejoin World failed: " + worldJoinResult.ToString());
    }

    internal enum LimitMode
    {
      None,
      Loops,
      Blocks,
      Time,
    }

    private class Patches
    {
      [HarmonyPatch(typeof (PlayerData), "AddGems")]
      private static class PD_AddGems
      {
        private static void Prefix(int addAmount)
        {
          if (!FarmBot.active)
            return;
          FarmBot.gemsProfit += addAmount;
        }
      }
    }
  }
}
