
 

using BMod.Auto;
using BMod.Discord;
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppKernys.Bson;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

 
namespace BMod.Patches
{
  internal class Patches
  {
    public static bool shouldUpdateESP = false;
    public static bool sendP = false;
    public static List<Vector2i> giftBoxes = new List<Vector2i>();
    private static string[] _repeatFails = new string[6]
    {
      "Nuh Uh",
      "These jokes are not working on me",
      "I don't understand, try smth else",
      "//firedeath",
      "I'm not stupid",
      "Don't even try"
    };

    public static void AntiIce()
    {
      foreach (int iceBlock in Globals.iceBlocks)
      {
        if (Globals.antiBounce)
          ConfigData.SetBlockGroundDamping("10", iceBlock);
        else
          ConfigData.SetBlockGroundDamping("1", iceBlock);
      }
    }

    private static void AntiGlue()
    {
      foreach (int glueBlock in Globals.glueBlocks)
      {
        if (Globals.antiBounce)
          ConfigData.SetBlockRunSpeed("1.8", glueBlock);
        else
          ConfigData.SetBlockRunSpeed("0.3", glueBlock);
      }
    }

    public static void ChatSend(string text)
    {
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockHot")]
    private static class CD_IsBlockHot
    {
      private static bool Prefix(ref World.BlockType blockType)
      {
        BMod.Patches.Patches.AntiIce();
        BMod.Patches.Patches.AntiGlue();
        return !Globals.antiBounce;
      }
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockSpring")]
    private static class CD_IsBlockSpring
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockPinball")]
    private static class CD_IsBlockPinball
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockElastic")]
    private static class CD_IsBlockElastic
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockWind")]
    private static class CD_IsBlockWind
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockElevator")]
    private static class CD_IsBlockElevator
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockTrampolin")]
    private static class CD_IsBlockTrampolin
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockSwimming")]
    private static class CD_IsBlockSwimming
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "GetDeflectorMaxForceRange")]
    private static class CD_GetDeflectorMaxForceRange
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (ConfigData), "GetDeflectorRange")]
    private static class CD_GetDeflectorRange
    {
      private static bool Prefix(ref World.BlockType blockType) => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (Player), "ShouldBelowBlockDoBounce")]
    private static class Pl_ShouldBelowBlockDoBounce
    {
      private static bool Prefix() => !Globals.antiBounce;
    }

    [HarmonyPatch(typeof (WorldController), "BlockColliderAndLayerHelper")]
    private static class WC_BlockColliderAndLayerHelper
    {
      private static void Prefix(ref World.BlockType blockType)
      {
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        if (!ConfigData.IsBlockInstakill((World.BlockType) ^(int&) ref blockType) || !Globals.antiBounce)
          return;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) ref blockType = 3386;
      }
    }

    [HarmonyPatch(typeof (Player), "HitPlayerFromBlock", new Type[] {typeof (int), typeof (World.BlockType), typeof (Vector2i)})]
    private static class Pl_HitPlayerFromBlock
    {
      private static bool Prefix(
        Player __instance,
        ref bool __result,
        int hitForce,
        World.BlockType hitFromBlockBlockType,
        Vector2i blockMapPoint)
      {
        if (!Globals.godMode && !Globals.isTeleporting)
          return true;
        __result = true;
        return false;
      }
    }

    [HarmonyPatch(typeof (Player), "HitPlayerFromBlock", new Type[] {typeof (World.BlockType), typeof (Vector2i), typeof (bool)})]
    private static class Pl_HitPlayerFromBlock2
    {
      private static bool Prefix(
        Player __instance,
        ref bool __result,
        World.BlockType blocktype,
        Vector2i blockMapPoint,
        bool shouldEndDamageFromTrap = false)
      {
        if (!Globals.godMode && !Globals.isTeleporting)
          return true;
        __result = true;
        return false;
      }
    }

    [HarmonyPatch(typeof (Player), "CanGiveAIEnemyDamage")]
    private static class Pl_CanGiveAIEnemyDamage
    {
      private static bool Prefix() => !Globals.godMode && !Globals.isTeleporting;
    }

    [HarmonyPatch(typeof (Player), "HitPlayerFromAIEnemy", new Type[] {typeof (AIBase), typeof (AIDamageModelType)})]
    private static class Pl_HitPlayerFromAIEnemy1
    {
      private static bool Prefix()
      {
        if (Globals.godMode || Globals.isTeleporting)
          Globals.player.TakeHitFromOtherPlayer("", false, false, (World.BlockType) 0, 0, (float) ConfigData.playerHitPoints);
        return !Globals.godMode;
      }
    }

    [HarmonyPatch(typeof (Player), "HitPlayerFromAIEnemy", new Type[] {typeof (int), typeof (AIEnemyType)})]
    private static class Pl_HitPlayerFromAIEnemy2
    {
      private static bool Prefix()
      {
        if (Globals.godMode || Globals.isTeleporting)
          Globals.player.TakeHitFromOtherPlayer("", false, false, (World.BlockType) 0, 0, (float) ConfigData.playerHitPoints);
        return !Globals.godMode;
      }
    }

    [HarmonyPatch(typeof (AIEnemyMonoBehaviourBase), "TouchDamageHelper")]
    private static class AIEnemyMonoBehaviourBase_TouchDamageHelper
    {
      private static bool Prefix() => !Globals.godMode && !Globals.isTeleporting;
    }

    [HarmonyPatch(typeof (ConfigData), "IsBlockPortal")]
    private static class CD_IsBlockPortal
    {
      private static bool Prefix(ref bool __result)
      {
        if (Globals.ignoreVortex)
        {
          ConfigData.vortexPortalActivateDistance = 0.0f;
          __result = false;
        }
        else
          ConfigData.vortexPortalActivateDistance = 0.14f;
        return !Globals.ignoreVortex;
      }
    }

    [HarmonyPatch(typeof (Player), "IsPlayerInMapPoint")]
    private static class Pl_IsPlayerInMapPoint
    {
      private static bool Prefix(ref bool __result)
      {
        if (Globals.blockOnPlayer)
          __result = false;
        return !Globals.blockOnPlayer;
      }
    }

    [HarmonyPatch(typeof (ConfigData), "CanPlayerPickCollectableFromBlock")]
    private static class CD_CanPlayerPickCollectableFromBlock
    {
      private static bool Prefix(ref bool __result)
      {
        if (Globals.antiCollect)
          __result = false;
        return !Globals.antiCollect;
      }
    }

    [HarmonyPatch(typeof (RootUI), "SetWorldLighting")]
    private static class RootUI_SetWorldLighting
    {
      public static bool Prefix() => Globals.world.worldType <= 0;
    }

    [HarmonyPatch(typeof (WorldController), "InstantiateFogOfWar")]
    private static class WC_InstantiateFogOfWar
    {
      public static bool Prefix() => false;
    }

    [HarmonyPatch(typeof (ChatUI), "NewMessage", new Type[] {typeof (ChatMessage)})]
    private class ChatUI_NewMessage
    {
      private static bool Prefix(ref ChatMessage msg)
      {
        if (msg.channelType == 0 && Globals.repeatTroll)
        {
          foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
          {
            if (msg.userID == Globals.repeatTrollId && msg.channelType == 0)
            {
              string str1 = msg.message.ToLower().Replace(" i ", " you ");
              if (str1.StartsWith("i ") || str1.StartsWith("я ") || str1.StartsWith("im ") || str1.StartsWith("i'm") || str1.StartsWith("me ") || str1.StartsWith("am "))
                str1 = "you " + str1.Remove(0, 3);
              if (str1.EndsWith(" i") || str1.EndsWith(" я") || str1.EndsWith(" im") || str1.EndsWith(" i'm") || str1.EndsWith(" me") || str1.EndsWith(" am"))
                str1 = str1.Remove(str1.Length - 3) + " you";
              string str2 = str1.Replace(" я ", " ты ").Replace(" im ", " you ").Replace(" i'm ", " you ").Replace(" me ", " you ").Replace(" am ", " you ").Replace("i'm", "you").Replace(" " + StaticPlayer.theRealPlayername.ToLower() + " ", " " + msg.nick + " ");
              if (str2.StartsWith("/"))
                str2 = BMod.Patches.Patches._repeatFails[new Random().Next(0, 5)];
              Globals.chatUI.Submit(str2);
              break;
            }
          }
        }
        if ((msg.channelType == 1 || msg.channelType == 2) && !msg.message.Contains("(" + msg.channel + ")") && msg.nick != StaticPlayer.theRealPlayername)
          msg.message = msg.message + " (" + msg.channel + ")";
        return true;
      }
    }

    [HarmonyPatch(typeof (MainMenuPersonal), "LogoutButtonClicked")]
    private static class MMPersonal_LogoutButtonClicked
    {
      private static void Prefix()
      {
        UserIdent.LogOut();
        SceneLoader.GoToWelcomeScene();
      }
    }

    [HarmonyPatch(typeof (TextBubble), "Show", new Type[] {typeof (string), typeof (Action)})]
    private static class TextBubble_Show
    {
      private static void Prefix(ref string bubbleText, Action doAfterTween = null)
      {
        if (!bubbleText.ToLower().Contains("size=-") && !bubbleText.ToLower().Contains("size=999"))
          return;
        bubbleText = "<#ff0000>[AntiCrasher]</color><br><noparse>" + bubbleText;
      }
    }

    [HarmonyPatch(typeof (ShowOverUI), "Show", new Type[] {typeof (Vector3), typeof (PlayerData.InventoryKey), typeof (InventoryItemBase)})]
    private static class ShowOverUI_Show
    {
      private static void Prefix(
        Vector3 pos,
        PlayerData.InventoryKey ik,
        InventoryItemBase dataClass = null)
      {
        if (Globals.playerData.GetInventoryData(ik) == null)
          return;
        World.BlockType blockType = ik.blockType;
        BSONObject asBson = Globals.playerData.GetInventoryData(ik).GetAsBSON();
        if (((BSONValue) asBson)["inventoryClass"].stringValue.Contains("TrophyFish") && ((BSONValue) asBson)["playerName"].stringValue.ToLower().Contains("size=-"))
        {
          ((BSONValue) asBson)["playerName"] = BSONValue.op_Implicit("<#ff0000>[AntiCrasher]</color><br><noparse>" + ((BSONValue) asBson)["playerName"].stringValue);
          Globals.playerData.GetInventoryData(ik).SetViaBSON(asBson);
        }
        else if ((((BSONValue) asBson)["inventoryClass"].stringValue.Contains("FishingTournament") || ((BSONValue) asBson)["inventoryClass"].stringValue.Contains("CardSeasonsTrophy") || ((BSONValue) asBson)["inventoryClass"].stringValue == "BestSetTrophyInventoryData") && ((BSONValue) asBson)["winnerName"].stringValue.ToLower().Contains("size=-"))
        {
          ((BSONValue) asBson)["winnerName"] = BSONValue.op_Implicit("<#ff0000>[AntiCrasher]</color><br><noparse>" + ((BSONValue) asBson)["winnerName"].stringValue);
          Globals.playerData.GetInventoryData(ik).SetViaBSON(asBson);
        }
        else if (((BSONValue) asBson)["inventoryClass"].stringValue.ToLower().Contains("wotw") && (((BSONValue) asBson)["winnername"].stringValue.ToLower().Contains("size=-") || ((BSONValue) asBson)["worldname"].stringValue.ToLower().Contains("size=-")))
        {
          ((BSONValue) asBson)["winnername"] = BSONValue.op_Implicit("");
          ((BSONValue) asBson)["worldname"] = BSONValue.op_Implicit("");
          Globals.playerData.GetInventoryData(ik).SetViaBSON(asBson);
        }
        else if (((BSONValue) asBson)["inventoryClass"].stringValue.Contains("Bottle") && ((BSONValue) asBson)["sendername"].stringValue.ToLower().Contains("size=-"))
        {
          ((BSONValue) asBson)["sendername"] = BSONValue.op_Implicit("<#ff0000>[AntiCrasher]</color><br>");
          ((BSONValue) asBson)["worldname"] = BSONValue.op_Implicit("[REMOVED]");
          ((BSONValue) asBson)["topic"] = BSONValue.op_Implicit("[REMOVED]");
          ((BSONValue) asBson)["message"] = BSONValue.op_Implicit("[REMOVED]");
          Globals.playerData.GetInventoryData(ik).SetViaBSON(asBson);
        }
      }
    }

    [HarmonyPatch(typeof (ChatUI), "Submit")]
    private static class CommandHandler
    {
      private static bool Prefix(ref string text)
      {
        string playerNameText = ControllerHelper.playerNamesManager.playerNameText;
        if (text.StartsWith("//"))
        {
          string str = text.Remove(0, 1);
          OutgoingMessages.SubmitWorldChatMessage(str);
          Globals.chatUI.chatClient.incomingChatMessages.Enqueue(new ChatMessage(str, DateTime.UtcNow, (ChatMessage.ChannelTypes) 0, StaticPlayer.theRealPlayername, StaticPlayer.playerData.playerId, "#" + ControllerHelper.worldController.world.worldName));
          return false;
        }
        if (!text.StartsWith("/"))
          return true;
        ControllerHelper.chatUI.NewMessage(new ChatMessage(text + "</color>", DateTime.Now, (ChatMessage.ChannelTypes) 6, "<#42b3e3>Command</color>", "", Globals.world.worldName));
        if (text.Contains("  "))
        {
          Utils.Error("Prompt can not contain dual Space");
          ControllerHelper.notificationController.MakeNotification((NotificationController.NotificationType) 74, Globals.player.currentPlayerMapPoint);
          return false;
        }
        ChatCommand.Execute(text.Split(' '));
        return false;
      }
    }

    [HarmonyPatch(typeof (ConfigData), "GetBlockRunSpeed")]
    private static class CD_GetBlockRunSpeed
    {
      private static bool Prefix(ref float __result, World.BlockType blockType)
      {
        if ((double) Globals.speedHackSpeed <= 1.8999999761581421 || Globals.glueBlocks.Contains((int) blockType))
          return true;
        __result = Globals.speedHackSpeed;
        return false;
      }
    }

    [HarmonyPatch(typeof (Player), "ActivatePortalInAnimation")]
    private static class Player_ActivatePortalInAnimation
    {
      private static void Postfix(World.BlockType blockType)
      {
        if (BMod.Patches.Patches.shouldUpdateESP)
        {
          BMod.Patches.Patches.shouldUpdateESP = false;
          for (int index1 = 0; index1 < Globals.world.worldSizeY; ++index1)
          {
            for (int index2 = 0; index2 < Globals.world.worldSizeX; ++index2)
            {
              if (Main.IsBlockGemstoneBlock(new Vector2i(index2, index1)))
                Main.gemstonesInWorld.Add(new Vector2i(index2, index1));
              else if (ConfigData.IsBlockGiftBox(Globals.world.GetBlockType(new Vector2i(index2, index1))))
              {
                if (((BSONValue) Globals.world.GetWorldItemData(new Vector2i(index2, index1)).GetAsBSON())["itemAmount"].int32Value > 0)
                  BMod.Patches.Patches.giftBoxes.Add(new Vector2i(index2, index1));
              }
              else if (ConfigData.IsBlockButterflyEventItem(Globals.world.GetBlockType(new Vector2i(index2, index1))))
                Globals.extraESPbutterflies.Add(new Vector2i(index2, index1));
            }
          }
          if (Globals.world.worldName == "THEBLACKTOWER" || Globals.world.worldName == "SECRETBASE" && blockType == 110)
            Globals.discoveredPrizes.Clear();
          if (Globals.shouldShowWelcome)
          {
            Globals.shouldShowWelcome = false;
            ChatUI.SendMinigameMessage("Welcome to BMod v2.0.2");
            ChatUI.SendLogMessage("Creator: ne.kto, Special Thanks: krak, notanab, kumala\nPress F1 to Show/Hide menu, Type /help to view list of commands.\nJoin our discord server: https://discord.gg/7DaEF34tCk \n");
          }
          if (Globals.world.worldName == "NETHERWORLD" || Globals.world.worldName == "SECRETBASE")
          {
            bool flag1 = Globals.world.ContainsBlock((World.BlockType) 1341);
            bool flag2 = Globals.world.ContainsBlock((World.BlockType) 2066);
            string str1 = !flag1 || !(Globals.world.worldName == "NETHERWORLD") ? "No boss found" : "THERE IS A BOSS IN THE NETHERWORLD!";
            string str2 = !flag2 || !(Globals.world.worldName == "NETHERWORLD") ? "No shard/milk found" : "MILK OR SHARD FOUND!";
            if (flag1 | flag2)
            {
              Utils.Msg(str1 + ", " + str2);
              InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("check chat"), "There's smth insteresting");
              InfoPopupUI.ForceShowMenu();
              if (Globals.bossFinder)
              {
                Globals.bossFinder = false;
                Globals.godMode = true;
                ConfigData.playerInactivitySeconds = 9999;
                double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 128, 0.0f, -1);
                Utils.Msg("BossFinder deactivated.");
                Main.bossFinderText = "Boss Finder: OFF";
              }
            }
            else if (Globals.bossFinder)
              SceneLoader.JoinDynamicWorld("NETHERWORLD", "", false);
          }
          if (ACTk.active)
          {
            string str = PlayerIdNameHelper.CombineIdAndName(Globals.playerData.playerId, StaticPlayer.theRealPlayername);
            ILockWorld lockWorldDataHelper = Globals.world.lockWorldDataHelper;
            if (!lockWorldDataHelper.GetPlayersWhoHaveAccessToLock().Contains(str) && !lockWorldDataHelper.GetPlayersWhoHaveMinorAccessToLock().Contains(str) && !(lockWorldDataHelper.GetPlayerWhoOwnsLockId() == Globals.playerData.playerId) && lockWorldDataHelper.GetPlayerWhoOwnsLockName() != "Player(Clone)")
              ACTk.Stop("No WL rights to do that!");
          }
          DiscordManager.Update();
        }
        if (Globals.colorfulNames)
          SummonTimer.Run(0, 0, 1.5f, BSON.SummonTimerAction.PlayAudio);
        if (!MineBot.active || !(Globals.world.worldName == Globals.mineBot.bufferWorld) || Globals.mineBot.state != MineBot.BotState.InBufferWorld)
          return;
        SummonTimer.Run(0, 0, 1f, BSON.SummonTimerAction.BotWait, "BufferWorldJoined");
      }
    }

    [HarmonyPatch(typeof (WorldController), "AdjustGiftBoxAfterUpdate")]
    private static class WC_PrizeChanged
    {
      private static void Postfix(WorldItemBase wib, Vector2i mapPoint)
      {
        if (!(Globals.world.worldName == "THEBLACKTOWER") && !(Globals.world.worldName == "SECRETBASE") || Globals.discoveredPrizes.Contains(mapPoint))
          return;
        Globals.discoveredPrizes.Add(mapPoint);
        BSONObject asBson = Globals.world.GetWorldItemData(mapPoint).GetAsBSON();
        string stringValue1 = ((BSONValue) asBson)["blockType"].stringValue;
        int int32Value1 = ((BSONValue) asBson)["itemInventoryKeyAsInt"].int32Value;
        string stringValue2 = ((BSONValue) asBson)["itemAmount"].stringValue;
        int int32Value2 = ((BSONValue) asBson)["takeAmount"].int32Value;
        PlayerData.InventoryKey inventoryKey = PlayerData.InventoryKey.IntToInventoryKey(int32Value1);
        string blockTypeName = TextManager.GetBlockTypeName(inventoryKey.blockType);
        ChatUI.SendMinigameMessage(string.Format("Item data of {0}({1}) on {2}", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue1, (object) mapPoint));
        if (int32Value2 != 0)
          ChatUI.SendLogMessage(string.Format("{0} ({1}/{2})", (object) blockTypeName, (object) int32Value2, (object) stringValue2));
        else
          ChatUI.SendLogMessage("Gift Box is empty.");
        if (Globals.leakPrizes && inventoryKey.blockType != 2381 && inventoryKey.blockType != 1352)
          Globals.chatUI.Submit(string.Format("Next prize:{0} x{1}", (object) blockTypeName, (object) int32Value2));
      }
    }

    [HarmonyPatch(typeof (ConfigData), "GetBlockGravity")]
    private static class CD_GetBlockGravity
    {
      private static bool Prefix(
        World.BlockType blockType,
        GravityMode gravityMode,
        FPSSetting fpsSetting)
      {
        return !Globals.keysToFly;
      }
    }

    [HarmonyPatch(typeof (NetworkPlayers), "AddNewNetworkPlayer", new Type[] {typeof (BSONObject), typeof (GameObject)})]
    private static class NP_AddNewNetworkPlayer
    {
      private static void Postfix(BSONObject networkPlayerData, GameObject playerPrefab)
      {
        string stringValue = ((BSONValue) networkPlayerData)["U"].stringValue;
        if (!Globals.autoBan || Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff).Contains(stringValue))
          return;
        OutgoingMessages.BanAndKickPlayer(stringValue);
      }
    }
  }
}
