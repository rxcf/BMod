
using BMod.Patches;
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppKernys.Bson;
using MelonLoader;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

 
namespace BMod.Auto
{
  internal class FishBot
  {
    internal static bool active = false;
    private static bool sFishOff = false;
    internal static bool sFishOn = false;
    internal static bool sellFish = false;
    internal static bool leaveEnd = false;
    internal static bool hibernateEnd = false;
    private static float _fishOffTimer = 0.0f;
    internal static float mode__float = 1f;
    internal static int mode = 1;
    internal static int gemsProfit = 0;
    internal static int fishes = 0;
    internal static int ingredients = 0;
    internal static int fails = 0;
    internal static DateTime startTime = new DateTime();
    internal static DateTime timeSpent = new DateTime(0L);
    internal static string startTimeString = "n\\a";
    internal static Vector2i sMP = new Vector2i();
    internal static World.BlockType lastFishBT = (World.BlockType) 0;

    internal static Vector2i cMP => Globals.player.currentPlayerMapPoint;

    private static World.BlockType FindRod()
    {
      foreach (World.BlockType andWeaponBlockType in Globals.playerData.GetPlayerWearWearablesAndWeaponBlockTypes())
      {
        if (ConfigData.IsFishingRod(andWeaponBlockType))
          return andWeaponBlockType;
      }
      return (World.BlockType) 0;
    }

