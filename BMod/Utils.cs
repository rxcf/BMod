
using Il2Cpp;
using Il2CppBasicTypes;
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
using UnityEngine;

 
namespace BMod
{
  internal class Utils
  {
    public static void Msg(string message)
    {
      ControllerHelper.chatUI.NewMessage(new ChatMessage(message + "</color>", DateTime.Now, (ChatMessage.ChannelTypes) 6, "<B><#3481fa>[<#5996f7>BMod<#3481fa>]</color></color></color></B>", "", ""));
    }

    public static void Error(string message)
    {
      ControllerHelper.chatUI.NewMessage(new ChatMessage(message + "</color>", DateTime.Now, (ChatMessage.ChannelTypes) 6, "<B><#ff2919>[<#f25449>ERROR<#ff2919>]</color></color></color></B>", "", ""));
    }

    public static void Warning(string message)
    {
      ControllerHelper.chatUI.NewMessage(new ChatMessage(message + "</color>", DateTime.Now, (ChatMessage.ChannelTypes) 6, "<B><#ffbb00>[<#ffdd00>WARNING<#ffbb00>]</color></color></color></B>", "", ""));
    }

    public static void D(string message)
    {
      ControllerHelper.chatUI.NewMessage(new ChatMessage(message + "</color>", DateTime.Now, (ChatMessage.ChannelTypes) 6, "<B><#707070>[<#808080>DEBUG<#707070>]</color></color></color></B>", "", ""));
    }

    public static void InvalidInputNotification(string arg0)
    {
      ControllerHelper.notificationController.MakeNotification((NotificationController.NotificationType) 74, Globals.player.currentPlayerMapPoint);
      Utils.Error("Too few arguments or invalid input. Type '" + arg0 + " ?' to view help.");
    }

    public static Vector2i ConvertWorldPointToMapPoint(Vector2 worldPoint)
    {
      Vector2i mapPoint;
      mapPoint.x = 0;
      mapPoint.y = 0;
      mapPoint.x = (int) (((double) worldPoint.x + (double) ConfigData.tileSizeX * 0.5) / (double) ConfigData.tileSizeX);
      mapPoint.y = (int) (((double) worldPoint.y + (double) ConfigData.tileSizeY * 0.5) / (double) ConfigData.tileSizeY);
      return mapPoint;
    }

    public static Vector2 ConvertMapPointToWorldPoint(Vector2i mapPoint)
    {
      Vector2 worldPoint;
      worldPoint.x = (float) mapPoint.x * ConfigData.tileSizeX;
      worldPoint.y = (float) mapPoint.y * ConfigData.tileSizeY;
      return worldPoint;
    }

    public static List<Vector2i> GetMapPointsGridInRange(int range)
    {
      try
      {
        List<Vector2i> pointsGridInRange = new List<Vector2i>();
        Vector2i vector2i;
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i).\u002Ector(Globals.player.currentPlayerMapPoint.x - range, Globals.player.currentPlayerMapPoint.y + range);
        int x = vector2i.x;
        int y = vector2i.y;
        for (int index1 = 0; index1 < 2 * range + 1; ++index1)
        {
          for (int index2 = 0; index2 < 2 * range + 1; ++index2)
          {
            if (x >= 0 && y >= 0 && x <= Globals.world.worldSizeX && y <= Globals.world.worldSizeY)
              pointsGridInRange.Add(new Vector2i(x, y));
            ++x;
          }
          x = vector2i.x;
          --y;
        }
        return pointsGridInRange;
      }
      catch
      {
        return (List<Vector2i>) null;
      }
    }

