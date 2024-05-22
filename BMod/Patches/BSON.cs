

using BMod.Auto;
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

 
namespace BMod.Patches
{
  internal class BSON
  {
    private static BSONObject _empty;
    internal static bool evil_cows;
    private static List<string> ToCapture;

    private static async void EvilCows()
    {
      await Task.Run((Action) (() =>
      {
        Task.Delay(4000);
        BSON.evil_cows = true;
      }));
    }

    private static bool BounceOrInstaKill(int blockType)
    {
      World.BlockType blockType1 = (World.BlockType) blockType;
      return ConfigData.IsBlockPinball(blockType1) || ConfigData.IsBlockInstakill(blockType1) || ConfigData.IsBlockElastic(blockType1);
    }

    static BSON()
    {
      BSONObject bsonObject = new BSONObject();
      ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("EmptY");
      BSON._empty = bsonObject;
      BSON.evil_cows = false;
      BSON.ToCapture = new List<string>()
      {
        "KPl",
        "WP",
        "WCM",
        "BGM",
        "TTjW",
        "GGBI",
        "PPA",
        "KErr",
        "LW",
        "GetWorldError",
        "RpUo",
        "CPRPa",
        "PAoP",
        "PAiP",
        "AnP",
        "SB",
        "MGA",
        "WIU",
        "MGSt",
        "GWC",
        "BuySkipCraftTime",
        "BIPack",
        "CWeOwC",
        "GPd"
      };
    }

    public enum SummonTimerAction
    {
      Summon,
      WarpOwnerGM,
      AutoFlag,
      PlayAudio,
      BotWait,
      Duper,
    }

