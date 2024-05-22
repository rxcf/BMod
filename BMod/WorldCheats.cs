
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppKernys.Bson;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 
namespace BMod
{
  internal class WorldCheats
  {
    public static List<string> playersWhoHaveAccessToLockWorld = new List<string>();
    public static List<string> playersWhoHaveMinorAccessToLockWorld = new List<string>();
    private static string textToShow = "None";

    public static void QueryPlayers()
    {
      if (NetworkPlayers.otherPlayers.Count > 0)
      {
        foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
          ChatUI.SendLogMessage(string.Format("{0} ({1}), Lvl: {2}", (object) otherPlayer.name, (object) otherPlayer.clientId, (object) otherPlayer.playerScript.myPlayerData.xpLevel));
      }
      else
        ControllerHelper.notificationController.MakeLargeNotification((NotificationController.NotificationType) 63, ControllerHelper.worldController.player.GetPlayerMapPoint());
    }

    public static void QueryWLData()
    {
      string str1 = Globals.world.lockWorldDataHelper.GetBlockType().ToString();
      string playerWhoOwnsLockName = Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockName();
      string playerWhoOwnsLockId = Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockId();
      WorldCheats.playersWhoHaveAccessToLockWorld = Globals.world.lockWorldDataHelper.GetPlayersWhoHaveAccessToLock();
      WorldCheats.playersWhoHaveMinorAccessToLockWorld = Globals.world.lockWorldDataHelper.GetPlayersWhoHaveMinorAccessToLock();
      bool isOpen = Globals.world.lockWorldDataHelper.GetIsOpen();
      bool isPunchingAllowed = Globals.world.lockWorldDataHelper.GetIsPunchingAllowed();
      bool isBattleOn = Globals.world.lockWorldDataHelper.GetIsBattleOn();
      DateTime lastActivatedTime = Globals.world.lockWorldDataHelper.GetLastActivatedTime();
      ChatUI.SendMinigameMessage("============================================================================");
      ChatUI.SendLogMessage(string.Format("{0} ({1}) owned by {2} ({3}) [Last Active on {4}]", (object) Globals.world.worldName, (object) Globals.world.worldID, (object) playerWhoOwnsLockName, (object) playerWhoOwnsLockId, (object) lastActivatedTime));
      ChatUI.SendMinigameMessage(string.Format("There are {0} players who got Full access to the world lock", (object) WorldCheats.playersWhoHaveAccessToLockWorld.Count));
      foreach (string str2 in WorldCheats.playersWhoHaveAccessToLockWorld)
        ChatUI.SendLogMessage(str2);
      ChatUI.SendMinigameMessage(string.Format("There are {0} players who got Minor access to the world lock", (object) WorldCheats.playersWhoHaveMinorAccessToLockWorld.Count));
      foreach (string str3 in WorldCheats.playersWhoHaveMinorAccessToLockWorld)
        ChatUI.SendLogMessage(str3);
      ChatUI.SendMinigameMessage(str1 + " Properties:");
      ChatUI.SendLogMessage("[" + (isOpen ? "Lock is OPEN" : "Lock is Closed") + "|" + (isPunchingAllowed ? "Hitting Allowed" : "Hitting Forbidden") + "|" + (isBattleOn ? "Battle Active" : "Battle Inactive") + "]");
      ChatUI.SendMinigameMessage("============================================================================");
    }

    public static void ScanForHackers()
    {
      if (NetworkPlayers.otherPlayers.Count > 0)
      {
        short num = 0;
        ChatUI.SendMinigameMessage(string.Format("Scanning {0} players in {1}", (object) NetworkPlayers.otherPlayers.Count, (object) Globals.world.worldName));
        foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
        {
          if (otherPlayer.playerScript.myPlayerData.GetWornWeaponDurability() > 10000)
          {
            ++num;
            Utils.Msg(string.Format("{0} has {1} durability on their pickaxe!", (object) otherPlayer.name, (object) otherPlayer.playerScript.myPlayerData.GetWornWeaponDurability()));
          }
        }
        if (num > (short) 0)
          ChatUI.SendMinigameMessage(string.Format("Found {0} cheaters in this world.", (object) num));
        else
          ChatUI.SendMinigameMessage("No cheaters were found.");
      }
      else
        ControllerHelper.notificationController.MakeNotification((NotificationController.NotificationType) 59, Globals.player.currentPlayerMapPoint);
    }