    internal static void Start()
    {
      if (FishBot.FindRod() > 0)
      {
        if ((double) FishingData.GetRodSliderSizeMultiplier(FishBot.FindRod()) < 1.5)
        {
          InfoPopupUI.SetupInfoPopup("ROD BANNED", "Your rod is shit\nmin: flawless rods");
          InfoPopupUI.ForceShowMenu();
        }
        else if (ConfigData.IsFishingLure(Globals.gameplayUI.inventoryControl.GetCurrentSelection().blockType))
        {
          FishBot.active = true;
          FishBot.sMP = FishBot.cMP;
          if (FishBot.mode == 0)
          {
            ((Component) Globals.gameplayUI.battleBorder).gameObject.SetActive(true);
            double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 264, 0.0f, -1);
          }
          else
          {
            double num1 = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 166, 0.0f, -1);
          }
          FishBot.gemsProfit = 0;
          FishBot.fishes = 0;
          FishBot.ingredients = 0;
          FishBot.fails = 0;
          DateTime now = DateTime.Now;
          FishBot.startTime = now;
          FishBot.startTimeString = now.ToString() + string.Format("{0}:{1}:{2}", (object) now.Hour, (object) now.Minute, (object) now.Second);
          ConfigData.playerInactivitySeconds = 999999;
          ConfigData.playerInactivitySecondsFishing = 999999;
          ConfigData.playerChangeToSleepSeconds = 999999;
        }
        else
          Utils.DoCustomNotification("Select Lure!", FishBot.cMP);
      }
      else
        ControllerHelper.notificationController.DoNotification((NotificationController.NotificationType) 100, FishBot.cMP);
    }

    public static void Stop(string reason, bool byUser = false, bool remote = false)
    {
      FishBot.active = false;
      ConfigData.playerInactivitySeconds = 600;
      ConfigData.playerInactivitySecondsFishing = 1800;
      ConfigData.playerChangeToSleepSeconds = 120;
      if (!byUser)
      {
        MelonLogger.Error("FishBot :: " + reason);
        InfoPopupUI.SetupInfoPopup(TextManager.Capitalize(nameof (FishBot)), "was deactivated due to\n" + reason);
        InfoPopupUI.ForceShowMenu();
        if (FishBot.leaveEnd)
          SceneLoader.GoFromWorldToMainMenu();
        if (FishBot.hibernateEnd)
          Process.Start(new ProcessStartInfo("cmd.exe", "/c shutdown /h"));
      }
      else
        MelonLogger.Msg("FishBot :: was deactivated by user.");
      if (FishBot.mode == 0)
      {
        ((Component) Globals.gameplayUI.battleBorder).gameObject.SetActive(false);
        double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 266, 0.0f, -1);
      }
      else
      {
        double num1 = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 12, 0.0f, -1);
      }
    }

    internal static bool SetBait()
    {
      if (Globals.playerData.GetCurrentFreeSlotsCount() < (short) 1)
      {
        FishBot.Stop("Inventory Full");
        return false;
      }
      if (!ConfigData.IsFishingLure(Globals.gameplayUI.inventoryControl.GetCurrentSelection().blockType))
      {
        FishBot.Stop("No Lure");
        return false;
      }
      Vector2i vector2i;
      // ISSUE: explicit constructor call
      ((Vector2i) ref vector2i).\u002Ector(FishBot.cMP.x - 1, FishBot.cMP.y - 1);
      if (Globals.player.lastPlayerDirection == 3)
      {
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i).\u002Ector(FishBot.cMP.x + 1, FishBot.cMP.y - 1);
      }
      return Globals.worldController.SetBaitWithTool(Globals.gameplayUI.inventoryControl.GetCurrentSelection().blockType, vector2i, 0.0f);
    }

    private class Patches
    {
      [HarmonyPatch(typeof (FishingGaugeMinigameUI), "Update")]
      private static class Fishing_Update
      {
        private static void Postfix(FishingGaugeMinigameUI __instance)
        {
          if (!FishBot.active)
            return;
          __instance.targetAreaPosition = __instance.fishPosition + __instance.fishVelocity * Time.deltaTime;
          if (FishBot.mode == 0)
          {
            ((Graphic) __instance.fish).color = Color.yellow;
            if ((double) __instance.progress > 0.85000002384185791 && (double) __instance.progress < 0.89999997615814209)
              __instance.progress = 0.99f;
          }
          if ((double) __instance.progress == 1.0)
          {
            __instance.LandButtonPressed();
            FishBot.sFishOff = true;
          }
          if (FishBot.mode == 1)
          {
            if (FishBot.sFishOff && (double) __instance.progress >= 0.800000011920929)
            {
              FishBot.sFishOff = false;
              __instance.fishIsOnTheRun = true;
              __instance.fishPauseChance = 0.0f;
              __instance.targetAreaOverlappingFish = false;
              SummonTimer.Run(0, 0, 2f, BSON.SummonTimerAction.BotWait, "FishOff");
            }
            else if (FishBot.sFishOn)
            {
              __instance.fishPauseChance = 0.0f;
              __instance.fishIsOnTheRun = false;
              __instance.targetAreaOverlappingFish = true;
            }
            else if (!FishBot.sFishOff)
            {
              __instance.targetAreaOverlappingFish = false;
              __instance.fishIsOnTheRun = true;
              __instance.fishPauseChance = 0.0f;
            }
            FishBot._fishOffTimer += Time.deltaTime;
            if ((double) FishBot._fishOffTimer >= 0.5)
            {
              FishBot._fishOffTimer -= 0.5f;
              BSONObject bsonObject = new BSONObject();
              ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("FiOffAM");
              ((BSONValue) bsonObject)["FiD"] = BSONValue.op_Implicit(0.30000001192092896);
              OutgoingMessages.AddOneMessageToList(bsonObject);
            }
            if ((double) __instance.fishVelocity == 0.0)
              __instance.fishVelocity = (double) __instance.fishPosition <= 0.5 ? 0.3f : -0.3f;
          }
        }

        [HarmonyPatch(typeof (BaseMenuUI), "Update")]
        private static class RootUI_ShowResults
        {
          private static void Postfix()
          {
            if (!FishBot.active)
              return;
            FishingResultsPopupUI objectOfType = Object.FindObjectOfType<FishingResultsPopupUI>();
            if (Object.op_Inequality((Object) objectOfType, (Object) null))
              objectOfType.TakeFishPressed();
          }
        }

        [HarmonyPatch(typeof (FishingGaugeMinigameUI), "SetupMinigame", new Type[] {typeof (World.BlockType), typeof (World.BlockType)})]
        private static class Fishing_SetupMinigame
        {
          private static void Postfix(World.BlockType rod, World.BlockType caughtFish)
          {
            InfoPopupUI.SetupInfoPopup("FISHING STARTED!", TextManager.GetBlockTypeName(caughtFish));
            InfoPopupUI.ForceShowMenu();
            MelonLogger.Msg("Catching Fish: " + TextManager.GetBlockTypeName(caughtFish));
            FishBot.sFishOff = true;
            FishBot.sFishOn = false;
            FishBot.lastFishBT = caughtFish;
            if (!ConfigData.IsFish(caughtFish))
              return;
            ++FishBot.fishes;
            FishBot.gemsProfit += ConfigData.GetFishRecycleValueForFishRecycler(caughtFish);
          }
        }
      }
    }
  }
}