    public static List<string> FindNetworkPlayers(Utils.SearchType searchType, string shortName = "---")
    {
      List<NetworkPlayer> otherPlayers = NetworkPlayers.otherPlayers;
      List<FriendSlot> friendList = FriendsUI.friendList;
      List<string> list1 = new List<string>();
      List<string> networkPlayers = new List<string>();
      foreach (FriendSlot friendSlot in friendList)
        list1.Add(((PlayerListsUI.PlayerSlot) friendSlot).GetPlayerID());
      switch (searchType)
      {
        case Utils.SearchType.Others:
          foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
            networkPlayers.Add(otherPlayer.clientId);
          return networkPlayers;
        case Utils.SearchType.Friends:
          foreach (NetworkPlayer networkPlayer in otherPlayers)
          {
            if (list1.Contains(networkPlayer.clientId))
              networkPlayers.Add(networkPlayer.clientId);
          }
          return networkPlayers;
        case Utils.SearchType.NonFriends:
          foreach (NetworkPlayer networkPlayer in otherPlayers)
          {
            if (!list1.Contains(networkPlayer.clientId))
              networkPlayers.Add(networkPlayer.clientId);
          }
          return networkPlayers;
        case Utils.SearchType.WorldStaff:
          if (Globals.world == null || Globals.world.lockWorldDataHelper == null)
            return new List<string>();
          List<string> list2 = new List<string>();
          List<string> list3 = new List<string>();
          List<string> haveAccessToLock = Globals.world.lockWorldDataHelper.GetPlayersWhoHaveAccessToLock();
          List<string> minorAccessToLock = Globals.world.lockWorldDataHelper.GetPlayersWhoHaveMinorAccessToLock();
          string playerWhoOwnsLockId = Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockId();
          foreach (string str in haveAccessToLock)
            list2.Add(PlayerIdNameHelper.GetPlayerIdFromCombined(str));
          foreach (string str in minorAccessToLock)
            list2.Add(PlayerIdNameHelper.GetPlayerIdFromCombined(str));
          foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
            list3.Add(otherPlayer.clientId);
          if (list3.Contains(playerWhoOwnsLockId))
            list2.Add(Globals.world.lockWorldDataHelper.GetPlayerWhoOwnsLockId());
          foreach (string str in list2)
          {
            if (list3.Contains(str))
              networkPlayers.Add(str);
          }
          return networkPlayers;
        case Utils.SearchType.ShortName:
          foreach (NetworkPlayer otherPlayer in NetworkPlayers.otherPlayers)
          {
            if (otherPlayer.name.ToLower().Replace("<#ff8000>", "").Replace("<#00ff00>", "").StartsWith(shortName.ToLower()))
              networkPlayers.Add(otherPlayer.clientId);
          }
          return networkPlayers;
        default:
          return (List<string>) null;
      }
    }