    private static void ShowTooltipBox(string text)
    {
      int num1 = text.Count<char>((Func<char, bool>) (s => s == '\n'));
      text.Split('\n');
      string source = ((IEnumerable<string>) text.Split('\n')).OrderByDescending<string, int>((Func<string, int>) (s => s.Length)).First<string>();
      int length = source.Length;
      float num2 = (float) ((double) source.Count<char>((Func<char, bool>) (c => char.IsUpper(c))) * 12.0 + (double) source.Count<char>((Func<char, bool>) (c => char.IsLower(c))) * 9.0 + (double) source.Count<char>((Func<char, bool>) (c => char.IsWhiteSpace(c))) * 6.0 + (double) source.Count<char>((Func<char, bool>) (c => char.IsSymbol(c))) * 8.0 + (double) source.Count<char>((Func<char, bool>) (c => char.IsNumber(c))) * 8.0 + (double) source.Count<char>((Func<char, bool>) (c => char.IsPunctuation(c))) * 9.0);
      GUIStyle guiStyle = new GUIStyle()
      {
        richText = true,
        alignment = (TextAnchor) 0,
        fontSize = 16,
        fontStyle = (FontStyle) 1
      };
      guiStyle.normal.textColor = Color.white;
      float num3 = num2;
      float num4 = (float) num1 * 19f;
      float num5 = (float) num1 * 20f;
      if ((double) num3 > (double) Screen.width || (double) num4 > (double) Screen.height)
      {
        GUI.Box(new Rect(3f, 0.0f, (float) Screen.width, (float) (Screen.height / 2)), "");
        GUI.Label(new Rect(0.0f, 0.0f, (float) Screen.width, (float) (Screen.height / 2)), text, guiStyle);
      }
      else if ((double) Screen.width - (double) Input.mousePosition.x <= (double) num3 && (double) Screen.height - (double) Input.mousePosition.y > (double) Screen.height - (double) num5)
      {
        GUI.Box(new Rect(Input.mousePosition.x - num3, (float) Screen.height - Input.mousePosition.y - num5, num3, num4), "");
        GUI.Label(new Rect(Input.mousePosition.x + 3f - num3, (float) Screen.height - Input.mousePosition.y - num5, num3, num5), text, guiStyle);
      }
      else if ((double) Screen.width - (double) Input.mousePosition.x <= (double) num3)
      {
        GUI.Box(new Rect(Input.mousePosition.x - num3, (float) ((double) Screen.height - (double) Input.mousePosition.y + 25.0), num3, num4), "");
        GUI.Label(new Rect(Input.mousePosition.x + 3f - num3, (float) ((double) Screen.height - (double) Input.mousePosition.y + 28.0), num3, num5), text, guiStyle);
      }
      else if ((double) Screen.height - (double) Input.mousePosition.y > (double) Screen.height - (double) num5)
      {
        GUI.Box(new Rect(Input.mousePosition.x, (float) Screen.height - Input.mousePosition.y - num5, num3, num4), "");
        GUI.Label(new Rect(Input.mousePosition.x + 3f, (float) Screen.height - Input.mousePosition.y - num5, num3, num5), text, guiStyle);
      }
      else
      {
        GUI.Box(new Rect(Input.mousePosition.x, (float) ((double) Screen.height - (double) Input.mousePosition.y + 25.0), num3, num4), "");
        GUI.Label(new Rect(Input.mousePosition.x + 3f, (float) ((double) Screen.height - (double) Input.mousePosition.y + 28.0), num3, num5), text, guiStyle);
      }
    }

