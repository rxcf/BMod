
using BMod.Auto;
using BMod.Patches;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppCodeStage.AntiCheat.ObscuredTypes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

 
namespace BMod
{
  internal class ChatCommand
  {
    private static BSONObject transmitorData = new BSONObject();

    public static void Execute(string[] args)
    {
      int num1 = ((IEnumerable<string>) args).Count<string>();
      args[0] = args[0].ToLower();
      for (int index = 0; index < args.Length; ++index)
        args[index] = args[index].Replace(',', ' ');
      Vector2i currentPlayerMapPoint = Globals.player.currentPlayerMapPoint;
      Vector2i mapPoint1 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
      string s1 = args[0];
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(s1))
      {
        case 41671447:
          if (s1 == "/tradeall")
          {
            foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
              OutgoingMessages.AskTrade(otherPlayer.clientId);
            return;
          }
          break;
        case 111188871:
          if (s1 == "/ja")
          {
            if (num1 >= 2)
            {
              try
              {
                if (args[1] == "?")
                {
                  ChatUI.SendMinigameMessage("Change Jump Ability\n/ja index \nAbilities:\n0: None\n1: Double\n2: Long\n3: Glide\n4: Rocket\n5: Continuous Jumping\n6: Triple\n7: Flying Mount");
                  return;
                }
                Globals.player.UndressFlyingMountIfNeeded();
                if (int.Parse(args[1]) == 5)
                {
                  if (Globals.player.isContinuesJumpingActivated)
                  {
                    Globals.player.isContinuesJumpingActivated = false;
                    Utils.Msg("Bunny-Hop deactivated");
                    return;
                  }
                  Globals.player.isContinuesJumpingActivated = true;
                  Utils.Msg("Bunny-Hop activated");
                  return;
                }
                Globals.player.jumpMode = (PlayerJumpMode) int.Parse(args[1]);
                Utils.Msg("Jump ability changed for " + Globals.player.jumpMode.ToString());
                return;
              }
              catch
              {
              }
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 124283614:
          if (s1 == "/takedon")
          {
            BSONObject asBson = Globals.world.GetWorldItemData(currentPlayerMapPoint).GetAsBSON();
            short num2 = short.Parse(args[1]);
            BSONObject bsonObject = new BSONObject();
            ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("TakeDon");
            ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
            ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(currentPlayerMapPoint.x);
            ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(currentPlayerMapPoint.y);
            ((BSONValue) bsonObject)["IK"] = ((BSONValue) asBson)["itemsAsInventoryKeys"][(int) num2];
            ((BSONValue) bsonObject)["Amt"] = ((BSONValue) asBson)["itemsAmounts"][(int) num2];
            ((BSONValue) bsonObject)["Idx"] = BSONValue.op_Implicit((int) num2);
            OutgoingMessages.AddOneMessageToList(bsonObject);
            return;
          }
          break;
        case 128805133:
          if (s1 == "/gd")
          {
            try
            {
              BSONObject asBson = Globals.world.GetWorldItemData(currentPlayerMapPoint).GetAsBSON();
              ChatUI.SendMinigameMessage(string.Format("{0}({1}) on {2}\n", (object) Globals.world.GetBlockType(currentPlayerMapPoint), (object) ((BSONValue) asBson)["blockType"].stringValue, (object) currentPlayerMapPoint));
              Utils.Msg(Utils.DumpBSON(asBson));
              return;
            }
            catch
            {
              Globals.notificationController.DoNotification((NotificationController.NotificationType) 119, currentPlayerMapPoint);
              return;
            }
          }
          else
            break;
        case 131942149:
          if (s1 == "/puthere")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Put chosen block to current player positiong (visual)\n/puthere blockId [range]");
                return;
              }
              Globals.world.SetBlock((World.BlockType) Convert.ToInt32(args[1]), currentPlayerMapPoint, "", "", false);
              Globals.worldController.SetBlock((World.BlockType) Convert.ToInt32(args[1]), currentPlayerMapPoint.x, currentPlayerMapPoint.y);
              Utils.Msg(string.Format("Placed {0} on {1}", (object) TextManager.GetBlockTypeName((World.BlockType) Convert.ToInt32(args[1])), (object) currentPlayerMapPoint));
              return;
            }
            if (num1 >= 2)
            {
              foreach (Vector2i vector2i in Utils.GetMapPointsGridInRange(Convert.ToInt32(args[2])))
              {
                Globals.world.SetBlock((World.BlockType) Convert.ToInt32(args[1]), vector2i, "", "", false);
                Globals.worldController.SetBlock((World.BlockType) Convert.ToInt32(args[1]), vector2i.x, vector2i.y);
              }
              Utils.Msg("Filled an area within a " + args[2] + "-cell radius with " + TextManager.GetBlockTypeName((World.BlockType) Convert.ToInt32(args[1])));
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 200382555:
          if (s1 == "/ocapture")
          {
            if (num1 != 2)
              return;
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Capture outgoing packets with certain ids\n/icapture ID\n/icapture reset");
              return;
            }
            if (args[1] == "reset")
            {
              Globals.oCapture.Clear();
              ChatUI.SendMinigameMessage("List cleared.");
              return;
            }
            if (string.IsNullOrEmpty(args[1]))
            {
              Utils.Error("ID cannot be empty.");
              return;
            }
            Globals.oCapture.Add(args[1]);
            string str1 = "";
            foreach (string str2 in Globals.oCapture)
              str1 = str1 + str2 + ", ";
            ChatUI.SendMinigameMessage("Capturing these: " + str1 + "\nAdded new ID: " + args[1]);
            return;
          }
          break;
        case 352925584:
          if (s1 == "/_leakprize")
          {
            Globals.leakPrizes = !Globals.leakPrizes;
            Utils.Msg("Leak Prizes in bt/sb is now: " + Globals.leakPrizes.ToString());
            return;
          }
          break;
        case 377872604:
          if (s1 == "/oblock")
          {
            if (num1 != 2)
              return;
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Ignore outgoing packets with certain ids\n/oblock ID\n/oblock reset");
              return;
            }
            if (args[1].ToLower() == "reset")
            {
              Globals.oIgnore.Clear();
              ChatUI.SendMinigameMessage("List cleared.");
              return;
            }
            if (string.IsNullOrEmpty(args[1]))
            {
              Utils.Error("ID cannot be empty.");
              return;
            }
            Globals.oIgnore.Add(args[1]);
            string str3 = "";
            foreach (string str4 in Globals.oIgnore)
              str3 = str3 + str4 + ", ";
            ChatUI.SendMinigameMessage("Ignoring these: " + str3 + "\nAdded new ID: " + args[1]);
            return;
          }
          break;
        case 409312342:
          if (s1 == "/adddon")
          {
            BSONObject asBson = Globals.world.GetWorldItemData(currentPlayerMapPoint).GetAsBSON();
            int count = ((Il2CppObjectBase) ((BSONValue) asBson)["itemsAmounts"]).Cast<BSONArray>().Count;
            ((BSONValue) asBson)["itemsAsInventoryKeys"].Add(BSONValue.op_Implicit(int.Parse(args[1])));
            ((BSONValue) asBson)["itemsAmounts"].Add(BSONValue.op_Implicit(int.Parse(args[2])));
            ((BSONValue) asBson)["inventoryDatas"]["DatasCount"] = BSONValue.op_Implicit(count + 1);
            ((BSONValue) asBson)["inventoryDatas"][count.ToString()] = ((BSONValue) asBson)["inventoryDatas"]["0"];
            ((BSONValue) asBson)["playerIds"].Add(BSONValue.op_Implicit(Globals.playerData.playerId));
            World world = Globals.world;
            BSONObject bsonObject1 = new BSONObject();
            ((BSONValue) bsonObject1)["ID"] = BSONValue.op_Implicit("WIU");
            ((BSONValue) bsonObject1)["WiB"] = (BSONValue) asBson;
            ((BSONValue) bsonObject1)["x"] = BSONValue.op_Implicit(currentPlayerMapPoint.x);
            ((BSONValue) bsonObject1)["y"] = BSONValue.op_Implicit(currentPlayerMapPoint.y);
            ((BSONValue) bsonObject1)["PT"] = BSONValue.op_Implicit(1);
            ((BSONValue) bsonObject1)["U"] = BSONValue.op_Implicit("LOCALWIU");
            BSONObject bsonObject2 = bsonObject1;
            world.WorldItemUpdate(bsonObject2);
            return;
          }
          break;
        case 422404178:
          if (s1 == "/settransmit")
          {
            ChatCommand.transmitorData = new BSONObject();
            BSONObject asBson = Globals.world.GetWorldItemData(mapPoint1).GetAsBSON();
            if (BSONValue.op_Inequality((BSONValue) asBson, (Object) null))
              ChatCommand.transmitorData = asBson;
            Utils.D("SUCCESS!");
            return;
          }
          break;
        case 426593371:
          if (s1 == "/vname")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Set visual name\n/vname username");
                return;
              }
              ((TMP_Text) Globals.player.playerNameTextMeshPro).text = args[1];
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 686634835:
          if (s1 == "/getrating")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Tries to get player's rating for current world.\n/GetRating username");
              return;
            }
            if (!Globals.world.ContainsBlock((World.BlockType) 293))
            {
              Utils.Error("There is no Rating Board in the world!");
              return;
            }
            if (!string.IsNullOrWhiteSpace(args[1]))
            {
              List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
              if (networkPlayers.Count < 1)
              {
                ChatUI.SendMinigameMessage("No such player was found.");
                return;
              }
              if (networkPlayers.Count > 1)
              {
                ChatUI.SendMinigameMessage("Several players were found, you can pick only one!");
                return;
              }
              if (networkPlayers.Count != 1)
                return;
              string nameWithId = NetworkPlayers.GetNameWithId(networkPlayers[0], false);
              string str5 = networkPlayers[0];
              string str6 = "0";
              foreach (World.RatingPair worldRating in Globals.world.worldRatings)
              {
                if (worldRating.userId == str5)
                {
                  str6 = worldRating.rating.ToString();
                  break;
                }
              }
              string str7 = !(str6 != "0") ? "Hasn't rated the world yet." : "Current rating: " + str6 + " Stars.";
              BluePopupUI.SetPopupValue((PopupMode) 1, "", nameWithId + "'s rating", str7, "Close", "Close", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
              Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 734670263:
          if (s1 == "/minep")
          {
            Globals.world.SetBlock((World.BlockType) 3965, currentPlayerMapPoint, "", "", false);
            Globals.worldController.SetBlock((World.BlockType) 3965, currentPlayerMapPoint.x, currentPlayerMapPoint.y);
            return;
          }
          break;
        case 808785958:
          if (s1 == "/cls")
          {
            Object.FindObjectOfType<ChatUI>().chatMainWindow.Clear();
            return;
          }
          break;
        case 825913993:
          if (s1 == "/mousepos")
          {
            ChatUI.SendMinigameMessage(string.Format("World: {0}, Map: {1}, Screen: {2}", (object) Camera.main.ScreenToWorldPoint(Input.mousePosition), (object) Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)), (object) Input.mousePosition));
            return;
          }
          break;
        case 936001691:
          if (s1 == "/mined")
          {
            WorldItemBase worldItemData = Globals.world.GetWorldItemData(currentPlayerMapPoint);
            BSONObject asBson = worldItemData.GetAsBSON();
            ((BSONValue) asBson)["targetWorldID"] = BSONValue.op_Implicit("MINEWORLD");
            ((BSONValue) asBson)["targetEntryPointID"] = BSONValue.op_Implicit(args[1]);
            worldItemData.SetViaBSON(asBson);
            return;
          }
          break;
        case 1063395862:
          if (s1 == "/bedfk")
          {
            ChatCommand.Execute("/sdm animOn true b");
            ChatCommand.Execute("/sdm isPlayerIn true b");
            ChatCommand.Execute("/sdm playerId AAAAAAAA s");
            ChatCommand.Execute("/wiu");
            return;
          }
          break;
        case 1082913524:
          if (s1 == "/animoff")
          {
            ChatCommand.Execute("/sdm animOn false b");
            ChatCommand.Execute("/wiu");
            return;
          }
          break;
        case 1203277154:
          if (s1 == "/kill")
          {
            if (num1 == 1)
            {
              Globals.player.KillPlayer((World.BlockType) 1340);
              OutgoingMessages.ForceKillPlayerByBlockHit((World.BlockType) 1340, Globals.player.currentPlayerMapPoint, DateTime.UtcNow);
              return;
            }
            if (num1 >= 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Kills a player\n/kill [blockId=1340]");
                ChatUI.SendLogMessage("blockId: 0-default; 111-fire; 3636-electic; 4457-frost.");
                return;
              }
              World.BlockType int32 = (World.BlockType) Convert.ToInt32(args[1]);
              Globals.player.KillPlayer(int32);
              OutgoingMessages.ForceKillPlayerByBlockHit(int32, Globals.player.currentPlayerMapPoint, DateTime.UtcNow);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1297949599:
          if (s1 == "/getlogin")
          {
            WarningPopupUI.SetWarningPopupValues("Safety Warning", "You're about to read ident data of your account.\nDo not share that with third parties or you might get <b>hacked!</b>", "Cancel", "Output", 1, Action.op_Implicit((Action) (() =>
            {
              MelonLogger.Warning("[Do not share this with anyone!]\nCognito: " + UserIdent.CognitoID + "\nLoginKey: " + UserIdent.lastLogin);
              BluePopupUI.SetPopupValue((PopupMode) 1, "", "Output", "Cognito and Login key were sent on the Console", "Close", "", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
              Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
            })), 0);
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<WarningPopupUI>());
            return;
          }
          break;
        case 1310257743:
          if (s1 == "/jmine")
          {
            Globals.mineBot.JoinMine(int.Parse(args[1]));
            return;
          }
          break;
        case 1311500922:
          if (s1 == "/farminfo")
          {
            if (num1 > 1 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Get farmability of selected tile\n/farminfo");
              return;
            }
            PlayerData.InventoryKey currentSelection = ControllerHelper.gameplayUI.inventoryControl.GetCurrentSelection();
            ChatUI.SendMinigameMessage(string.Format("{0}(INACCURATE!)\nFarmability: {1}.", (object) TextManager.GetBlockTypeName(currentSelection.blockType), (object) Math.Round(ConfigData.GetBlockFarmability(currentSelection.blockType), 2)) + "\nTier: " + ConfigData.GetBlockTier(currentSelection.blockType) + string.Format("\nGem drop average: {0}", (object) Math.Round((double) ConfigData.GetBlockGemDropAverage(currentSelection.blockType), 2)) + string.Format("\nDrop seed chance: {0}%", (object) ConfigData.GetBlockDropSeedPercentage(currentSelection.blockType)) + string.Format("\nDrop block chance: {0}%", (object) ConfigData.GetBlockDropBlockPercentage(currentSelection.blockType)) + string.Format("\nDrop gem range: {0}-{1}", (object) ConfigData.GetBlockDropGemRangeMin(currentSelection.blockType), (object) ConfigData.GetBlockDropGemRangeMax(currentSelection.blockType)) + string.Format("\nRecycler points: {0}", (object) ConfigData.GetRecycleValueForRecycler(currentSelection.blockType, (short) 1)));
            return;
          }
          break;
        case 1351110114:
          if (s1 == "/ilogin")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Login into account through ident data (cognito, loginkey)\n/ilogin cognito lk");
                return;
              }
              Utils.Error("Too few arguments.");
              Utils.InvalidInputNotification(args[0]);
              return;
            }
            if (num1 >= 3)
            {
              ChatUI.SendLogMessage("Trying..");
              UserIdent.LogOut();
              UserIdent.SetCognitoIDAndMarkReady(args[1]);
              UserIdent.UpdateLastLogin(args[2]);
              SceneLoader.ReloadGame();
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1365365368:
          if (s1 == "/sdurability")
          {
            if (num1 >= 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Set custom durability to pickaxes\n/sdurability amount");
                return;
              }
              PlayerCheats.ChangeIK(PlayerCheats.DataHackType.Durability, "...", "", Convert.ToInt32(args[1]));
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1450789344:
          if (s1 == "/sdi")
          {
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Changes selected inventory item data\n/sdi Property Value ValueKind\nValue kinds: s (string); i (integer); l (long); b (boolean)");
              return;
            }
            if (num1 >= 2)
            {
              try
              {
                if (args[2].Contains("<size=-"))
                {
                  InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("not implemented"), "Not allowed to do that!");
                  InfoPopupUI.ForceShowMenu();
                  return;
                }
                InventoryItemBase inventoryData = Globals.playerData.GetInventoryData(Globals.gameplayUI.inventoryControl.GetCurrentSelection());
                BSONObject asBson = inventoryData.GetAsBSON();
                Utils.DumpBSON(asBson);
                string lower = args[3].ToLower();
                if (lower == "s" || lower == "str" || lower == "string")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(args[2]);
                else if (lower == "i" || lower == "int" || lower == "integer" || lower == "int32")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt32(args[2]));
                else if (lower == "l" || lower == "int64" || lower == "long")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt64(args[2]));
                else if (lower == "b" || lower == "bool" || lower == "boolean")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToBoolean(args[2]));
                inventoryData.SetViaBSON(asBson);
                ChatUI.SendMinigameMessage("Set [\"" + args[1] + "\"] = " + args[2]);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error("Failed: " + ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 1459561764:
          if (s1 == "/setspammertime")
          {
            PlayerCheats.dataSpamTimerMax = (float) (int.Parse(args[1]) / 1000);
            return;
          }
          break;
        case 1459592875:
          if (s1 == "/login")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("/login username pass");
                return;
              }
              Utils.Error("Too few arguments.");
              Utils.InvalidInputNotification(args[0]);
              return;
            }
            if (num1 >= 3)
            {
              UserIdent.LoginWithUsernameAndPassword(args[1], args[2], false);
              SceneLoader.ReloadGame();
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1462069196:
          if (s1 == "/ytmode")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Changes your username\n/ytmode name");
                return;
              }
              if (!string.IsNullOrWhiteSpace(args[1]))
              {
                BluePopupUI.SetPopupValue((PopupMode) 0, "", "Be Careful!", "Do not submit global/ham radio message or you might get Perma Banned!\nView Player Info to update name.", "I understand", "", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
                Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
                StaticPlayer.theRealPlayername = args[1];
                Globals.ytmode = true;
                Globals.ytmodeName = args[1];
                return;
              }
            }
            Utils.Warning("Use: /ytmode name");
            return;
          }
          break;
        case 1487432844:
          if (s1 == "/countall")
          {
            List<World.BlockType> list = new List<World.BlockType>();
            for (int index1 = 0; index1 < Globals.world.worldSizeY; ++index1)
            {
              for (int index2 = 0; index2 < Globals.world.worldSizeX; ++index2)
              {
                World.BlockType blockType = Globals.world.GetBlockType(index2, index1);
                if (!list.Contains(blockType))
                  list.Add(blockType);
              }
            }
            foreach (World.BlockType blockType in list)
              ChatUI.SendMinigameMessage(string.Format("Found {0} x{1} in the world", (object) TextManager.GetBlockTypeName(blockType), (object) Globals.world.GetBlockCountInWorldFromAnyLayer(blockType)));
            return;
          }
          break;
        case 1517899820:
          if (s1 == "/sdm")
          {
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Set item data on mouse position mappoint\n/sdm Property Value ValueKind\nValue kinds: s (string); i (integer); l (long); b (boolean); a (array)");
              return;
            }
            if (num1 >= 4)
            {
              try
              {
                Vector2i mapPoint2 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                BSONObject asBson = Globals.world.GetWorldItemData(mapPoint2).GetAsBSON();
                string lower = args[3].ToLower();
                if (lower == "s" || lower == "str" || lower == "string")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(args[2]);
                else if (lower == "i" || lower == "int" || lower == "integer" || lower == "int32")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt32(args[2]));
                else if (lower == "l" || lower == "int64" || lower == "long")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt64(args[2]));
                else if (lower == "b" || lower == "bool" || lower == "boolean")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToBoolean(args[2]));
                else if (lower == "a" || lower == "arr" || lower == "array")
                {
                  BSONArray bsonArray = new BSONArray();
                  string str = args[2];
                  char[] chArray = new char[1]{ ' ' };
                  foreach (string s2 in str.Split(chArray))
                  {
                    ((BSONValue) bsonArray).Add(BSONValue.op_Implicit(int.Parse(s2)));
                    ChatUI.SendMinigameMessage("New item added to the array");
                  }
                  ((BSONValue) asBson)[args[1]] = (BSONValue) bsonArray;
                }
                BSONObject bsonObject = new BSONObject();
                ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
                ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
                ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(mapPoint2.x);
                ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(mapPoint2.y);
                ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit(1);
                ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
                Globals.world.WorldItemUpdate(bsonObject);
                ChatUI.SendMinigameMessage("Set [\"" + args[1] + "\"] = " + args[2]);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error("Failed: " + ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 1523709323:
          if (s1 == "/w")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Warp to specified world\n/w world [entryPoint]");
                return;
              }
              SceneLoader.CheckIfWeCanGoFromWorldToWorld(args[1], "", Action<WorldJoinResult>.op_Implicit(new Action<WorldJoinResult>(Globals.chatUI.WarpToWorldFailed)), false, (Action<WorldJoinResult>) null);
              return;
            }
            if (num1 >= 3)
            {
              if (new List<string>()
              {
                "PIXELSTATION",
                "PIXELMINES",
                "DAILYBONUS",
                "TUTORIAL2",
                "LEPRECHAUNCAVE",
                "DAILYQUEST",
                "SUMMERQUEST",
                "THEBLACKTOWER",
                "HALLOWEENCASTLE",
                "NETHERWORLD",
                "DEEPNETHER",
                "SECRETBASE",
                "FORESTHILLS",
                "101011011110",
                "SAECULUM",
                "MINEWORLD",
                "HANGAR",
                "JETRACE",
                "FISHING-101",
                "LOUNGE_SEX",
                "LOUNGE_EN"
              }.Contains(args[1].ToUpper()))
              {
                SceneLoader.JoinDynamicWorld(args[1], args[2], false);
                return;
              }
              SceneLoader.CheckIfWeCanGoFromWorldToWorld(args[1], args[2], (Action<WorldJoinResult>) null, false, (Action<WorldJoinResult>) null);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1551467282:
          if (s1 == "/iblock")
          {
            if (num1 != 2)
              return;
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Ignore incoming packets with certain ids\n/iblock ID\n/iblock reset");
              return;
            }
            if (args[1].ToLower() == "reset")
            {
              Globals.iIgnore.Clear();
              ChatUI.SendMinigameMessage("List cleared.");
              return;
            }
            if (string.IsNullOrEmpty(args[1]))
            {
              Utils.Error("ID cannot be empty.");
              return;
            }
            Globals.iIgnore.Add(args[1]);
            string str8 = "";
            foreach (string str9 in Globals.iIgnore)
              str8 = str8 + str9 + ", ";
            ChatUI.SendMinigameMessage("Ignoring these: " + str8 + "\nAdded new ID: " + args[1]);
            return;
          }
          break;
        case 1588623970:
          if (s1 == "/ikinfo")
          {
            PlayerData.InventoryKey currentSelection = Globals.gameplayUI.inventoryControl.GetCurrentSelection();
            ChatUI.SendMinigameMessage(string.Format("BT: {0}, IT: {1}, IK: {2}", (object) (int) currentSelection.blockType, (object) (int) currentSelection.itemType, (object) PlayerData.InventoryKey.InventoryKeyToInt(currentSelection)));
            return;
          }
          break;
        case 1590819799:
          if (s1 == "/s")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Summon players\n/s [target]\nTargets: f(friends), nf(non-friends), ws(world staff), name");
                return;
              }
              if (args[1].ToLower() == "others")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Others);
                if (networkPlayers.Count > 5)
                {
                  Utils.Error("Can't summon them, too many targets");
                  Globals.notificationController.MakeNotification((NotificationController.NotificationType) 14, currentPlayerMapPoint);
                  return;
                }
                foreach (string str in networkPlayers)
                  OutgoingMessages.SummonPlayerToLocation(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to summon {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "f")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Friends);
                if (networkPlayers.Count > 5)
                {
                  Utils.Error("Can't summon them, too many targets");
                  Globals.notificationController.MakeNotification((NotificationController.NotificationType) 14, currentPlayerMapPoint);
                  return;
                }
                foreach (string str in networkPlayers)
                  OutgoingMessages.SummonPlayerToLocation(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to summon {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "nf")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.NonFriends);
                if (networkPlayers.Count > 5)
                {
                  Utils.Error("Can't summon them, too many targets");
                  Globals.notificationController.MakeNotification((NotificationController.NotificationType) 14, currentPlayerMapPoint);
                  return;
                }
                foreach (string str in networkPlayers)
                  OutgoingMessages.SummonPlayerToLocation(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to summon {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "ws")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff);
                if (networkPlayers.Count > 5)
                {
                  Utils.Error("Can't summon them, too many targets");
                  Globals.notificationController.MakeNotification((NotificationController.NotificationType) 14, currentPlayerMapPoint);
                  return;
                }
                foreach (string str in networkPlayers)
                  OutgoingMessages.SummonPlayerToLocation(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to summon {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (!string.IsNullOrWhiteSpace(args[1]))
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
                if (networkPlayers.Count > 5)
                {
                  Utils.Error("Can't summon them, too many targets");
                  Globals.notificationController.MakeNotification((NotificationController.NotificationType) 14, currentPlayerMapPoint);
                  return;
                }
                foreach (string str in networkPlayers)
                {
                  ChatUI.SendLogMessage("Match found: " + NetworkPlayers.GetNameWithId(str, false));
                  OutgoingMessages.SummonPlayerToLocation(str);
                }
                ChatUI.SendMinigameMessage(networkPlayers.Count >= 1 ? string.Format("Trying to summon {0} players..", (object) networkPlayers.Count) : "Player doesn't exist!");
                return;
              }
            }
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<AdminToolsSummonPlayerUI>());
            return;
          }
          break;
        case 1624375037:
          if (s1 == "/q")
          {
            Environment.Exit(0);
            return;
          }
          break;
        case 1725040751:
          if (s1 == "/k")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Kick players from the world\n/k [target]\nTargets: f(friends), nf(non-friends), ws(world staff), name");
                return;
              }
              if (args[1].ToLower() == "others")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Others);
                foreach (string str in networkPlayers)
                  OutgoingMessages.KickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to kick {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "f")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Friends);
                foreach (string str in networkPlayers)
                  OutgoingMessages.KickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to kick {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "nf")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.NonFriends);
                foreach (string str in networkPlayers)
                  OutgoingMessages.KickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to kick {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "ws")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff);
                foreach (string str in networkPlayers)
                  OutgoingMessages.KickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to kick {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (!string.IsNullOrWhiteSpace(args[1]))
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
                foreach (string str in networkPlayers)
                {
                  ChatUI.SendLogMessage("Match found: " + NetworkPlayers.GetNameWithId(str, false));
                  OutgoingMessages.KickPlayer(str);
                }
                ChatUI.SendMinigameMessage(networkPlayers.Count > 0 ? string.Format("Trying to kick {0} players..", (object) networkPlayers.Count) : "Player doesn't exist!");
                return;
              }
            }
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<AdminToolsKickPlayerUI>());
            return;
          }
          break;
        case 1808928846:
          if (s1 == "/d")
          {
            BluePopupUI.SetPopupValue((PopupMode) 2, "", "Are you sure?", "You're about to eject BMod from the game, continue?", "Confirm", "Cancel", PopupEvent.op_Implicit((Action) (() => MelonBase.FindMelon("BMod", "Nekto").Unregister("Ejected by You", false))), (PopupEvent) null, 0.0f, 0, false, false, false);
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
            ControllerHelper.playerNamesManager.UpdateMyStatusIcon((PlayerNamesManager.StatusIconType) 1);
            return;
          }
          break;
        case 1821864942:
          if (s1 == "/check")
          {
            if (num1 >= 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Query player info with ID\n/check playerId");
                return;
              }
              try
              {
                Globals.rootUI.OnOrOffMenu(Il2CppType.Of<PlayerInfoLiteUI>());
                ((TMP_Text) Object.FindObjectOfType<PlayerInfoLiteUI>().topicText).text = "Player Info";
                OutgoingMessages.QueryPlayerInfo(args[1].ToUpper());
                return;
              }
              catch
              {
                Globals.notificationController.DoNotification((NotificationController.NotificationType) 102, currentPlayerMapPoint);
              }
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1842484084:
          if (s1 == "/b")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Ban players from the world\n/b target\nTargets: f(friends), nf(non-friends), ws(world staff), name");
                return;
              }
              if (args[1].ToLower() == "others")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Others);
                foreach (string str in networkPlayers)
                  OutgoingMessages.BanAndKickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to ban {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "f")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.Friends);
                foreach (string str in networkPlayers)
                  OutgoingMessages.BanAndKickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to ban {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "nf")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.NonFriends);
                foreach (string str in networkPlayers)
                  OutgoingMessages.BanAndKickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to ban {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (args[1].ToLower() == "ws")
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff);
                foreach (string str in networkPlayers)
                  OutgoingMessages.BanAndKickPlayer(str);
                ChatUI.SendMinigameMessage(string.Format("Trying to ban {0} players..", (object) networkPlayers.Count));
                return;
              }
              if (!string.IsNullOrWhiteSpace(args[1]))
              {
                List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
                foreach (string str in networkPlayers)
                {
                  ChatUI.SendLogMessage("Match found: " + NetworkPlayers.GetNameWithId(str, false));
                  OutgoingMessages.BanAndKickPlayer(str);
                }
                ChatUI.SendMinigameMessage(networkPlayers.Count >= 1 ? string.Format("Trying to ban {0} players..", (object) networkPlayers.Count) : "Player doesn't exist!");
                return;
              }
            }
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<AdminToolsBanPlayerFromWorldUI>());
            return;
          }
          break;
        case 1926306050:
          if (s1 == "/smsall")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Send a message to every online friend\n/smsall text");
                return;
              }
              List<FriendSlot> friendList = FriendsUI.friendList;
              int num3 = 0;
              foreach (FriendSlot friendSlot in friendList)
              {
                if (friendSlot.IsOnline())
                {
                  if (num3 < 3)
                  {
                    ++num3;
                    OutgoingMessages.SubmitPrivateChatMessage(((PlayerListsUI.PlayerSlot) friendSlot).GetPlayerID(), args[1]);
                  }
                  else
                    break;
                }
              }
              ChatUI.SendMinigameMessage(string.Format("Message was sent to {0} friends", (object) num3));
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 1951638975:
          if (s1 == "/fbfm")
          {
            ChatUI.SendMinigameMessage("Hours - h; Minutes - m; Seconds - s");
            ChatUI.SendLogMessage("Examples: \n30m\n2h\n4000s");
            return;
          }
          break;
        case 1958832880:
          if (s1 == "/introll")
          {
            Globals.interactionTroll = !Globals.interactionTroll;
            ChatUI.SendMinigameMessage("Interaction Trolling is now: " + Globals.interactionTroll.ToString());
            return;
          }
          break;
        case 1979507232:
          if (s1 == "/gdm")
          {
            try
            {
              Vector2i mapPoint3 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
              BSONObject asBson = Globals.world.GetWorldItemData(mapPoint3).GetAsBSON();
              ChatUI.SendMinigameMessage(string.Format("{0}({1}) on {2}\n", (object) Globals.world.GetBlockType(currentPlayerMapPoint), (object) ((BSONValue) asBson)["blockType"].stringValue, (object) mapPoint3));
              Utils.Msg(Utils.DumpBSON(asBson));
              return;
            }
            catch
            {
              Globals.notificationController.DoNotification((NotificationController.NotificationType) 119, currentPlayerMapPoint);
              return;
            }
          }
          else
            break;
        case 2022361223:
          if (s1 == "/sms")
          {
            foreach (FriendSlot friend in FriendsUI.friendList)
            {
              if (((TMP_Text) friend.nameText).text.ToUpper().StartsWith(args[1].ToUpper()))
                OutgoingMessages.SubmitPrivateChatMessage(((PlayerListsUI.PlayerSlot) friend).GetPlayerID(), args[2]);
            }
            return;
          }
          break;
        case 2046617708:
          if (s1 == "/gdi")
          {
            try
            {
              Utils.Msg(Utils.DumpBSON(Globals.playerData.GetInventoryData(Globals.gameplayUI.inventoryControl.GetCurrentSelection()).GetAsBSON()));
              return;
            }
            catch
            {
              Globals.notificationController.DoNotification((NotificationController.NotificationType) 119, currentPlayerMapPoint);
              return;
            }
          }
          else
            break;
        case 2051721759:
          if (s1 == "/give")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Gives choosen item (visual)\n/give blockId [amount=1]");
                return;
              }
              World.BlockType int32 = (World.BlockType) Convert.ToInt32(args[1]);
              PlayerData.InventoryKey inventoryKey;
              inventoryKey.blockType = int32;
              inventoryKey.itemType = ConfigData.GetBlockTypeInventoryItemType(int32);
              Globals.playerData.AddItemToInventory(inventoryKey, (short) 1, Globals.playerData.GetInventoryData(inventoryKey));
              ChatUI.SendMinigameMessage("Given " + TextManager.GetBlockTypeName(int32) + " x1");
              return;
            }
            if (num1 >= 3)
            {
              World.BlockType int32 = (World.BlockType) Convert.ToInt32(args[1]);
              PlayerData.InventoryKey inventoryKey;
              inventoryKey.blockType = int32;
              inventoryKey.itemType = ConfigData.GetBlockTypeInventoryItemType(int32);
              Globals.playerData.AddItemToInventory(inventoryKey, Convert.ToInt16(args[2]), Globals.playerData.GetInventoryData(inventoryKey));
              ChatUI.SendMinigameMessage("Given " + TextManager.GetBlockTypeName(int32) + " x" + args[2]);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 2092516097:
          if (s1 == "/repeat")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Repeats other player's chat messages\n/repeat [player={Deactivate}]");
              return;
            }
            if (num1 >= 2)
            {
              List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
              if (!string.IsNullOrWhiteSpace(args[1]) && networkPlayers.Count > 0)
              {
                Globals.repeatTrollId = networkPlayers[0];
                Globals.repeatTroll = true;
                ChatUI.SendMinigameMessage("New victim: " + NetworkPlayers.GetNameWithId(Globals.repeatTrollId, false));
                return;
              }
              Utils.Error("Player doesn't exist!");
            }
            Globals.repeatTrollId = "";
            Globals.repeatTroll = false;
            ChatUI.SendMinigameMessage("Repeat Mode is now deactivated.");
            return;
          }
          break;
        case 2105416981:
          if (s1 == "/goto")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Tries to teleport on mappoint\n/goto MapPointX MapPointY");
              return;
            }
            if (num1 < 3)
              return;
            Globals.invisX = Convert.ToInt32(args[1]);
            Globals.invisY = Convert.ToInt32(args[2]);
            Main.GoToMP(new Vector2i(Globals.invisX, Globals.invisY));
            return;
          }
          break;
        case 2171476538:
          if (s1 == "/gem+")
          {
            Dictionary<World.BlockType, int> advancedCount = new Dictionary<World.BlockType, int>();
            Utils.WorldCounter.CountGems(true, out advancedCount);
            string str = "COLLECTABLES IN WORLD\n";
            foreach (World.BlockType key in advancedCount.Keys)
              str += string.Format("{0}: {1}<sprite=\"emj\" name=\"gem\">\n", (object) TextManager.GetBlockTypeName(key), (object) advancedCount[key]);
            Utils.Msg(str + "\n");
            return;
          }
          break;
        case 2223848971:
          if (s1 == "/farmbot")
          {
            if (num1 == 2)
            {
              if (!(args[1] == "?"))
                return;
              ChatUI.SendMinigameMessage("Set custom settings for farm bot\n/farmbot action milliseconds/state\nActions:\nNextStep (default = 900)\nNextHit (Don't change, default = 220)\nValidatePlace (default = true)\n\n/farmbot hits amount (default = 0, sets custom hits amount");
              return;
            }
            if (num1 >= 3)
            {
              if (args[1].ToLower() == "nextstep")
              {
                FarmBot.wait_NextStep = (float) Convert.ToInt32(args[2]) / 1000f;
                Utils.Msg("NextStep delay is now " + FarmBot.wait_NextStep.ToString());
                return;
              }
              if (args[1].ToLower() == "nexthit")
              {
                FarmBot.wait_NextHit = (float) Convert.ToInt32(args[2]) / 1000f;
                Utils.Msg("NextHit delay is now " + FarmBot.wait_NextHit.ToString());
                return;
              }
              if (args[1].ToLower() == "validateplace")
              {
                FarmBot.use_ValidatePlace = !FarmBot.use_ValidatePlace;
                Utils.Msg("ValidatePlace is now " + FarmBot.use_ValidatePlace.ToString());
                return;
              }
              if (args[1].ToLower() == "hits")
              {
                try
                {
                  FarmBot.customHitsAmt = int.Parse(args[2]);
                  if (FarmBot.customHitsAmt > 0)
                  {
                    Utils.Msg("Bot will hit block for " + FarmBot.customHitsAmt.ToString() + " times.");
                    return;
                  }
                  Utils.Msg("Bot will calculate required amount of hits automatically.");
                  return;
                }
                catch
                {
                  FarmBot.customHitsAmt = 0;
                  Utils.Msg("Bot will calculate required amount of hits automatically.");
                  return;
                }
              }
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 2266777947:
          if (s1 == "/autoasp")
          {
            AutoASP.active = !AutoASP.active;
            if (AutoASP.active)
            {
              AutoASP.Start();
              return;
            }
            AutoASP.Stop("", true);
            return;
          }
          break;
        case 2294651940:
          if (s1 == "/sdpotion2")
          {
            if (num1 < 2)
            {
              Utils.Error("Usage: /sdpotion2 itemId");
              return;
            }
            foreach (Vector2i vector2i in Utils.GetMapPointsGridInRange(2))
            {
              if (((Il2CppArrayBase<WorldItemBase>) ((Il2CppArrayBase<Il2CppReferenceArray<WorldItemBase>>) Globals.world.worldItemsData)[vector2i.x])[vector2i.y] != null)
              {
                BSONObject asBson = ((Il2CppArrayBase<WorldItemBase>) ((Il2CppArrayBase<Il2CppReferenceArray<WorldItemBase>>) Globals.world.worldItemsData)[vector2i.x])[vector2i.y].GetAsBSON();
                World.BlockType blockType1 = (World.BlockType) int.Parse(args[1]);
                World.BlockType blockType2 = Globals.world.GetBlockType(vector2i);
                if (blockType2 == 294 || blockType2 == 1125)
                {
                  int num4;
                  if (PotionDatas.GetAllPotionDatas().ContainsKey(blockType1))
                  {
                    Utils.D("Potion found, exploiting..");
                    PotionDatas.PotionData potionData = PotionDatas.GetPotionData(blockType1);
                    ((BSONValue) asBson)["craftingStartTimeInTicks"] = BSONValue.op_Implicit(DateTime.UtcNow.AddHours((double) -potionData.craftTimeInHours).AddSeconds(55.0).Ticks);
                    num4 = PlayerData.InventoryKey.InventoryKeyToInt(new PlayerData.InventoryKey(blockType1, (PlayerData.InventoryItemType) 7));
                  }
                  else if (FamiliarFoodDatas.GetAllFamiliarFoodDatas().ContainsKey(blockType1))
                  {
                    Utils.D("FamFood found, exploiting..");
                    FamiliarFoodDatas.FamiliarFoodData familiarFoodData = FamiliarFoodDatas.GetFamiliarFoodData(blockType1);
                    ((BSONValue) asBson)["craftingStartTimeInTicks"] = BSONValue.op_Implicit(DateTime.UtcNow.AddHours((double) -familiarFoodData.craftTimeInHours).AddSeconds(55.0).Ticks);
                    num4 = PlayerData.InventoryKey.InventoryKeyToInt(new PlayerData.InventoryKey(blockType1, (PlayerData.InventoryItemType) 11));
                  }
                  else
                  {
                    Utils.Error("Not a potion/fam food");
                    break;
                  }
                  ((BSONValue) asBson)["itemBeingCraftedAsBlockType"] = BSONValue.op_Implicit((int) blockType1);
                  BSONObject bsonObject = new BSONObject();
                  ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("BuySkipCraftTime");
                  ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
                  ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(vector2i.x);
                  ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(vector2i.y);
                  ((BSONValue) bsonObject)["IK"] = BSONValue.op_Implicit(num4);
                  OutgoingMessages.AddOneMessageToList(bsonObject);
                  break;
                }
              }
            }
            return;
          }
          break;
        case 2304301357:
          if (s1 == "/reinit")
          {
            foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
              otherPlayer.gameObject.SetActive(false);
            foreach (Component currentCollectable in Globals.worldController.currentCollectables)
              currentCollectable.gameObject.SetActive(false);
            Globals.worldController.ReInit();
            return;
          }
          break;
        case 2331582266:
          if (s1 == "/sdm2")
          {
            if (num1 >= 2)
            {
              try
              {
                Vector2i mapPoint4 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                BSONObject asBson = Globals.world.GetWorldItemData(mapPoint4).GetAsBSON();
                string lower = args[4].ToLower();
                if (lower == "s" || lower == "str" || lower == "string")
                  ((BSONValue) asBson)[args[1]][args[2]] = BSONValue.op_Implicit(args[3]);
                else if (lower == "i" || lower == "int" || lower == "integer" || lower == "int32")
                  ((BSONValue) asBson)[args[1]][args[2]] = BSONValue.op_Implicit(Convert.ToInt32(args[3]));
                else if (lower == "l" || lower == "int64" || lower == "long")
                  ((BSONValue) asBson)[args[1]][args[2]] = BSONValue.op_Implicit(Convert.ToInt64(args[3]));
                else if (lower == "b" || lower == "bool" || lower == "boolean")
                  ((BSONValue) asBson)[args[1]][args[2]] = BSONValue.op_Implicit(Convert.ToBoolean(args[3]));
                BSONObject bsonObject = new BSONObject();
                ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
                ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
                ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(mapPoint4.x);
                ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(mapPoint4.y);
                ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit(1);
                ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
                Globals.world.WorldItemUpdate(bsonObject);
                ChatUI.SendMinigameMessage("Set [\"" + args[1] + "\"] = " + args[2]);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error("Failed: " + ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 2342341902:
          if (s1 == "/cinv")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Set custom mappoint for InvisHack\n/cinv [MapPointX] [MapPointY]");
              return;
            }
            if (num1 >= 3)
            {
              Globals.invisX = Convert.ToInt32(args[1]);
              Globals.invisY = Convert.ToInt32(args[2]);
              Globals.customInvis = true;
              Utils.Msg("Custom invis set for " + TextManager.GetBlockTypeName(Globals.world.GetBlockType(Globals.invisX, Globals.invisY)) + string.Format(" on {0};{1}", (object) Globals.invisX, (object) Globals.invisY));
              return;
            }
            Globals.customInvis = false;
            Utils.Msg("Custom coords for invis deactivated");
            return;
          }
          break;
        case 2348359885:
          if (s1 == "/sdm3")
          {
            if (num1 >= 2)
            {
              try
              {
                Vector2i mapPoint5 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                BSONObject asBson = Globals.world.GetWorldItemData(mapPoint5).GetAsBSON();
                string lower = args[5].ToLower();
                if (lower == "s" || lower == "str" || lower == "string")
                  ((BSONValue) asBson)[args[1]][args[2]][args[3]] = BSONValue.op_Implicit(args[4]);
                else if (lower == "i" || lower == "int" || lower == "integer" || lower == "int32")
                  ((BSONValue) asBson)[args[1]][args[2]][args[3]] = BSONValue.op_Implicit(Convert.ToInt32(args[4]));
                else if (lower == "l" || lower == "int64" || lower == "long")
                  ((BSONValue) asBson)[args[1]][args[2]][args[3]] = BSONValue.op_Implicit(Convert.ToInt64(args[4]));
                else if (lower == "b" || lower == "bool" || lower == "boolean")
                  ((BSONValue) asBson)[args[1]][args[2]][args[3]] = BSONValue.op_Implicit(Convert.ToBoolean(args[4]));
                BSONObject bsonObject = new BSONObject();
                ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
                ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
                ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(mapPoint5.x);
                ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(mapPoint5.y);
                ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit(1);
                ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
                Globals.world.WorldItemUpdate(bsonObject);
                ChatUI.SendMinigameMessage("Set [\"" + args[1] + "\"] = " + args[2]);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error("Failed: " + ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 2555204345:
          if (s1 == "/npc")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Spawns info npc\n/npc index");
                return;
              }
              try
              {
                Globals.world.SetBlock((World.BlockType) 1568, currentPlayerMapPoint, "", "", false);
                Globals.worldController.SetBlock((World.BlockType) 1568, currentPlayerMapPoint.x, currentPlayerMapPoint.y);
                BSONObject bson = new BSONObject();
                ((BSONValue) bson)["class"] = BSONValue.op_Implicit("InfoNPCData");
                ((BSONValue) bson)["itemId"] = BSONValue.op_Implicit(80000);
                ((BSONValue) bson)["blockType"] = BSONValue.op_Implicit(1568);
                ((BSONValue) bson)["animOn"] = BSONValue.op_Implicit(false);
                ((BSONValue) bson)["direction"] = BSONValue.op_Implicit(5);
                ((BSONValue) bson)["anotherSprite"] = BSONValue.op_Implicit(false);
                ((BSONValue) bson)["infoType"] = BSONValue.op_Implicit(int.Parse(args[1]));
                ((BSONValue) bson)["damageNow"] = BSONValue.op_Implicit(false);
                ((BSONValue) bson)["infoNPCVisualLookType"] = BSONValue.op_Implicit(9);
                ((BSONValue) bson)["indexNumber"] = BSONValue.op_Implicit(0);
                Utils.WorldItemUpdated(bson, currentPlayerMapPoint);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error(ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 2583251253:
          if (s1 == "/gem")
          {
            Dictionary<World.BlockType, int> advancedCount = new Dictionary<World.BlockType, int>();
            Dictionary<Utils.WorldCounter.ItemType, int> dictionary = Utils.WorldCounter.CountGems(false, out advancedCount);
            Utils.Msg("GEMS IN WORLD\n" + string.Format("<sprite=\"emj\" name=\"1f48e\">Dropped: {0}", (object) dictionary[Utils.WorldCounter.ItemType.GemDrop]) + string.Format("\n<sprite=\"emj\" name=\"1f41f\">FGems: {0}", (object) dictionary[Utils.WorldCounter.ItemType.FishGems]) + string.Format("\n<sprite=\"emj\" name=\"26cf\">Mgems: {0}", (object) dictionary[Utils.WorldCounter.ItemType.GemstoneGems]) + string.Format("\n<sprite=\"emj\" name=\"1f4b0\">PGems: {0}", (object) dictionary[Utils.WorldCounter.ItemType.PouchGems]) + string.Format("\n<sprite=\"emj\" name=\"1f9fe\">Total: {0}", (object) (dictionary[Utils.WorldCounter.ItemType.FishGems] + dictionary[Utils.WorldCounter.ItemType.GemstoneGems] + dictionary[Utils.WorldCounter.ItemType.PouchGems] + dictionary[Utils.WorldCounter.ItemType.GemDrop])));
            return;
          }
          break;
        case 2595342849:
          if (s1 == "/help")
          {
            ChatUI.SendMinigameMessage("Usage: /command arg1 arg2..\nArguments in [brackets] are optional\nSay in chat: //command args\nView command syntax: /command ?\n\n/kill, /res, /w, /login, /puthere, /smsall, /s, /k, /b, /ilogin,\n/ownall, /vname, /gems, /bc, /dropall, /reinit, /autoflag, /farminfo,\n/mypos, /replace, /world, /find, /playsfx, /_leakprize, /_oldtp, /icapture,\n/ocapture, /countall, /repeat, /tradeall, /support, /pwe, /check, /q,\n/sdurability, /introll, /mousepos /farmbot, /sd, /sdm, /sdi, /ja, /ikinfo\n/gd, /gdm, /gdi, /license, /gem, /cls, /wiu, /animon, /bedfk, /bedfree, /npc,\n/vendor, /ytmode, /animon, /animoff, /getlogin, /sp, /getrating, /cinv, /d,\n/gem+, /lw");
            return;
          }
          break;
        case 2601119830:
          if (s1 == "/dropall")
          {
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Drop specified items from inventory\n/dropall [category]");
              return;
            }
            if (num1 >= 2)
            {
              Il2CppStructArray<PlayerData.InventoryKey> inventoryItemType = Globals.playerData.GetInventoryAsOrderedByInventoryItemType();
              List<PlayerData.InventoryKey> list = new List<PlayerData.InventoryKey>();
              foreach (PlayerData.InventoryKey inventoryKey in (Il2CppArrayBase<PlayerData.InventoryKey>) inventoryItemType)
              {
                if (inventoryKey.itemType.ToString().ToLower().StartsWith(args[1].ToLower()))
                  list.Add(inventoryKey);
              }
              try
              {
                for (int index = 0; index < 5; ++index)
                {
                  short count = Globals.playerData.GetCount(list[index]);
                  Globals.player.DropItems(list[index], count);
                }
                return;
              }
              catch
              {
                return;
              }
            }
            else
            {
              for (int index = 0; index < 5; ++index)
              {
                Il2CppStructArray<PlayerData.InventoryKey> inventoryItemType = Globals.playerData.GetInventoryAsOrderedByInventoryItemType();
                short count = Globals.playerData.GetCount(((Il2CppArrayBase<PlayerData.InventoryKey>) inventoryItemType)[index]);
                Globals.player.DropItems(((Il2CppArrayBase<PlayerData.InventoryKey>) inventoryItemType)[index], count);
              }
              return;
            }
          }
          else
            break;
        case 2650693849:
          if (s1 == "/find")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Find item by name\n/find ItemName\nTip: use ',' except Spaces if search more than 1 word item name.");
              return;
            }
            if (num1 >= 2)
            {
              short num5 = 0;
              List<string> list = new List<string>();
              for (int index = 0; index < Enum.GetNames(typeof (World.BlockType)).Length; ++index)
              {
                list.Add(TextManager.GetBlockTypeName((World.BlockType) index).ToLower());
                if (list[index].Contains(args[1].ToLower()))
                {
                  ChatUI.SendLogMessage(string.Format("Match Found: [{0}] {1}", (object) index, (object) TextManager.GetBlockTypeName((World.BlockType) index)));
                  ++num5;
                }
              }
              if (num5 > (short) 0)
              {
                ChatUI.SendMinigameMessage(num5.ToString() + "items found");
                return;
              }
              ChatUI.SendMinigameMessage("Nothing found.");
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 2740025595:
          if (s1 == "/bedfree")
          {
            ChatCommand.Execute("/sdm isPlayerIn false b");
            ChatCommand.Execute("/wiu");
            return;
          }
          break;
        case 2756388571:
          if (s1 == "/autoflag")
          {
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Copies another player's flag\n/autoflag Player [SayInChat(1/0)=0] [seconds=2]");
              return;
            }
            if (num1 >= 2)
            {
              List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.ShortName, args[1]);
              ChatUI.SendLogMessage("Match Found: " + NetworkPlayers.GetNameWithId(networkPlayers[0], false));
              for (int index = 0; index < NetworkPlayers.otherPlayers.Count; ++index)
              {
                if (NetworkPlayers.otherPlayers[index].clientId == networkPlayers[0])
                {
                  short countryCode = NetworkPlayers.otherPlayers[index].playerScript.myPlayerData.countryCode;
                  UISpriteStorage objectOfType = Object.FindObjectOfType<UISpriteStorage>();
                  Utils.Msg(NetworkPlayers.otherPlayers[index].name + "'s country is: " + objectOfType.GetCountryName(countryCode) + "!");
                  string str = num1 >= 4 ? args[3] : "2";
                  if (!(args[2] == "1") && !(args[2].ToLower() == "true") && !(args[2].ToLower() == "y") && !(args[2].ToLower() == "yes"))
                    break;
                  ChatUI.SendMinigameMessage("Send answer in " + str + " seconds..");
                  SummonTimer.Run(0, 0, (float) Convert.ToInt32(str), BSON.SummonTimerAction.AutoFlag, objectOfType.GetCountryName(countryCode));
                  break;
                }
              }
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 2785657658:
          if (s1 == "/transmit")
          {
            if (BSONValue.op_Equality((BSONValue) ChatCommand.transmitorData, (Object) null))
            {
              Utils.D("Transmition data is null");
              return;
            }
            ((Il2CppArrayBase<WorldItemBase>) ((Il2CppArrayBase<Il2CppReferenceArray<WorldItemBase>>) Globals.world.worldItemsData)[mapPoint1.x])[mapPoint1.y].SetViaBSON(ChatCommand.transmitorData);
            BSONObject bsonObject = new BSONObject();
            ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
            ((BSONValue) bsonObject)["WiB"] = (BSONValue) ChatCommand.transmitorData;
            ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(mapPoint1.x);
            ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(mapPoint1.y);
            ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit(1);
            ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
            Globals.world.WorldItemUpdate(bsonObject);
            Utils.D("SUCCESS!");
            return;
          }
          break;
        case 2842581298:
          if (s1 == "/gems")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Gives visual gems\n/gems +/-/=amount");
                return;
              }
              int int32 = Convert.ToInt32(args[1].Remove(0, 1));
              if (args[1].StartsWith("-"))
              {
                Globals.playerData.RemoveGems(int32);
                return;
              }
              if (args[1].StartsWith("="))
              {
                Globals.playerData.RemoveGems(ObscuredInt.op_Implicit(Globals.playerData.gems));
                Globals.playerData.AddGems(int32);
                return;
              }
              if (args[1].StartsWith("+"))
              {
                Globals.playerData.AddGems(int32);
                return;
              }
              Utils.Error("+/-/= Operator expected");
              Utils.InvalidInputNotification(args[0]);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 2925213045:
          if (s1 == "/ownall")
          {
            short num6 = 0;
            try
            {
              Globals.world.lockWorldDataHelper.SetPlayerWhoOwnsLockId(Globals.playerData.playerId);
              Globals.world.lockWorldDataHelper.SetPlayerWhoOwnsLockName(StaticPlayer.playerName);
              ++num6;
            }
            catch
            {
            }
            foreach (ILockSmall ilockSmall in Globals.world.lockSmallDataHelper)
            {
              ilockSmall.SetPlayerWhoOwnsLockId(Globals.playerData.playerId);
              ilockSmall.SetPlayerWhoOwnsLockName(((Object) Globals.player).name);
              ++num6;
            }
            ChatUI.SendMinigameMessage("Owned " + num6.ToString() + "locks.");
            Globals.notificationController.MakeNotification((NotificationController.NotificationType) 9, currentPlayerMapPoint);
            return;
          }
          break;
        case 3020910537:
          if (s1 == "/wiu")
          {
            Vector2i mapPoint6 = Globals.worldController.ConvertWorldPointToMapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            OutgoingMessages.SendWorldItemUpdateMessage(mapPoint6, Globals.world.GetWorldItemData(mapPoint6), ConfigData.GetToolUsableForBlock(Globals.world.GetBlockType(mapPoint6)));
            Utils.Msg("Packet was sent. Edit " + TextManager.GetBlockTypeName(Globals.world.GetBlockType(mapPoint6)) + " with " + ConfigData.GetToolUsableForBlock(Globals.world.GetBlockType(mapPoint6)).ToString());
            return;
          }
          break;
        case 3065118284:
          if (s1 == "/vendor")
          {
            Globals.world.SetBlock((World.BlockType) 1482, currentPlayerMapPoint, "", "", false);
            Globals.worldController.SetBlock((World.BlockType) 1482, currentPlayerMapPoint.x, currentPlayerMapPoint.y);
            BSONArray bsonArray = new BSONArray();
            for (int index = 0; index < 10; ++index)
              ((BSONValue) bsonArray).Add(BSONValue.op_Implicit(index));
            BSONObject bson = new BSONObject();
            ((BSONValue) bson)["class"] = BSONValue.op_Implicit("VendorNPCData");
            ((BSONValue) bson)["itemId"] = BSONValue.op_Implicit(80000);
            ((BSONValue) bson)["blockType"] = BSONValue.op_Implicit(1482);
            ((BSONValue) bson)["animOn"] = BSONValue.op_Implicit(false);
            ((BSONValue) bson)["direction"] = BSONValue.op_Implicit(5);
            ((BSONValue) bson)["anotherSprite"] = BSONValue.op_Implicit(false);
            ((BSONValue) bson)["damageNow"] = BSONValue.op_Implicit(false);
            ((BSONValue) bson)["vendorNPCVisualLookType"] = BSONValue.op_Implicit(6);
            ((BSONValue) bson)["vendorNPCCatalogs"] = (BSONValue) bsonArray;
            Utils.WorldItemUpdated(bson, currentPlayerMapPoint);
            return;
          }
          break;
        case 3104136938:
          if (s1 == "/mypos")
          {
            ChatUI.SendMinigameMessage(string.Format("World: {0}, Map: {1}", (object) Globals.player.currentPlayerPosition, (object) currentPlayerMapPoint));
            return;
          }
          break;
        case 3217373373:
          if (s1 == "/decompressmap")
          {
            File.Open(AppDomain.CurrentDomain.BaseDirectory + "UserData\\Maps\\" + Globals.world.worldName + ".json", FileMode.Create).Close();
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "UserData\\Maps\\" + Globals.world.worldName + ".json", Utils.DumpBSON(Globals.world.GetEverythingAsBSON()));
            Utils.D("World Decompressed and savet on Pixel Worlds\\UserData\\Maps.");
            return;
          }
          break;
        case 3271216486:
          if (s1 == "/pwe")
          {
            Globals.world.SetBlock((World.BlockType) 1605, currentPlayerMapPoint, "", "", false);
            Globals.worldController.SetBlock((World.BlockType) 1605, currentPlayerMapPoint.x, currentPlayerMapPoint.y);
            return;
          }
          break;
        case 3387738029:
          if (s1 == "/intp")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Gives a list of valid mappoints for teleport to the target\n/intp MapPointX MapPointY");
              return;
            }
            if (num1 < 3)
              return;
            Globals.invisX = Convert.ToInt32(args[1]);
            Globals.invisY = Convert.ToInt32(args[2]);
            List<Vector2i> pathToPortal = Globals.teleport.GetPathToPortal(new Vector2i(Globals.invisX, Globals.invisY));
            if (pathToPortal == null)
              return;
            foreach (Vector2i vector2i in pathToPortal)
              Utils.Msg(string.Format("{0} on: {1}", (object) TextManager.GetBlockTypeName(Globals.world.GetBlockType(vector2i)), (object) vector2i));
            return;
          }
          break;
        case 3388267842:
          if (s1 == "/animon")
          {
            ChatCommand.Execute("/sdm animOn true b");
            ChatCommand.Execute("/wiu");
            return;
          }
          break;
        case 3515991537:
          if (s1 == "/icapture")
          {
            if (num1 != 2)
              return;
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Capture incoming packets with certain ids\n/icapture ID\n/icapture reset");
              return;
            }
            if (args[1] == "reset")
            {
              Globals.iCapture.Clear();
              ChatUI.SendMinigameMessage("List cleared.");
              return;
            }
            if (string.IsNullOrEmpty(args[1]))
            {
              Utils.Error("ID cannot be empty.");
              return;
            }
            Globals.iCapture.Add(args[1]);
            string str10 = "";
            foreach (string str11 in Globals.iCapture)
              str10 = str10 + str11 + ", ";
            ChatUI.SendMinigameMessage("Capturing these: " + str10 + "\nAdded new ID: " + args[1]);
            return;
          }
          break;
        case 3606224413:
          if (s1 == "/ppa")
          {
            double num7 = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) int.Parse(args[1]), (World.BlockType) int.Parse(args[2]), 0.0f, -1);
            OutgoingMessages.SendPlayPlayerAudioMessage(int.Parse(args[1]), int.Parse(args[2]));
            ChatUI.SendMinigameMessage(string.Format("Playing {0} as {1}", (object) (AudioManager.SoundType) int.Parse(args[1]), (object) (World.BlockType) int.Parse(args[2])));
            return;
          }
          break;
        case 3611592288:
          if (s1 == "/_oldtp")
          {
            Globals.useOldTP = !Globals.useOldTP;
            Utils.Msg("Old TP (F12) is now: " + Globals.useOldTP.ToString());
            return;
          }
          break;
        case 3626530415:
          if (s1 == "/playsfx")
          {
            if (num1 == 2 && args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Plays chosen SFX\n/playsfx index");
              return;
            }
            if (num1 >= 2)
            {
              AudioManager.SoundType int32 = (AudioManager.SoundType) Convert.ToInt32(args[1]);
              double num8 = (double) ControllerHelper.audioManager.PlaySFX(int32, 0.0f, -1);
              ChatUI.SendMinigameMessage("Currently playing: " + int32.ToString());
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 3634274802:
          if (s1 == "/world")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Changes world properties (visual)\n/world property index\nProperties: orb, gravity, weather, lighting, name");
                return;
              }
            }
            else if (num1 >= 3)
            {
              switch (args[1])
              {
                case "orb":
                  Globals.world.SetBackgroundType((World.LayerBackgroundType) Convert.ToInt32(args[2]));
                  Globals.worldController.ChangeBackground((World.LayerBackgroundType) Convert.ToInt32(args[2]));
                  Globals.worldController.SetBackground((World.LayerBackgroundType) Convert.ToInt32(args[2]));
                  return;
                case "gravity":
                  Globals.world.SetGravityMode((GravityMode) Convert.ToInt32(args[2]));
                  return;
                case "weather":
                  Globals.world.SetWeatherType((World.WeatherType) Convert.ToInt32(args[2]));
                  Globals.worldController.ChangeWeather((World.WeatherType) Convert.ToInt32(args[2]));
                  return;
                case "lighting":
                  Globals.world.SetLightingType((World.LightingType) Convert.ToInt32(args[2]));
                  Globals.worldController.ChangeLighting((World.LightingType) Convert.ToInt32(args[2]));
                  return;
                case "name":
                  Globals.world.worldName = args[2];
                  return;
                default:
                  return;
              }
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 3733019210:
          if (s1 == "/replace")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Replaces specific blocks in the world with other blocks(visual)\n/replace blockId1 blockId2 range/all");
                return;
              }
            }
            else if (num1 >= 4)
            {
              World.BlockType int32_1 = (World.BlockType) Convert.ToInt32(args[1]);
              World.BlockType int32_2 = (World.BlockType) Convert.ToInt32(args[2]);
              if (args[3].ToLower() == "all")
              {
                for (int index3 = 0; index3 < Globals.world.worldSizeY; ++index3)
                {
                  for (int index4 = 0; index4 < Globals.world.worldSizeX; ++index4)
                  {
                    Vector2i vector2i;
                    // ISSUE: explicit constructor call
                    ((Vector2i) ref vector2i).\u002Ector(index4, index3);
                    if (Globals.world.GetBlockType(vector2i) == int32_1)
                    {
                      Globals.world.SetBlock(int32_2, vector2i, "", "", false);
                      Globals.worldController.SetBlock(int32_2, vector2i.x, vector2i.y);
                    }
                  }
                }
                return;
              }
              foreach (Vector2i vector2i in Utils.GetMapPointsGridInRange(Convert.ToInt32(args[3])))
              {
                if (Globals.world.GetBlockType(vector2i) == int32_1)
                {
                  Globals.world.SetBlock((World.BlockType) Convert.ToInt32((object) int32_2), vector2i, "", "", false);
                  Globals.worldController.SetBlock((World.BlockType) Convert.ToInt32((object) int32_2), vector2i.x, vector2i.y);
                }
              }
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 3739566597:
          if (s1 == "/setspammer")
          {
            Globals.textToSpam = args[1];
            return;
          }
          break;
        case 3756153086:
          if (s1 == "/makefam2")
          {
            if (Globals.world.GetBlockType(currentPlayerMapPoint) != 1126)
            {
              Utils.Error("Not an evolverator!");
              return;
            }
            PlayerData.InventoryKey inventoryKey;
            inventoryKey.blockType = (World.BlockType) int.Parse(args[1]);
            inventoryKey.itemType = (PlayerData.InventoryItemType) 10;
            short num9 = short.Parse(args[2]);
            BSONObject asBson = Globals.world.GetWorldItemData(currentPlayerMapPoint).GetAsBSON();
            if (Familiars.familiars.ContainsKey(inventoryKey.blockType))
            {
              Utils.D("Familiar found, exploiting..");
              Familiars.FamiliarData familiarData = Familiars.GetFamiliarData(inventoryKey.blockType);
              ((BSONValue) asBson)["evolveStartTimeInTicks"] = BSONValue.op_Implicit(DateTime.UtcNow.AddHours((double) -familiarData.evolveTimeInHours).AddSeconds(55.0).Ticks);
              ((BSONValue) asBson)["familiarBeingEvolvedAsBlockType"] = BSONValue.op_Implicit((int) inventoryKey.blockType);
              ((BSONValue) asBson)["itemInventoryKeyAsInt"] = BSONValue.op_Implicit(PlayerData.InventoryKey.InventoryKeyToInt(inventoryKey));
              BSONObject bsonObject3 = asBson;
              BSONObject bsonObject4 = new BSONObject();
              ((BSONValue) bsonObject4)["inventoryClass"] = BSONValue.op_Implicit(inventoryKey.blockType.ToString() + "InventoryData");
              ((BSONValue) bsonObject4)["inventoryBlockType"] = BSONValue.op_Implicit(int.Parse(args[1]));
              ((BSONValue) bsonObject4)["name"] = BSONValue.op_Implicit("");
              ((BSONValue) bsonObject4)["isNameChanged"] = BSONValue.op_Implicit(false);
              BSONObject bsonObject5 = bsonObject4;
              DateTime dateTime = DateTime.UtcNow;
              dateTime = dateTime.AddDays((double) -num9);
              BSONValue bsonValue = BSONValue.op_Implicit(dateTime.Ticks);
              ((BSONValue) bsonObject5)["dateOfBirthTicks"] = bsonValue;
              ((BSONValue) bsonObject4)["level"] = BSONValue.op_Implicit(1);
              ((BSONValue) bsonObject4)["foodEaten"] = BSONValue.op_Implicit(0);
              BSONObject bsonObject6 = bsonObject4;
              ((BSONValue) bsonObject3)["inventoryData"] = (BSONValue) bsonObject6;
              BSONObject bsonObject7 = new BSONObject();
              ((BSONValue) bsonObject7)["ID"] = BSONValue.op_Implicit("BuySkipCraftTime");
              ((BSONValue) bsonObject7)["WiB"] = (BSONValue) asBson;
              ((BSONValue) bsonObject7)["x"] = BSONValue.op_Implicit(currentPlayerMapPoint.x);
              ((BSONValue) bsonObject7)["y"] = BSONValue.op_Implicit(currentPlayerMapPoint.y);
              ((BSONValue) bsonObject7)["IK"] = BSONValue.op_Implicit(PlayerData.InventoryKey.InventoryKeyToInt(inventoryKey));
              OutgoingMessages.AddOneMessageToList(bsonObject7);
              return;
            }
            Utils.Error("Not a familiar");
            return;
          }
          break;
        case 3783802669:
          if (s1 == "/support")
          {
            Globals.chatUI.Submit("/w NEBOSS");
            return;
          }
          break;
        case 3784795884:
          if (s1 == "/nt")
          {
            ControllerHelper.networkClient.HandleBanLimitExceededMessage(new BSONObject());
            return;
          }
          break;
        case 3835422931:
          if (s1 == "/lw")
          {
            BSONObject messages = new BSONObject();
            ((BSONValue) messages)["ID"] = BSONValue.op_Implicit("LW");
            Utils.SimulateMessages(messages);
            return;
          }
          break;
        case 3952027621:
          if (s1 == "/sp")
          {
            OutgoingMessages.recentMapPoints.Add(Globals.world.playerStartPosition);
            BSONObject bsonObject = new BSONObject();
            ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("PAiP");
            ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(Globals.world.playerStartPosition.x);
            ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(Globals.world.playerStartPosition.y);
            OutgoingMessages.AddOneMessageToList(bsonObject);
            ((Component) Globals.player).transform.position = Globals.worldController.ConvertMapPointToWorldPoint(Globals.world.playerStartPosition);
            return;
          }
          break;
        case 4055198911:
          if (s1 == "/dupew")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Makes a world with similiar world name\n/dupew world");
                return;
              }
              SceneLoader.GoFromWorldToWorld(args[1] + "\n", "default");
              return;
            }
            if (num1 >= 3)
            {
              SceneLoader.CheckIfWeCanGoFromWorldToWorld(args[1] + "\n", args[2], (Action<WorldJoinResult>) null, false, (Action<WorldJoinResult>) null);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 4086602880:
          if (s1 == "/res")
          {
            OutgoingMessages.SendRespawn(DateTime.UtcNow, true);
            return;
          }
          break;
        case 4153359049:
          if (s1 == "/sd")
          {
            if (args[1] == "?")
            {
              ChatUI.SendMinigameMessage("Set item data on current mappoint\n/sd Property Value ValueKind\nValue kinds: s (string); i (integer); l (long); b (boolean)");
              return;
            }
            if (num1 >= 2)
            {
              try
              {
                BSONObject asBson = Globals.world.GetWorldItemData(currentPlayerMapPoint).GetAsBSON();
                string lower = args[3].ToLower();
                if (lower == "s" || lower == "str" || lower == "string")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(args[2]);
                else if (lower == "i" || lower == "int" || lower == "integer" || lower == "int32")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt32(args[2]));
                else if (lower == "l" || lower == "int64" || lower == "long")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToInt64(args[2]));
                else if (lower == "b" || lower == "bool" || lower == "boolean")
                  ((BSONValue) asBson)[args[1]] = BSONValue.op_Implicit(Convert.ToBoolean(args[2]));
                BSONObject bsonObject = new BSONObject();
                ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
                ((BSONValue) bsonObject)["WiB"] = (BSONValue) asBson;
                ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(currentPlayerMapPoint.x);
                ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(currentPlayerMapPoint.y);
                ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit(1);
                ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
                Globals.world.WorldItemUpdate(bsonObject);
                ChatUI.SendMinigameMessage("Set [\"" + args[1] + "\"] = " + args[2]);
                return;
              }
              catch (Exception ex)
              {
                Utils.Error("Failed: " + ex.Message);
                return;
              }
            }
            else
            {
              Utils.InvalidInputNotification(args[0]);
              return;
            }
          }
          else
            break;
        case 4172549429:
          if (s1 == "/bc")
          {
            if (num1 == 2)
            {
              if (args[1] == "?")
              {
                ChatUI.SendMinigameMessage("Gives visual bc\n/bc +/-/=amount");
                return;
              }
              int int32 = Convert.ToInt32(args[1].Remove(0, 1));
              if (args[1].StartsWith("-"))
              {
                Globals.playerData.RemoveByteCoins(int32);
                return;
              }
              if (args[1].StartsWith("="))
              {
                Globals.playerData.RemoveByteCoins(ObscuredInt.op_Implicit(Globals.playerData.bc));
                Globals.playerData.AddByteCoins(int32);
                return;
              }
              if (args[1].StartsWith("+"))
              {
                Globals.playerData.AddByteCoins(int32);
                return;
              }
              Utils.Error("+/-/= Operator expected");
              Utils.InvalidInputNotification(args[0]);
              return;
            }
            Utils.InvalidInputNotification(args[0]);
            return;
          }
          break;
        case 4232298968:
          if (s1 == "/donfuck")
          {
            OutgoingMessages.SendAddItemToDonationBox(currentPlayerMapPoint, Globals.world.GetWorldItemData(currentPlayerMapPoint), new PlayerData.InventoryKey((World.BlockType) 2275), -777, (InventoryItemBase) null);
            return;
          }
          break;
      }
      ControllerHelper.notificationController.MakeNotification((NotificationController.NotificationType) 74, Globals.player.currentPlayerMapPoint);
    }

    public static void Execute(string input) => ChatCommand.Execute(input.Split(' '));
  }
}