    public static string GetLoadingTip(int index)
    {
      string[] strArray1 = new string[59]
      {
        "Hacking is bad.",
        "Do not hack on giveaways",
        "Mods will ban you",
        "Never hack in blacktower",
        "No fly - just ping, no hack - bad sync",
        "Hackers always win",
        "Giveaway rooms are best",
        "It will never load",
        "Press F3 and arrow keys for freecam",
        "I love chips",
        "Press F8 to force leave world",
        "Don't get caught!",
        "No mobile hack",
        "H4W Sucks <sprite=\"emj\" name=\"1f92e\">",
        "Do not hack on giveaways",
        "Only retards play dropgames",
        "Fishing for noobs",
        "I miss Jake",
        "Donate at BPLUS",
        "Fishing hacks shit",
        "Never buy minor rights",
        "Noob?",
        "Better luck next time",
        "Don't press alt+f4",
        "BTrainer is not shit",
        "Mods are not proud of you",
        "Free stuff only in dailybonus",
        "Don't try to dupe",
        "Scamming = niga",
        "Press alt+esc for dupe",
        "Hackers never stop",
        "Mining hacks are bad",
        "Don't be racist",
        "hacking is illegal",
        "Hacks for noobs",
        "BTrainer is better",
        "Press F2 for data tooltip",
        "Scam PW - frick",
        "Going to cheat on giveaways?",
        "Don't make others see you hack",
        "Don't trust private servers",
        "Press Alt+Click for teleport",
        "Type /help to view commands",
        "Fuck these bullshitters",
        "I hope you won't get banned",
        "BMod is good cheat",
        "Never farm spike bombs",
        "Our public world is BMOD",
        "Mods spectate you",
        "Hack download link?",
        "Don't use other hacks",
        "Say \"I hack\", mods will show trick",
        "Mods are noobs :)",
        "Unwear wings before KeysFly!",
        "Wear jetpack to bypass fly kick",
        "I was bad, now I'm good",
        "Don't warp FX36",
        "Join discord server for updates",
        "Good Luck!"
      };
      string[] strArray2 = new string[60]
      {
        "Get a life",
        "dw about freebc.exe",
        "Keep playing Pixel Worlds",
        "We grow like a tree",
        "Your PC was compromised",
        "niga-niga-niga-niga",
        "use /w for join other worlds faster",
        "Benz was here",
        "tips are paid, insert money",
        "/q for free bc",
        "black ni.. ght",
        "don't miss daily jetrace entry",
        "bmod > h4w",
        "AutoMine for clowns",
        "PWE orders good way for profit",
        "Hack illegal but do if necessary",
        "Keep yourself safe",
        "gamblit is bullshit",
        "use /s to summon several people at once",
        "Balls",
        "Don't trust private servers!",
        "Only Bmod",
        "i know you " + StaticPlayer.theRealPlayername,
        "Failed to reach the world",
        "Donate at BPLUS for a candy",
        "Buy premium for green name",
        "use /command ? to view syntax",
        "Don't forget to feed pets",
        "Share BMod with your friends",
        "Join official discord server",
        "Use Ctrl+M+G to sell mgems quickly",
        "Use W/S to switch pages in pwe/hologram",
        "#BModIsBEST",
        "Project CX1 Scammers",
        "No android hack link :(",
        "Watch on time",
        "Use FarmBot to get more xp and gems",
        "Sub to yt. Nekto PW",
        "FishHack not profitable",
        "Press Shift+F9 if loading stuck",
        "Use Ctrl+F+G to sell fgems quickly",
        "Planning world re-design? Try /replace",
        "/kill faster than respawn",
        "use /login to switch accs faster",
        "Use invis carefully",
        "Don't use /b others",
        "Type /q to close game instantly",
        "Wanna try item? activate LagHack and use /give",
        "Join discord server for updates and giveaways",
        "Use Hit Annoy to troll people",
        "use laghack when tp, no one see you",
        "Wanna troll friends? Try /bc +5000000",
        "This is the most safe hack",
        "dm ne.kto in discord if got questions",
        "promocode \"tips\" gives 10% discount for premium",
        "Try our Data cheats, they're profitable!",
        "Need visual rights? Type /ownall",
        "Opening many boosters? Press Space to ReBuy",
        "Record bmod showcase, get free premium",
        "Press W/S to switch PWE pages"
      };
      DateTime now = DateTime.Now;
      return ((DateTime) ref now).Hour % 2 == 0 ? strArray1[index] : strArray2[index];
    }

    public static string DumpBSON(BSONObject obj)
    {
      BsonDocument bsonDocument = BsonSerializer.Deserialize<BsonDocument>(Il2CppArrayBase<byte>.op_Implicit((Il2CppArrayBase<byte>) SimpleBSON.Dump(obj)), (Action<BsonDeserializationContext.Builder>) null);
      JsonWriterSettings jsonWriterSettings = new JsonWriterSettings();
      jsonWriterSettings.Indent = true;
      BsonSerializationArgs serializationArgs = new BsonSerializationArgs();
      return BsonExtensionMethods.ToJson<BsonDocument>(bsonDocument, jsonWriterSettings, (IBsonSerializer<BsonDocument>) null, (Action<BsonSerializationContext.Builder>) null, serializationArgs);
    }

