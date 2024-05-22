
using BMod.Auto;
using BMod.Discord;
using BMod.Patches;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppCodeStage.AntiCheat.ObscuredTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem.Collections.Generic;
using System;
using UnityEngine;

 
namespace BMod
{
  internal class PlayerCheats
  {
    private static float aiAimBotTimer = 0.0f;
    private static float fistCooldownMax = 0.22f;
    private static float autoClaimTimer = 0.0f;
    private static float safeAnnoySoundTimer = 0.0f;
    private static float annoySoundTimer = 0.0f;
    private static float playerAimBotTimer = 0.0f;
    private static float dataSpamTimer = 0.0f;
    public static float dataSpamTimerMax = 0.35f;
    private static float invisTimer = 0.0f;
    private static float giveawayModeTimer = 0.0f;
    private static float autoPlaceTimer = 0.0f;
    public static float submitTimer = 0.0f;
    public static World.BlockType cInventorySelection;

    public static void SetInfDurability(int amount)
    {
      foreach (int pickax in Globals.pickaxes)
      {
        World.BlockType blockType = (World.BlockType) pickax;
        try
        {
          Globals.playerData.GetDurabilityData(blockType).SetDurability(amount);
          Utils.Msg(string.Format("Set durability to {0} points for {1}", (object) amount, (object) blockType));
        }
        catch
        {
        }
      }
      ControllerHelper.notificationController.MakeLargeNotification((NotificationController.NotificationType) 130, ControllerHelper.worldController.player.GetPlayerMapPoint());
    }

    public static void UpdateCheats()
    {
      try
      {
        if (Globals.mouseFly && Input.GetKey((KeyCode) 325))
        {
          try
          {
            Vector2 vector2_1 = new Vector2();
            Vector2 vector2_2 = Vector2.op_Implicit(Camera.main.WorldToScreenPoint(((Component) Globals.player).transform.position));
            vector2_1.x = Input.mousePosition.x - vector2_2.x;
            vector2_1.y = (float) -((double) Input.mousePosition.y - (double) vector2_2.y);
            float num = 100f;
            Vector3 vector3;
            // ISSUE: explicit constructor call
            ((Vector3) ref vector3).\u002Ector(vector2_1.x / num, vector2_1.y / (float) (((double) num - (double) num / 4.0) / -1.0), 0.0f);
            Globals.player.SetVelocity(vector3);
          }
          catch
          {
          }
        }
        if (Globals.freeCam)
        {
          if (Input.GetKey((KeyCode) 273))
          {
            Transform transform = ((Component) Camera.main).transform;
            transform.position = Vector3.op_Addition(transform.position, Vector3.op_Multiply(((Component) Camera.main).transform.up, Globals.freeCamSpeed));
          }
          if (Input.GetKey((KeyCode) 274))
          {
            Transform transform = ((Component) Camera.main).transform;
            transform.position = Vector3.op_Subtraction(transform.position, Vector3.op_Multiply(((Component) Camera.main).transform.up, Globals.freeCamSpeed));
          }
          if (Input.GetKey((KeyCode) 276))
          {
            Transform transform = ((Component) Camera.main).transform;
            transform.position = Vector3.op_Subtraction(transform.position, Vector3.op_Multiply(((Component) Camera.main).transform.right, Globals.freeCamSpeed));
          }
          if (Input.GetKey((KeyCode) 275))
          {
            Transform transform = ((Component) Camera.main).transform;
            transform.position = Vector3.op_Addition(transform.position, Vector3.op_Multiply(((Component) Camera.main).transform.right, Globals.freeCamSpeed));
          }
        }
        if (!Globals.keysToFly || ControllerHelper.playerNamesManager.GetPlayerStatusIconType(Globals.playerData.playerId) != 0)
          return;
        Globals.player.isDoubleJumpFirstJumpDone = true;
        Globals.player.isTripleJumpFirstJumpDone = true;
        Globals.player.isTripleJumpSecondJumpDone = true;
        bool flag = false;
        Vector2.op_Implicit(Globals.player.velocity);
        if (Input.GetKey((KeyCode) 119))
        {
          flag = true;
          Globals.player.SetVelocity(new Vector3(Globals.player.velocity.x, Globals.keysFlySpeed));
        }
        if (Input.GetKey((KeyCode) 115))
        {
          flag = true;
          Globals.player.SetVelocity(new Vector3(Globals.player.velocity.x, -Globals.keysFlySpeed));
        }
        if (Input.GetKey((KeyCode) 97))
        {
          flag = true;
          Globals.player.SetVelocity(new Vector3(-Globals.keysFlySpeed, Globals.player.velocity.y));
        }
        if (Input.GetKey((KeyCode) 100))
        {
          flag = true;
          Globals.player.SetVelocity(new Vector3(Globals.keysFlySpeed, Globals.player.velocity.y));
        }
        if (!flag)
          Globals.player.SetVelocity(new Vector3(0.0f, 0.0f));
        if (!Input.GetKey((KeyCode) 97) && !Input.GetKey((KeyCode) 100))
          Globals.player.SetVelocity(new Vector3(0.0f, Globals.player.velocity.y));
        if (!Input.GetKey((KeyCode) 119) && !Input.GetKey((KeyCode) 115))
          Globals.player.SetVelocity(new Vector3(Globals.player.velocity.x, 0.0f));
      }
      catch
      {
      }
    }