    [HarmonyPatch(typeof (NetworkClient), "HandleMessages", new Type[] {typeof (BSONObject)})]
    private static class ClientPacketRecieved
    {
      private static bool Prefix(ref BSONObject messages)
      {
        BSONValue bsonValue1 = (BSONValue) messages;
        bool flag1 = false;
        string str1 = "";
        if (Globals.iCapture.Count > 0)
        {
          BsonDocument bsonDocument = BsonSerializer.Deserialize<BsonDocument>(Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>) SimpleBSON.Dump(messages)), (Action<BsonDeserializationContext.Builder>) null);
          JsonWriterSettings jsonWriterSettings = new JsonWriterSettings();
          jsonWriterSettings.Indent = true;
          BsonSerializationArgs serializationArgs = new BsonSerializationArgs();
          str1 = BsonExtensionMethods.ToJson<BsonDocument>(bsonDocument, jsonWriterSettings, (IBsonSerializer<BsonDocument>) null, (Action<BsonSerializationContext.Builder>) null, serializationArgs);
        }
        if (Globals.iCapture.Count > 0)
        {
          foreach (string str2 in Globals.iCapture)
          {
            if (Globals.iCapture[0] == "all")
            {
              flag1 = true;
              break;
            }
            flag1 = str1.Contains("\"ID\" : \"" + str2 + "\"");
            if (flag1)
              break;
          }
        }
        else
          flag1 = false;
        for (int index1 = 0; index1 < ((BSONValue) messages)["mc"].int32Value; ++index1)
        {
          BSONValue bsonValue2 = ((BSONValue) messages)["m" + index1.ToString()];
          string stringValue1 = bsonValue2["ID"].stringValue;
          if (Globals.iIgnore.Contains(stringValue1))
          {
            ChatUI.SendMinigameMessage("INCOMING BLOCKED | [ID] = \"" + stringValue1 + "\"");
            ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
          }
          else
          {
            bool flag2 = stringValue1 == "p" || stringValue1 == "mP" || stringValue1 == "mc" || stringValue1 == "ST" || stringValue1 == "FiOnAM" || stringValue1 == "FiOffAM" || stringValue1 == "PSicU" || stringValue1 == "AI";
            if (flag1 && (Globals.iCapture.Contains(stringValue1) || Globals.iCapture[0] == "all" && !flag2))
            {
              BsonDocument bsonDocument = BsonSerializer.Deserialize<BsonDocument>(Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>) SimpleBSON.Dump(((Il2CppObjectBase) bsonValue2).Cast<BSONObject>())), (Action<BsonDeserializationContext.Builder>) null);
              JsonWriterSettings jsonWriterSettings = new JsonWriterSettings();
              jsonWriterSettings.Indent = true;
              BsonSerializationArgs serializationArgs = new BsonSerializationArgs();
              string str3 = BsonExtensionMethods.ToJson<BsonDocument>(bsonDocument, jsonWriterSettings, (IBsonSerializer<BsonDocument>) null, (Action<BsonSerializationContext.Builder>) null, serializationArgs).Remove(0, 1);
              string str4 = str3.Remove(str3.LastIndexOf('}'));
              ChatUI.SendMinigameMessage("INCOMING | [ID] = \"" + stringValue1 + "\"");
              Console.WriteLine("ICAPTURE\n" + str4);
              break;
            }
            if (!flag2)
            {
              if (Globals.cmIncoming)
                Console.WriteLine("Incoming ← [ID] = \"" + stringValue1 + "\"");
              if (BSON.ToCapture.Contains(stringValue1))
              {
                if (Globals.handleKick && stringValue1 == "KPl")
                {
                  string str5 = bsonValue2["BPl"].stringValue == "1" ? "banned" : "kicked";
                  string str6 = bsonValue2["BanState"].stringValue == "World" ? "world" : "GAME";
                  if (str6 == "world")
                  {
                    double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 111, 0.0f, -1);
                    Utils.Warning("Oh no, someone " + str5 + " you from the " + str6 + ", you've 5 secs left!");
                  }
                  else
                  {
                    double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 148, 0.0f, -1);
                    Utils.Warning("Oh no, someone " + str5 + " you from the " + str6 + " due to " + bsonValue2["BanFromGameReasonValue"].stringValue + ", you've 5 secs left!");
                  }
                  if (str5 == "banned")
                    Utils.DoCustomNotification("You were banned!", Globals.player.currentPlayerMapPoint);
                  else
                    Utils.DoCustomNotification("You were kicked!", Globals.player.currentPlayerMapPoint);
                  ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                }
                else if (bsonValue2["ID"].stringValue == "WP")
                {
                  if (Globals.handleWP)
                  {
                    double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 116, 0.0f, -1);
                    SummonTimer.Run(BSONValue.op_Implicit(bsonValue2["PX"]), BSONValue.op_Implicit(bsonValue2["PY"]));
                    Utils.DoCustomNotification("You were summoned!", Globals.player.currentPlayerMapPoint);
                    ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                  }
                  List<string> networkPlayers = Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff);
                  List<NetworkPlayer> networkPlayerList = new List<NetworkPlayer>();
                  foreach (string str7 in networkPlayers)
                  {
                    int num = 0;
                    while (index1 < NetworkPlayers.otherPlayers.Count)
                    {
                      if (NetworkPlayers.otherPlayers[num].clientId == str7)
                      {
                        networkPlayerList.Add(NetworkPlayers.otherPlayers[num]);
                        break;
                      }
                      ++num;
                    }
                  }
                  if (networkPlayerList.Count >= 1)
                  {
                    Vector2 vector2_1 = Vector2.op_Implicit(Globals.worldController.ConvertMapPointToWorldPoint(BSONValue.op_Implicit(bsonValue2["PX"]), BSONValue.op_Implicit(bsonValue2["PY"])));
                    Vector2 vector2_2;
                    // ISSUE: explicit constructor call
                    ((Vector2) ref vector2_2).\u002Ector(vector2_1.x, vector2_1.y);
                    List<float> floatList = new List<float>();
                    foreach (NetworkPlayer networkPlayer in networkPlayerList)
                    {
                      float num = Vector3.Distance(networkPlayer.gameObject.transform.position, Vector2.op_Implicit(vector2_2));
                      floatList.Add(num);
                    }
                    short index2 = 0;
                    float num1 = floatList[0];
                    for (short index3 = 0; (int) index3 < floatList.Count; ++index3)
                    {
                      if ((double) num1 > (double) floatList[(int) index3])
                      {
                        num1 = floatList[(int) index3];
                        index2 = index3;
                      }
                    }
                    InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("Summoned"), "Closest: " + networkPlayerList[(int) index2].name);
                    InfoPopupUI.ForceShowMenu();
                  }
                  else
                    continue;
                }
                if (bsonValue2["ID"].stringValue == "BGM")
                {
                  string stringValue2 = bsonValue2["CmB"]["userID"].stringValue;
                  if (Globals.ownerIds.Contains(stringValue2) && bsonValue2["CmB"]["message"].stringValue.StartsWith("allcome"))
                  {
                    double num = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 148, 0.0f, -1);
                    Globals.notificationController.DoLargeNotification((NotificationController.NotificationType) 134, Globals.player.currentPlayerMapPoint);
                    SummonTimer.Run(1, 1, 30f, BSON.SummonTimerAction.WarpOwnerGM, bsonValue2["CmB"]["channel"].stringValue);
                  }
                }
                if (bsonValue2["ID"].stringValue == "GGBI")
                {
                  try
                  {
                    BMod.Patches.Patches.giftBoxes.Remove(new Vector2i(bsonValue2["x"].int32Value, bsonValue2["y"].int32Value));
                  }
                  catch
                  {
                  }
                }
                if (bsonValue2["ID"].stringValue == "PPA")
                {
                  if (Globals.colorfulNames)
                  {
                    if (bsonValue2["audioType"].int32Value == 18 && bsonValue2["audioBlockType"].int32Value == 2871 && !NetworkPlayers.GetNameWithId(bsonValue2["U"].stringValue, false).Contains("<#"))
                    {
                      NetworkPlayers.PlayerNameChanged(bsonValue2["U"].stringValue, "<#ff8000>" + NetworkPlayers.GetNameWithId(bsonValue2["U"].stringValue, false));
                      foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
                      {
                        if (otherPlayer.clientId == bsonValue2["U"].stringValue)
                        {
                          PlayerData myPlayerData = otherPlayer.playerScript.myPlayerData;
                          short countryCode = myPlayerData.countryCode;
                          string str8 = countryCode.ToString();
                          if (countryCode.ToString().Length == 1)
                            str8 = "00" + countryCode.ToString();
                          else if (countryCode.ToString().Length == 2)
                            str8 = "0" + countryCode.ToString();
                          string str9 = "";
                          string str10 = "";
                          if (myPlayerData.clanFaction > 0)
                            str9 = myPlayerData.clanFaction == 1 ? "<#56C6F4>[" + myPlayerData.clanTag + "]" : "<#D868F8>[" + myPlayerData.clanTag + "]";
                          if (Globals.world.lockWorldDataHelper != null)
                          {
                            bool flag3 = Globals.world.lockWorldDataHelper.DoesPlayerHaveAccessToLock(myPlayerData.playerId, Globals.world.clanID != 0 && Globals.world.clanID == myPlayerData.clanId, myPlayerData.clanMemberRank);
                            bool flag4 = Globals.world.lockWorldDataHelper.DoesPlayerHaveMinorAccessToLock(myPlayerData.playerId, Globals.world.clanID != 0 && Globals.world.clanID == myPlayerData.clanId, myPlayerData.clanMemberRank);
                            bool flag5 = Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockId() == myPlayerData.playerId;
                            if (flag3)
                              str10 = "<sprite=\"FlagAtlas\" name=\"rmaj\">";
                            if (flag4)
                              str10 = "<sprite=\"FlagAtlas\" name=\"rmin\">";
                            if (flag5)
                              str10 = "<sprite=\"FlagAtlas\" name=\"rown\">";
                          }
                          ((TMP_Text) otherPlayer.playerScript.playerNameTextMeshPro).text = str10 + "<sprite=\"Flagatlas\" name=\"" + str8 + "\"><#ff8000>" + otherPlayer.name + " " + str9;
                          break;
                        }
                      }
                    }
                  }
                  else if (ACTk.active && bsonValue2["audioType"].int32Value == 20 && BSON.BounceOrInstaKill(bsonValue2["audioBlockType"].int32Value) && !ACTk.CanBypass(bsonValue2["U"].stringValue))
                    ACTk.Punish(bsonValue2["U"].stringValue, "NoBounce | WalkON " + TextManager.GetBlockTypeName((World.BlockType) bsonValue2["audioBlockType"].int32Value));
                }
                if (bsonValue2["ID"].stringValue == "KErr")
                {
                  Globals.invisHack = false;
                  Globals.giveawayMode = false;
                  if (FarmBot.active)
                    FarmBot.Stop(string.Format("Connection Error: {0}", (object) (HardReconnectReason) bsonValue2["ER"].int32Value));
                  if (Globals.ignoreDisconnect)
                  {
                    Utils.Warning("Recieved Disconnect message: " + ((HardReconnectReason) bsonValue2["ER"].int32Value).ToString());
                    return false;
                  }
                  if (AutoASP.active)
                    AutoASP.Stop(string.Format("Connection Error: {0}", (object) (HardReconnectReason) bsonValue2["ER"].int32Value));
                  if (FishBot.active)
                    FishBot.Stop(string.Format("Connection Error: {0}", (object) (HardReconnectReason) bsonValue2["ER"].int32Value));
                }
                else if (bsonValue2["ID"].stringValue == "LW")
                {
                  Globals.invisHack = false;
                  Globals.giveawayMode = false;
                  if (FarmBot.active && !string.IsNullOrWhiteSpace(FarmBot.rejoinEntry))
                  {
                    FarmBot.rejoining = true;
                    FarmBot.outAreaLegal = true;
                    FarmBot.active = false;
                    SceneLoader.CheckIfWeCanGoFromWorldToWorld(Globals.world.worldName, FarmBot.rejoinEntry, Action<WorldJoinResult>.op_Implicit(new Action<WorldJoinResult>(FarmBot.RejoinFailed)), false, (Action<WorldJoinResult>) null);
                    ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                  }
                  else if (FarmBot.active)
                    FarmBot.Stop("LeaveWorld packet received.");
                  if (FishBot.active)
                    FishBot.Stop("LeaveWorld packet received.");
                  if (AutoASP.active)
                    AutoASP.Stop("LeaveWorld packet received.");
                }
                else if (stringValue1 == "GetWorldError" && Globals.bossFinder)
                {
                  ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                  SceneLoader.JoinDynamicWorld("NETHERWORLD", "", false);
                }
                else
                {
                  int num2;
                  switch (stringValue1)
                  {
                    case "TTjW":
                      if (bsonValue2["JR"].int32Value != 0 && Globals.bossFinder)
                      {
                        ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                        Console.WriteLine("Result: " + ((WorldJoinResult) (int) (byte) bsonValue2["JR"].int32Value).ToString());
                        SceneLoader.DynamicWorldJoinRetry();
                        Globals.lagHack = true;
                        BSON.EvilCows();
                        continue;
                      }
                      if (MineBot.active)
                      {
                        Globals.mineBot.HandleJoinWorldResult(((Il2CppObjectBase) bsonValue2).Cast<BSONObject>());
                        continue;
                      }
                      continue;
                    case "RpUo":
                      num2 = Globals.bedHack ? 1 : 0;
                      break;
                    default:
                      num2 = 0;
                      break;
                  }
                  if (num2 != 0)
                    bsonValue2["WiB"]["isPlayerIn"] = BSONValue.op_Implicit(false);
                  else if (stringValue1 == "CPRPa" && Globals.bedHack)
                    ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                  else if (stringValue1 == "CWeOwC")
                  {
                    ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                  }
                  else
                  {
                    if (ACTk.active)
                    {
                      if (stringValue1 == "PAoP" && Globals.world.GetBlockType(new Vector2i(bsonValue2["x"].int32Value, bsonValue2["y"].int32Value)) == 110)
                      {
                        ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                        continue;
                      }
                      if (stringValue1 == "PAiP" && Globals.world.GetBlockType(new Vector2i(bsonValue2["x"].int32Value, bsonValue2["y"].int32Value)) == 110)
                      {
                        Vector2i currentPlayerMapPoint = NetworkPlayers.otherPlayers[0].playerScript.currentPlayerMapPoint;
                        Vector2i point2;
                        // ISSUE: explicit constructor call
                        ((Vector2i) ref point2).\u002Ector(bsonValue2["x"].int32Value, bsonValue2["y"].int32Value);
                        string stringValue3 = bsonValue2["U"].stringValue;
                        Vector2i vector2i = Utils.DistanceMP(currentPlayerMapPoint, point2);
                        if ((vector2i.x > 4 || vector2i.y > 4) && !ACTk.CanBypass(stringValue3))
                        {
                          foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
                          {
                            if (otherPlayer.clientId == stringValue3)
                            {
                              if (ACTk.lc_invis == stringValue3)
                              {
                                ++ACTk.m_invis;
                              }
                              else
                              {
                                ACTk.lc_invis = stringValue3;
                                ACTk.m_invis = 1;
                              }
                            }
                            if (ACTk.m_invis > 3)
                            {
                              ACTk.Punish(stringValue3, string.Format("InvisHack | distance: {0}", (object) vector2i));
                              ACTk.m_invis = 0;
                              break;
                            }
                          }
                        }
                        ((BSONValue) messages)["m" + index1.ToString()] = (BSONValue) BSON._empty;
                        continue;
                      }
                      if (stringValue1 == "AnP" && bsonValue2["inPortal"].boolValue)
                      {
                        float num3 = Vector2.Distance(Vector2.op_Implicit(Globals.worldController.ConvertMapPointToWorldPoint(Globals.world.playerStartPosition)), new Vector2((float) BSONValue.op_Implicit(bsonValue2["x"]), (float) BSONValue.op_Implicit(bsonValue2["y"])));
                        string stringValue4 = bsonValue2["U"].stringValue;
                        if ((double) num3 > 1.3999999761581421 && !ACTk.CanBypass(stringValue4))
                        {
                          ACTk.Punish(stringValue4, string.Format("PortalHack | distance: {0}", (object) Math.Round((double) num3, 2)));
                          continue;
                        }
                        continue;
                      }
                    }
                    int num4;
                    switch (stringValue1)
                    {
                      case "SB":
                        if (ConfigData.IsBlockButterflyEventItem((World.BlockType) bsonValue2["BlockType"].int32Value))
                        {
                          Globals.extraESPbutterflies.Add(new Vector2i(BSONValue.op_Implicit(bsonValue2["x"]), BSONValue.op_Implicit(bsonValue2["y"])));
                          continue;
                        }
                        continue;
                      case "MGA":
                        num4 = FishBot.active ? 1 : 0;
                        break;
                      default:
                        num4 = 0;
                        break;
                    }
                    if (num4 != 0)
                    {
                      if (bsonValue2["MGD"].int32Value == 2)
                        OutgoingMessages.SendFishingGameActionMessage();
                      else if (bsonValue2["MGD"].int32Value == 1)
                        ++FishBot.fails;
                    }
                    else
                    {
                      switch (stringValue1)
                      {
                        case "BIPack":
                          if (AutoASP.active)
                          {
                            AutoASP.HandleItemPackBought(((Il2CppObjectBase) bsonValue2).Cast<BSONObject>());
                            continue;
                          }
                          continue;
                        case "GWC":
                          if (Globals.iCapture.Contains("savemap"))
                          {
                            using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open(AppDomain.CurrentDomain.BaseDirectory + "UserData\\Maps\\" + Globals.world.worldName + ".map", FileMode.Create)))
                              binaryWriter.Write(Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>) bsonValue2["W"].binaryValue));
                            Utils.D("World Saved.");
                            continue;
                          }
                          continue;
                        default:
                          continue;
                      }
                    }
                  }
                }
              }
            }
          }
        }
        return true;
      }
    }

    [HarmonyPatch(typeof (OutgoingMessages), "AddOneMessageToList", new Type[] {typeof (BSONObject)})]
    private static class ClientPacketSent
    {
      private static bool Prefix(BSONObject toAdd)
      {
        string stringValue = ((BSONValue) toAdd)["ID"].stringValue;
        if (Globals.oIgnore.Contains(stringValue))
        {
          ChatUI.SendMinigameMessage("OUTGOING BLOCKED | [ID] = \"" + stringValue + "\"");
          return false;
        }
        if (Globals.cmOutgoing)
          Console.WriteLine("Outgoing → [ID] = \"" + stringValue + "\"");
        int num;
        switch (stringValue)
        {
          case "RWI":
            return !Globals.interactionTroll;
          case "PPA":
            num = Globals.lagHack ? 1 : 0;
            break;
          default:
            num = 0;
            break;
        }
        if (num != 0)
          return false;
        if (stringValue == "PPA" && Globals.antiBounce)
        {
          if (((BSONValue) toAdd)["audioType"].int32Value == 20 && (BSON.BounceOrInstaKill(((BSONValue) toAdd)["audioBlockType"].int32Value) || ConfigData.IsBlockSpring((World.BlockType) ((BSONValue) toAdd)["audioBlockType"].int32Value)))
            return false;
        }
        else if (stringValue == "AdminTeleportMenuOpen")
          return false;
        if (stringValue == "AdminTeleportedTo")
        {
          if ((Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303)) && Input.GetKeyDown((KeyCode) 323))
          {
            Vector2i vector2i;
            // ISSUE: explicit constructor call
            ((Vector2i) ref vector2i).\u002Ector(BSONValue.op_Implicit(((BSONValue) toAdd)["1x"]), BSONValue.op_Implicit(((BSONValue) toAdd)["1y"]));
            Globals.player.WarpPlayer(vector2i.x, vector2i.y);
          }
          else if (Input.GetKeyDown((KeyCode) 323))
            Main.GoToMP(new Vector2i(BSONValue.op_Implicit(((BSONValue) toAdd)["1x"]), BSONValue.op_Implicit(((BSONValue) toAdd)["1y"])));
          return false;
        }
        if (stringValue == "MGA" && FishBot.active && ((BSONValue) toAdd)["LS"].int32Value == 1)
        {
          ((BSONValue) toAdd)["vI"] = BSONValue.op_Implicit(((BSONValue) toAdd)["vI"].int32Value + new Random().Next(2000, 10000));
          ((BSONValue) toAdd)["Idx"] = BSONValue.op_Implicit(((BSONValue) toAdd)["vI"].int32Value + new Random().Next(15000, 20000));
        }
        else if (stringValue == "RtP" && FarmBot.rejoining)
        {
          FarmBot.rejoining = false;
          FarmBot.outAreaLegal = false;
          FarmBot.active = true;
          SummonTimer.Run(0, 0, 3f, BSON.SummonTimerAction.BotWait, "NextLoop");
        }
        else if (stringValue == "Pho")
          return false;
        if (Globals.oCapture.Count > 0 && (Globals.oCapture.Contains(stringValue) || Globals.oCapture[0] == "all"))
        {
          BsonDocument bsonDocument = BsonSerializer.Deserialize<BsonDocument>(Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>) SimpleBSON.Dump(toAdd)), (Action<BsonDeserializationContext.Builder>) null);
          JsonWriterSettings jsonWriterSettings = new JsonWriterSettings();
          jsonWriterSettings.Indent = true;
          BsonSerializationArgs serializationArgs = new BsonSerializationArgs();
          string str1 = BsonExtensionMethods.ToJson<BsonDocument>(bsonDocument, jsonWriterSettings, (IBsonSerializer<BsonDocument>) null, (Action<BsonSerializationContext.Builder>) null, serializationArgs).Remove(0, 2);
          string str2 = str1.Remove(str1.LastIndexOf('}'));
          ChatUI.SendMinigameMessage("OUTGOING | [ID] = \"" + stringValue + "\"");
          Console.WriteLine("OCAPTURE\n" + str2);
        }
        return true;
      }
    }
  }
}