    public static void SimulateMessages(BSONObject[] messages)
    {
      BSONObject bsonObject = new BSONObject();
      for (int index = 0; index < messages.Length; ++index)
        ((BSONValue) bsonObject)[string.Format("m{0}", (object) index)] = (BSONValue) messages[index];
      ((BSONValue) bsonObject)["mc"] = BSONValue.op_Implicit(messages.Length);
      ControllerHelper.networkClient.HandleMessages(bsonObject);
    }

    public static void SimulateMessages(BSONObject messages)
    {
      BSONObject bsonObject = new BSONObject();
      ((BSONValue) bsonObject)["m0"] = (BSONValue) messages;
      ((BSONValue) bsonObject)["mc"] = BSONValue.op_Implicit(1);
      ControllerHelper.networkClient.HandleMessages(bsonObject);
    }

    public static Vector2i DistanceMP(Vector2i point1, Vector2i point2)
    {
      int num1 = 0;
      int num2 = 0;
      if (point1.x > point2.x)
        num1 = point1.x - point2.x;
      else if (point1.x < point2.x)
        num1 = point2.x - point1.x;
      if (point1.y > point2.y)
        num2 = point1.y - point2.y;
      else if (point1.y < point2.y)
        num2 = point2.y - point1.y;
      return new Vector2i(num1, num2);
    }

    public static float Distance(Vector2i point1, Vector2i point2)
    {
      float num1 = (float) (point1.x - point2.x);
      float num2 = (float) (point1.y - point2.y);
      return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
    }