    public static void FixedUpdateCheats()
    {
      try
      {
        if (Globals.fly)
        {
          ConfigData.jumpMinHeight = 0.3f;
          ConfigData.jumpMinHeightRocket = 0.3f;
          ControllerHelper.worldController.player.rocketFuelConsumptionSpeed = ObscuredFloat.op_Implicit(0.0f);
          ControllerHelper.worldController.player.rocketFuelConsumptionSpeed60FPS = ObscuredFloat.op_Implicit(0.0f);
        }
        else
        {
          ConfigData.jumpMinHeight = 0.0f;
          ConfigData.jumpMinHeightRocket = 0.0f;
          ControllerHelper.worldController.player.rocketFuelConsumptionSpeed = ObscuredFloat.op_Implicit(1.15f);
          ControllerHelper.worldController.player.rocketFuelConsumptionSpeed60FPS = ObscuredFloat.op_Implicit(1.29f);
        }
        if (Globals.instaRespawn)
        {
          ConfigData.playerIsDeadBackupTimerCheck = 0.0f;
          Globals.player.portalAnimationSpeed = 50f;
          Globals.player.portalScaleAnimationSpeed = 50f;
        }
        else
        {
          ConfigData.playerIsDeadBackupTimerCheck = 2.5f;
          Globals.player.portalAnimationSpeed = 1f;
          Globals.player.portalScaleAnimationSpeed = 2.5f;
        }
        ConfigData.playerHitOtherPlayerVelocityMax = !Globals.noKnockBack ? 0.3f : 0.0f;
        Globals.player.instakillLayerInt = !Globals.godMode && !Globals.isTeleporting ? 9 : 0;
        if (Globals.invisHack && Object.op_Inequality((Object) Globals.rootUI, (Object) null))
        {
          if (Object.op_Equality((Object) Globals.player, (Object) null))
            Globals.invisHack = false;
          PlayerCheats.invisTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.invisTimer >= 0.20000000298023224)
          {
            PlayerCheats.invisTimer -= 0.2f;
            int num1 = Globals.world.playerStartPosition.x;
            int num2 = Globals.world.playerStartPosition.y;
            if (Globals.customInvis)
            {
              num1 = Globals.invisX;
              num2 = Globals.invisY;
            }
            BSONObject bsonObject1 = new BSONObject();
            ((BSONValue) bsonObject1)["ID"] = BSONValue.op_Implicit("PAiP");
            ((BSONValue) bsonObject1)["x"] = BSONValue.op_Implicit(num1);
            ((BSONValue) bsonObject1)["y"] = BSONValue.op_Implicit(num2);
            OutgoingMessages.AddOneMessageToList(bsonObject1);
            BSONObject bsonObject2 = new BSONObject();
            ((BSONValue) bsonObject2)["ID"] = BSONValue.op_Implicit("PAoP");
            ((BSONValue) bsonObject2)["x"] = BSONValue.op_Implicit(num1);
            ((BSONValue) bsonObject2)["y"] = BSONValue.op_Implicit(num2);
            OutgoingMessages.AddOneMessageToList(bsonObject2);
          }
        }
        if (Globals.autoPlaceWL)
        {
          if (Object.op_Equality((Object) Globals.rootUI, (Object) null))
            Globals.autoPlaceWL = false;
          PlayerCheats.autoPlaceTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.autoPlaceTimer >= 0.05000000074505806)
          {
            PlayerCheats.autoPlaceTimer -= 0.05f;
            OutgoingMessages.SendSetBlockMessage(Globals.player.currentPlayerMapPoint, PlayerCheats.cInventorySelection);
            if (Globals.playerData.HasItemAmountInInventory(ControllerHelper.gameplayUI.inventoryControl.GetCurrentSelection(), (short) 40))
              Globals.playerData.RemoveItemFromInventory(ControllerHelper.gameplayUI.inventoryControl.GetCurrentSelection());
          }
        }
        if (Globals.giveawayMode)
          PlayerCheats.TestCollectNear();
        if (Globals.aiAimBot)
        {
          PlayerCheats.aiAimBotTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.aiAimBotTimer >= (double) PlayerCheats.fistCooldownMax)
          {
            PlayerCheats.aiAimBotTimer -= PlayerCheats.fistCooldownMax;
            Il2CppArrayBase<AIEnemyMonoBehaviourBase> objectsOfType = Object.FindObjectsOfType<AIEnemyMonoBehaviourBase>();
            AIEnemyMonoBehaviourBase monoBehaviourBase1 = (AIEnemyMonoBehaviourBase) null;
            float num3 = 1.5f;
            foreach (AIEnemyMonoBehaviourBase monoBehaviourBase2 in objectsOfType)
            {
              AIEnemyMonoBehaviourBase monoBehaviourBase3 = monoBehaviourBase2;
              if (Object.op_Implicit((Object) monoBehaviourBase3))
              {
                float num4 = Vector3.Distance(monoBehaviourBase2.tempPosition, Globals.player.currentPlayerPosition);
                if ((double) num4 < (double) num3 && AIEnemyConfigData.GetMaxHitPoints(monoBehaviourBase2.aiBase.enemyType) != 50 && AIEnemies.CanPlayerHitAIEnemy(monoBehaviourBase2.aiBase.id))
                {
                  num3 = num4;
                  monoBehaviourBase1 = monoBehaviourBase3;
                }
              }
            }
            if (Object.op_Implicit((Object) monoBehaviourBase1) && monoBehaviourBase1.aiBase.health > 0)
              OutgoingMessages.SendHitAIEnemyMessage(Utils.ConvertWorldPointToMapPoint(Vector2.op_Implicit(monoBehaviourBase1.tempPosition)), monoBehaviourBase1.aiBase.id, 1);
          }
        }
        if (Globals.playerAimBot)
        {
          PlayerCheats.playerAimBotTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.playerAimBotTimer >= (double) PlayerCheats.fistCooldownMax)
          {
            PlayerCheats.playerAimBotTimer -= PlayerCheats.fistCooldownMax;
            List<float> list = new List<float>();
            float num5 = 1.5f;
            List<NetworkPlayer> otherPlayers = NetworkPlayers.otherPlayers;
            foreach (NetworkPlayer networkPlayer in otherPlayers)
            {
              float num6 = Vector3.Distance(networkPlayer.gameObject.transform.position, Globals.player.currentPlayerPosition);
              list.Add(num6);
            }
            short num7 = 0;
            float num8 = list[0];
            for (short index = 0; (int) index < list.Count; ++index)
            {
              if ((double) num8 > (double) list[(int) index])
              {
                num8 = list[(int) index];
                num7 = index;
              }
            }
            if ((double) Vector3.Distance(otherPlayers[(int) num7].gameObject.transform.position, Globals.player.currentPlayerPosition) >= (double) num5)
              return;
            if (Globals.world.IsBattleOn(otherPlayers[(int) num7].playerScript.currentPlayerMapPoint) && Globals.world.IsBattleOn(Globals.player.currentPlayerMapPoint))
              OutgoingMessages.SendHitOtherPlayerMessage(otherPlayers[(int) num7].playerScript.currentPlayerMapPoint, otherPlayers[(int) num7].clientId, -1);
          }
        }
        if (Globals.autoClaimGift)
        {
          PlayerCheats.autoClaimTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.autoClaimTimer >= 0.10000000149011612)
          {
            PlayerCheats.autoClaimTimer -= 0.1f;
            foreach (Vector2i vector2i in Utils.GetMapPointsGridInRange(1))
            {
              if (ConfigData.IsBlockGiftBox(Globals.world.GetBlockType(vector2i)))
                OutgoingMessages.SendRequestItemFromGiftBoxMessage(vector2i);
            }
          }
        }
        if (Globals.safeAnnoySound)
        {
          PlayerCheats.safeAnnoySoundTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.safeAnnoySoundTimer >= 0.40000000596046448)
          {
            PlayerCheats.safeAnnoySoundTimer -= 0.4f;
            int index = new Random().Next(0, Globals.annoyBlocks.Count - 1);
            OutgoingMessages.SendPlayPlayerAudioMessage(18, Convert.ToInt32((object) Globals.annoyBlocks[index]));
            double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 18, Globals.annoyBlocks[index], 0.0f, -1);
          }
        }
        if (Globals.annoySound)
        {
          PlayerCheats.annoySoundTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.annoySoundTimer >= 0.40000000596046448)
          {
            PlayerCheats.annoySoundTimer -= 0.4f;
            int index = new Random().Next(0, Globals.annoyBlocks2.Count - 1);
            OutgoingMessages.SendPlayPlayerAudioMessage(14, Convert.ToInt32((object) Globals.annoyBlocks2[index]));
            double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 14, Globals.annoyBlocks2[index], 0.0f, -1);
          }
        }
        if (Globals.dataSpam)
        {
          PlayerCheats.dataSpamTimer += Time.fixedDeltaTime;
          if ((double) PlayerCheats.dataSpamTimer >= (double) PlayerCheats.dataSpamTimerMax)
          {
            PlayerCheats.dataSpamTimer -= 0.35f;
            ChatCommand.Execute(Globals.textToSpam.Split(' '));
          }
        }
      }
      catch
      {
      }
      if (BSON.evil_cows)
      {
        BSON.evil_cows = false;
        SceneLoader.ReloadWorld();
        Globals.lagHack = false;
      }
      if (FishBot.active && Object.op_Inequality((Object) Globals.player, (Object) null) && Globals.player.fishingState == 0)
        FishBot.SetBait();
      if (FishBot.active && Vector2i.op_Inequality(FishBot.sMP, Globals.player.currentPlayerMapPoint))
        FishBot.Stop("Player position changed");
      if (Globals.colorfulNames && (Globals.handleWP || FarmBot.active || FishBot.active))
        Globals.colorfulNames = false;
      if (Globals.ytmode)
        StaticPlayer.theRealPlayername = Globals.ytmodeName;
      if (!DiscordManager.initialized)
        return;
      DiscordManager.FixedUpdate();
    }

    public static void TestCollectNear()
    {
      PlayerCheats.giveawayModeTimer += Time.fixedDeltaTime;
      if ((double) PlayerCheats.giveawayModeTimer < 0.30000001192092896)
        return;
      PlayerCheats.giveawayModeTimer -= 0.3f;
      List<Collectable> currentCollectables = Globals.worldController.currentCollectables;
      List<CollectableData> list = new List<CollectableData>();
      short num = 0;
      Vector2i currentPlayerMapPoint = Globals.player.currentPlayerMapPoint;
      Vector2i[] vector2iArray = new Vector2i[9]
      {
        Globals.player.currentPlayerLeftMapPoint,
        Globals.player.currentPlayerRightMapPoint,
        Globals.player.currentPlayerAboveMapPoint,
        Globals.player.currentPlayerBelowMapPoint,
        Globals.player.currentPlayerMapPoint,
        new Vector2i(currentPlayerMapPoint.x - 1, currentPlayerMapPoint.y + 1),
        new Vector2i(currentPlayerMapPoint.x + 1, currentPlayerMapPoint.y + 1),
        new Vector2i(currentPlayerMapPoint.x - 1, currentPlayerMapPoint.y - 1),
        new Vector2i(currentPlayerMapPoint.x + 1, currentPlayerMapPoint.y - 1)
      };
      foreach (Collectable collectable in currentCollectables)
        list.Add(collectable.collectableData);
      foreach (CollectableData collectableData in list)
      {
        for (int index = 0; index < vector2iArray.Length; ++index)
        {
          if (Vector2i.op_Equality(collectableData.mapPoint, vector2iArray[index]))
          {
            ++num;
            OutgoingMessages.SendCollectCollectableMessage(collectableData.id);
          }
        }
      }
    }

    internal static void ChangeIK(
      PlayerCheats.DataHackType type,
      string key,
      string value,
      int value2 = 100)
    {
      try
      {
        switch (type)
        {
          case PlayerCheats.DataHackType.Durability:
            PlayerCheats.SetInfDurability(value2);
            break;
          case PlayerCheats.DataHackType.Familiar:
            InventoryItemBase inventoryData1 = Globals.playerData.GetInventoryData(Globals.gameplayUI.inventoryControl.GetCurrentSelection());
            BSONObject asBson1 = inventoryData1.GetAsBSON();
            if (value == "false")
              ((BSONValue) asBson1)[key] = BSONValue.op_Implicit(false);
            if (value == "true")
              ((BSONValue) asBson1)[key] = BSONValue.op_Implicit(true);
            if (value != "false" && value != "true" && value2 == 100)
              ((BSONValue) asBson1)[key] = BSONValue.op_Implicit(value);
            else if (value != "false" && value != "true")
              ((BSONValue) asBson1)[key] = BSONValue.op_Implicit(value2);
            inventoryData1.SetViaBSON(asBson1);
            break;
          case PlayerCheats.DataHackType.Bottle:
            PlayerData.InventoryKey currentSelection = Globals.gameplayUI.inventoryControl.GetCurrentSelection();
            InventoryItemBase inventoryData2 = Globals.playerData.GetInventoryData(currentSelection);
            BSONObject asBson2 = inventoryData2.GetAsBSON();
            if (currentSelection.blockType != 2696 && key == "sendername")
            {
              try
              {
                if (!value.Contains("<size=-"))
                {
                  ((BSONValue) asBson2)["playerName"] = BSONValue.op_Implicit(value);
                  inventoryData2.SetViaBSON(asBson2);
                  break;
                }
                InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("not implemented"), "Not allowed to do that!");
                InfoPopupUI.ForceShowMenu();
                break;
              }
              catch
              {
                Utils.Error("Nab, i can't hack " + TextManager.GetBlockTypeOrSeedName(currentSelection) + ", select correct item from inventory!");
              }
            }
            if (!value.Contains("<size=-"))
            {
              ((BSONValue) asBson2)[key] = BSONValue.op_Implicit(value);
              inventoryData2.SetViaBSON(asBson2);
              break;
            }
            InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("not implemented"), "Not allowed to do that!");
            InfoPopupUI.ForceShowMenu();
            break;
        }
        Utils.Msg("Changed \"" + key + "\" for " + TextManager.GetBlockTypeOrSeedName(Globals.gameplayUI.inventoryControl.GetCurrentSelection()));
      }
      catch
      {
        Utils.Error("Nab, i can't hack " + TextManager.GetBlockTypeOrSeedName(Globals.gameplayUI.inventoryControl.GetCurrentSelection()) + ", select correct item from inventory!");
      }
    }

    internal enum DataHackType
    {
      Durability,
      Familiar,
      Bottle,
      FishTrophy,
      WOTWTrophy,
      CardTrophy,
    }
  }
}