    public static void QueryWorldItemData(Vector2i mapPoint)
    {
      if (HotkeyEvent.CtrlPressed && HotkeyEvent.ShiftPressed)
      {
        WorldCheats.textToShow = string.Format("{0} | {1} ({2})\n", (object) (int) Globals.world.GetBlockType(mapPoint), (object) Globals.world.GetBlockType(mapPoint), (object) mapPoint);
        WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
      }
      else
      {
        WorldItemBase worldItemData;
        BSONObject asBson;
        try
        {
          worldItemData = Globals.world.GetWorldItemData(mapPoint);
          asBson = worldItemData.GetAsBSON();
        }
        catch
        {
          return;
        }
        if (worldItemData == null)
          return;
        if (HotkeyEvent.AltPressed)
        {
          WorldCheats.textToShow = Utils.DumpBSON(asBson);
          WorldCheats.textToShow = WorldCheats.textToShow.Remove(0, 1);
          WorldCheats.textToShow = WorldCheats.textToShow.Remove(WorldCheats.textToShow.Length - 1);
          WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
        }
        else
        {
          foreach (string portalsDataClass in Globals.portalsDataClasses)
          {
            if (((BSONValue) asBson)["class"].stringValue == portalsDataClass)
            {
              string stringValue1 = ((BSONValue) asBson)["blockType"].stringValue;
              string stringValue2 = ((BSONValue) asBson)["name"].stringValue;
              string str1 = "W: " + Globals.world.worldName + " ";
              string stringValue3 = ((BSONValue) asBson)["entryPointID"].stringValue;
              string stringValue4 = ((BSONValue) asBson)["targetWorldID"].stringValue;
              string stringValue5 = ((BSONValue) asBson)["targetEntryPointID"].stringValue;
              string stringValue6 = ((BSONValue) asBson)["isLocked"].stringValue;
              string str2 = stringValue3 == "" ? "" : "P:" + stringValue3 + " ";
              string str3 = stringValue4 == "" ? "W:#None " : "W:" + stringValue4 + " ";
              string str4 = stringValue5 == "" ? "" : "P:" + stringValue5;
              string str5 = stringValue6 == "true" ? "LOCKED" : "Open!";
              WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue1, (object) mapPoint) + "Name: " + stringValue2 + "\n" + str1 + str2 + ">> " + str3 + str4 + "\nThe item is now " + str5 + "\n(Hold shift + click to join entry)\n";
              if (Input.GetKey((KeyCode) 304) && Input.GetKeyDown((KeyCode) 323))
              {
                if (!string.IsNullOrWhiteSpace(str2))
                  SceneLoader.CheckIfWeCanGoFromWorldToWorld(Globals.world.worldName, ((BSONValue) asBson)["entryPointID"].stringValue, (Action<WorldJoinResult>) null, false, (Action<WorldJoinResult>) null);
                else
                  Globals.notificationController.DoNotification((NotificationController.NotificationType) 63, mapPoint);
              }
              WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
              return;
            }
          }
          foreach (string signsDataClass in Globals.signsDataClasses)
          {
            if (((BSONValue) asBson)["class"].stringValue == signsDataClass)
            {
              string stringValue7 = ((BSONValue) asBson)["text"].stringValue;
              string stringValue8 = ((BSONValue) asBson)["blockType"].stringValue;
              WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue8, (object) mapPoint);
              WorldCheats.textToShow = string.IsNullOrEmpty(stringValue7) ? WorldCheats.textToShow + "Item text is empty\n" : WorldCheats.textToShow + "Item text:\n" + stringValue7 + "\n";
              WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
              return;
            }
          }
          foreach (string advancedDataClass in Globals.signsAdvancedDataClasses)
          {
            if (((BSONValue) asBson)["class"].stringValue == advancedDataClass)
            {
              List<string> stringListValue = ((BSONValue) asBson)["text"].stringListValue;
              string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
              WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
              if (stringListValue.Count > 0)
              {
                for (int index = 0; index < stringListValue.Count; ++index)
                  WorldCheats.textToShow += string.Format("{0}: {1}\n", (object) (index + 1), (object) stringListValue[index]);
              }
              else
                WorldCheats.textToShow += "Item text is empty\n";
              WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
              return;
            }
          }
          if (((BSONValue) asBson)["class"].stringValue == "SignSwitchableTextWoodenData")
          {
            string stringValue9 = ((BSONValue) asBson)["textA"].stringValue;
            string stringValue10 = ((BSONValue) asBson)["textB"].stringValue;
            string stringValue11 = ((BSONValue) asBson)["blockType"].stringValue;
            string str6 = string.IsNullOrWhiteSpace(stringValue9) ? "1: Text is empty" : "1: <noparse>" + stringValue9;
            string str7 = string.IsNullOrWhiteSpace(stringValue10) ? "2: Text is empty" : "2: <noparse>" + stringValue10;
            WorldCheats.textToShow = string.Format("{0}({1}) on {2} (Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue11, (object) mapPoint);
            WorldCheats.textToShow = WorldCheats.textToShow + str6 + "\n";
            WorldCheats.textToShow = WorldCheats.textToShow + str7 + "\n";
            WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
          }
          else
          {
            foreach (string chestsDataClass in Globals.chestsDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == chestsDataClass)
              {
                string stringValue12 = ((BSONValue) asBson)["blockType"].stringValue;
                string stringValue13 = ((BSONValue) asBson)["maxStorageItems"].stringValue;
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue12, (object) mapPoint);
                List<int> int32ListValue1 = ((BSONValue) asBson)["storageItemsAmounts"].int32ListValue;
                List<int> int32ListValue2 = ((BSONValue) asBson)["storageItemsAsInventoryKeys"].int32ListValue;
                WorldCheats.textToShow += string.Format("Fullness: {0}/{1}\n", (object) int32ListValue2.Count, (object) stringValue13);
                for (int index = 0; index < int32ListValue2.Count; ++index)
                {
                  PlayerData.InventoryKey inventoryKey = PlayerData.InventoryKey.IntToInventoryKey(int32ListValue2[index]);
                  string blockTypeOrSeedName = TextManager.GetBlockTypeOrSeedName(inventoryKey);
                  WorldCheats.textToShow += string.Format("{0}: {1} x{2} [{3}]\n", (object) (index + 1), (object) blockTypeOrSeedName, (object) int32ListValue1[index], (object) inventoryKey.itemType);
                }
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string locksDataClass in Globals.locksDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == locksDataClass)
              {
                string stringValue14 = ((BSONValue) asBson)["blockType"].stringValue;
                World.BlockType blockType = Globals.world.GetBlockType(mapPoint);
                string stringValue15 = ((BSONValue) asBson)["playerWhoOwnsLockId"].stringValue;
                string stringValue16 = ((BSONValue) asBson)["playerWhoOwnsLockName"].stringValue;
                bool boolValue1 = ((BSONValue) asBson)["isOpen"].boolValue;
                bool boolValue2 = ((BSONValue) asBson)["isBattleOn"].boolValue;
                string str8 = ((BSONValue) asBson)["lastActivatedTime"].dateTimeValue.ToString();
                string str9 = ((BSONValue) asBson)["creationTime"].dateTimeValue.ToString();
                List<string> stringListValue1 = ((BSONValue) asBson)["playersWhoHaveAccessToLock"].stringListValue;
                List<string> stringListValue2 = ((BSONValue) asBson)["playersWhoHaveMinorAccessToLock"].stringListValue;
                string str10 = "";
                string str11 = "";
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) blockType, (object) stringValue14, (object) mapPoint);
                WorldCheats.textToShow = WorldCheats.textToShow + TextManager.GetBlockTypeName(blockType) + " owned by " + stringValue16 + " (" + stringValue15 + ")\n";
                WorldCheats.textToShow = WorldCheats.textToShow + "[Last active on: " + str8 + "\n";
                WorldCheats.textToShow = WorldCheats.textToShow + "[Placed on: " + str9 + "\n";
                WorldCheats.textToShow += string.Format("There are {0} players who got Full access to this lock\n", (object) stringListValue1.Count);
                foreach (string str12 in stringListValue1)
                  str10 = str10 + " " + str12 + ",";
                if (!string.IsNullOrEmpty(str10))
                  str10 += ".";
                WorldCheats.textToShow = WorldCheats.textToShow + str10 + "\n";
                WorldCheats.textToShow += string.Format("There are {0} players who got Minor access to this lock\n", (object) stringListValue2.Count);
                foreach (string str13 in stringListValue2)
                  str11 = str11 + " " + str13 + ",";
                if (!string.IsNullOrEmpty(str11))
                  str11 += ".";
                WorldCheats.textToShow = WorldCheats.textToShow + str11 + "\n";
                string str14 = boolValue1 ? "Lock is OPEN" : "Lock is Closed";
                string str15 = boolValue2 ? "Battle Active" : "Battle Inactive";
                WorldCheats.textToShow = WorldCheats.textToShow + TextManager.GetBlockTypeName(blockType) + ": [" + str14 + "|" + str15 + "]\n";
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string doorsDataClass in Globals.doorsDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == doorsDataClass)
              {
                string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
                string str = ((BSONValue) asBson)["isLocked"].boolValue ? "Door is currently locked." : "Door is currently open.";
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
                WorldCheats.textToShow = WorldCheats.textToShow + str + "\n";
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string advancedDataClass in Globals.doorsAdvancedDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == advancedDataClass)
              {
                string stringValue17 = ((BSONValue) asBson)["blockType"].stringValue;
                bool boolValue = ((BSONValue) asBson)["isVIPOnly"].boolValue;
                string stringValue18 = ((BSONValue) asBson)["minLevel"].stringValue;
                string stringValue19 = ((BSONValue) asBson)["maxLevel"].stringValue;
                string str = boolValue ? "|VIP Only]" : "]";
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue17, (object) mapPoint);
                WorldCheats.textToShow = WorldCheats.textToShow + "[Min: " + stringValue18 + "|Max: " + stringValue19 + str + "\n";
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string triggersDataClass in Globals.triggersDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == triggersDataClass)
              {
                string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
                string str = ((BSONValue) asBson)["canEveryoneUse"].boolValue ? "Trigger is currently open." : "Trigger is currently closed.";
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
                WorldCheats.textToShow = WorldCheats.textToShow + str + "\n";
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string giftboxesDataClass in Globals.giftboxesDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == giftboxesDataClass)
              {
                string stringValue20 = ((BSONValue) asBson)["blockType"].stringValue;
                int int32Value1 = ((BSONValue) asBson)["itemInventoryKeyAsInt"].int32Value;
                string stringValue21 = ((BSONValue) asBson)["itemAmount"].stringValue;
                int int32Value2 = ((BSONValue) asBson)["takeAmount"].int32Value;
                string blockTypeOrSeedName = TextManager.GetBlockTypeOrSeedName(PlayerData.InventoryKey.IntToInventoryKey(int32Value1));
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue20, (object) mapPoint);
                WorldCheats.textToShow = int32Value2 == 0 ? WorldCheats.textToShow + "Gift Box is empty.\n" : WorldCheats.textToShow + string.Format("{0} ({1}/{2})\n", (object) blockTypeOrSeedName, (object) int32Value2, (object) stringValue21);
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string guestbooksDataClass in Globals.guestbooksDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == guestbooksDataClass)
              {
                string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
                bool boolValue = ((BSONValue) asBson)["locked"].boolValue;
                List<string> stringListValue = ((BSONValue) asBson)["messages"].stringListValue;
                List<int> int32ListValue = ((BSONValue) asBson)["approved"].int32ListValue;
                string str16 = boolValue ? "Item is currently locked." : "Item is currently open.";
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
                WorldCheats.textToShow += string.Format("Fullness: {0}/20, {1}\n", (object) stringListValue.Count, (object) str16);
                for (int index = 0; index < stringListValue.Count; ++index)
                {
                  string str17 = int32ListValue[index] == 1 ? "Approved" : "Waiting";
                  WorldCheats.textToShow += string.Format("{0}: ({1}) {2}\n", (object) (index + 1), (object) str17, (object) stringListValue[index]);
                }
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            foreach (string displayDataClass in Globals.displayDataClasses)
            {
              if (((BSONValue) asBson)["class"].stringValue == displayDataClass)
              {
                string stringValue22 = ((BSONValue) asBson)["blockType"].stringValue;
                string stringValue23 = ((BSONValue) asBson)["text"].stringValue;
                int int32Value = ((BSONValue) asBson)["storageItemAsInventoryKey"].int32Value;
                string str = string.IsNullOrWhiteSpace(stringValue23) ? "No Text" : "Item Text:\n " + stringValue23;
                PlayerData.InventoryKey inventoryKey = PlayerData.InventoryKey.IntToInventoryKey(int32Value);
                string blockTypeOrSeedName = TextManager.GetBlockTypeOrSeedName(inventoryKey);
                WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue22, (object) mapPoint);
                WorldCheats.textToShow = WorldCheats.textToShow + blockTypeOrSeedName + string.Format(" [{0}]\n{1}\n", (object) inventoryKey.itemType, (object) str);
                WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                return;
              }
            }
            if (((BSONValue) asBson)["class"].stringValue == "DonationBoxData" || ((BSONValue) asBson)["class"].stringValue == "DonationBoxValentinesData")
            {
              string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
              List<int> int32ListValue3 = ((BSONValue) asBson)["itemsAsInventoryKeys"].int32ListValue;
              List<int> int32ListValue4 = ((BSONValue) asBson)["itemsAmounts"].int32ListValue;
              List<string> stringListValue = ((BSONValue) asBson)["playerIds"].stringListValue;
              int int32Value3 = ((BSONValue) asBson)["minRarity"].int32Value;
              int int32Value4 = ((BSONValue) asBson)["maxItems"].int32Value;
              WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
              WorldCheats.textToShow += string.Format("Fullness: {0}/{1}, Min. Rarity: {2}\n", (object) int32ListValue3.Count, (object) int32Value4, (object) int32Value3);
              for (int index = 0; index < int32ListValue3.Count; ++index)
              {
                PlayerData.InventoryKey inventoryKey = PlayerData.InventoryKey.IntToInventoryKey(int32ListValue3[index]);
                string blockTypeOrSeedName = TextManager.GetBlockTypeOrSeedName(inventoryKey);
                WorldCheats.textToShow += string.Format("{0}: {1} x{2} [{3}] (From : {4})\n", (object) (index + 1), (object) blockTypeOrSeedName, (object) int32ListValue4[index], (object) inventoryKey.itemType, (object) stringListValue[index]);
              }
              WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
            }
            else if (((BSONValue) asBson)["class"].stringValue == "AdTVData")
            {
              string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
              int int32Value = ((BSONValue) asBson)["amount"].int32Value;
              WorldCheats.ShowTooltipBox(string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint) + string.Format("Amount: {0}\n", (object) int32Value));
            }
            else
            {
              foreach (string trapsDataClass in Globals.trapsDataClasses)
              {
                if (((BSONValue) asBson)["class"].stringValue == trapsDataClass)
                {
                  string stringValue = ((BSONValue) asBson)["blockType"].stringValue;
                  int int32Value5 = ((BSONValue) asBson)["trapFrequencyType"].int32Value;
                  int int32Value6 = ((BSONValue) asBson)["trapSyncIndex"].int32Value;
                  WorldCheats.textToShow = string.Format("{0}({1}) on {2}\n(Original: hold Alt)\n", (object) Globals.world.GetBlockType(mapPoint), (object) stringValue, (object) mapPoint);
                  WorldCheats.textToShow += string.Format("FrequencyType: {0}({1})\nTrapSyncIndex: {2}\nDirection: {3}\nDoDamageNow: {4}\n", (object) (TrapFrequencyType) (int) (byte) int32Value5, (object) int32Value5, (object) int32Value6, (object) worldItemData.blockDirection, (object) worldItemData.doDamageNow);
                  WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
                  return;
                }
              }
              WorldCheats.textToShow = Utils.DumpBSON(asBson);
              WorldCheats.textToShow = WorldCheats.textToShow.Remove(0, 1);
              WorldCheats.textToShow = WorldCheats.textToShow.Remove(WorldCheats.textToShow.Length - 1);
              WorldCheats.ShowTooltipBox(WorldCheats.textToShow);
            }
          }
        }
      }
    }
  }
}