    public static void DoCustomNotification(string text, Vector2i mapPoint)
    {
      if (mapPoint.y >= ControllerHelper.worldController.world.worldSizeY)
        mapPoint.y = ControllerHelper.worldController.world.worldSizeY - 1;
      if (Object.op_Equality((Object) ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex], (Object) null))
      {
        ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex] = Object.Instantiate<GameObject>(Globals.notificationController.notificationPrefab, ControllerHelper.worldController.ConvertMapPointToWorldPoint(mapPoint), Globals.notificationController.notificationPrefab.transform.rotation);
        ((Il2CppArrayBase<TextMeshPro>) Globals.notificationController.notificationTextMeshPros)[Globals.notificationController.notificationsIndex] = ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex].GetComponent<TextMeshPro>();
        ((Il2CppArrayBase<DestroyTextAnimation>) Globals.notificationController.notificationDestroyTextAnimation)[Globals.notificationController.notificationsIndex] = ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex].GetComponent<DestroyTextAnimation>();
      }
      ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex].transform.position = ControllerHelper.worldController.ConvertMapPointToWorldPoint(mapPoint);
      ((TMP_Text) ((Il2CppArrayBase<TextMeshPro>) Globals.notificationController.notificationTextMeshPros)[Globals.notificationController.notificationsIndex]).text = text;
      ((Il2CppArrayBase<DestroyTextAnimation>) Globals.notificationController.notificationDestroyTextAnimation)[Globals.notificationController.notificationsIndex].StartAnimation();
      ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications)[Globals.notificationController.notificationsIndex].SetActive(true);
      ++Globals.notificationController.notificationsIndex;
      if (Globals.notificationController.notificationsIndex < ((Il2CppArrayBase<GameObject>) Globals.notificationController.notifications).Length)
        return;
      Globals.notificationController.notificationsIndex = 0;
    }

    public static void WorldItemUpdated(BSONObject bson, Vector2i mapPoint)
    {
      World world = Globals.world;
      BSONObject bsonObject = new BSONObject();
      ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("WIU");
      ((BSONValue) bsonObject)["WiB"] = (BSONValue) bson;
      ((BSONValue) bsonObject)["x"] = BSONValue.op_Implicit(mapPoint.x);
      ((BSONValue) bsonObject)["y"] = BSONValue.op_Implicit(mapPoint.y);
      ((BSONValue) bsonObject)["PT"] = BSONValue.op_Implicit((int) ConfigData.GetToolUsableForBlock(Globals.world.GetBlockType(mapPoint)));
      ((BSONValue) bsonObject)["U"] = BSONValue.op_Implicit("LOCALWIU");
      world.WorldItemUpdate(bsonObject);
    }

    public static BSONObject ToBson(string json)
    {
      BsonDocument bsonDocument = BsonSerializer.Deserialize<BsonDocument>(json, (Action<BsonDeserializationContext.Builder>) null);
      BsonExtensionMethods.ToBson<BsonDocument>(bsonDocument, (IBsonSerializer<BsonDocument>) null, (BsonBinaryWriterSettings) null, (Action<BsonSerializationContext.Builder>) null, new BsonSerializationArgs(), 0);
      return SimpleBSON.Load(Il2CppStructArray<byte>.op_Implicit(BsonExtensionMethods.ToBson<BsonDocument>(bsonDocument, (IBsonSerializer<BsonDocument>) null, (BsonBinaryWriterSettings) null, (Action<BsonSerializationContext.Builder>) null, new BsonSerializationArgs(), 0)));
    }

    public enum SearchType
    {
      Others,
      Friends,
      NonFriends,
      WorldStaff,
      ShortName,
      BUser,
    }

    public static class WorldCounter
    {
      public static Dictionary<Utils.WorldCounter.ItemType, int> CountGems(
        bool advanced,
        out Dictionary<World.BlockType, int> advancedCount)
      {
        Dictionary<Utils.WorldCounter.ItemType, int> dictionary = new Dictionary<Utils.WorldCounter.ItemType, int>();
        advancedCount = new Dictionary<World.BlockType, int>();
        dictionary.Add(Utils.WorldCounter.ItemType.GemDrop, 0);
        dictionary.Add(Utils.WorldCounter.ItemType.GemstoneGems, 0);
        dictionary.Add(Utils.WorldCounter.ItemType.FishGems, 0);
        dictionary.Add(Utils.WorldCounter.ItemType.PouchGems, 0);
        foreach (CollectableData collectable in Globals.world.collectables)
        {
          if (collectable.isGem)
          {
            int gemValue = ConfigData.GetGemValue(collectable.gemType);
            dictionary[Utils.WorldCounter.ItemType.GemDrop] += gemValue;
            if (advanced)
            {
              if (!advancedCount.ContainsKey(collectable.blockType))
                advancedCount.Add(collectable.blockType, gemValue);
              else
                advancedCount[collectable.blockType] += gemValue;
            }
          }
          if (ConfigData.IsBlockMiningGemstone(collectable.blockType))
          {
            int num = ConfigData.GetGemstoneRecycleValueForMiningGemstoneRecycler(collectable.blockType) * (int) collectable.amount;
            dictionary[Utils.WorldCounter.ItemType.GemstoneGems] += num;
            if (advanced)
            {
              if (!advancedCount.ContainsKey(collectable.blockType))
                advancedCount.Add(collectable.blockType, num);
              else
                advancedCount[collectable.blockType] += num;
            }
          }
          if (ConfigData.IsFish(collectable.blockType))
          {
            int num = ConfigData.GetFishRecycleValueForFishRecycler(collectable.blockType) * (int) collectable.amount;
            dictionary[Utils.WorldCounter.ItemType.FishGems] += num;
            if (advanced)
            {
              if (!advancedCount.ContainsKey(collectable.blockType))
                advancedCount.Add(collectable.blockType, num);
              else
                advancedCount[collectable.blockType] += num;
            }
          }
          if (ConfigData.IsConsumableTreasurePouch(collectable.blockType))
          {
            int num = ConfigData.GetTreasurePouchRewardAmount(collectable.blockType) * (int) collectable.amount;
            dictionary[Utils.WorldCounter.ItemType.PouchGems] += num;
            if (advanced)
            {
              if (!advancedCount.ContainsKey(collectable.blockType))
                advancedCount.Add(collectable.blockType, num);
              else
                advancedCount[collectable.blockType] += num;
            }
          }
        }
        return dictionary;
      }

      public enum ItemType
      {
        GemDrop,
        GemstoneGems,
        FishGems,
        PouchGems,
      }
    }
  }
}
