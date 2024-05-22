
using BMod.Discord;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

 
namespace BMod.Auto
{
  internal class MineBot
  {
    public static bool active = false;
    public bool[] mineLevels = new bool[5]
    {
      true,
      true,
      true,
      true,
      true
    };
    public static bool pickShit = false;
    public static bool joiningMine = false;
    public bool useTeleport = true;
    public static bool showPath = true;
    public static bool useWebhook = false;
    public static bool nigaMode = false;
    public static bool hibernateEnd = false;
    public bool reconnecting = false;
    private static World.BlockType pickaxe = (World.BlockType) 4087;
    public List<PNode> path = new List<PNode>();
    public Vector2i lastMP = new Vector2i(0, 0);
    public MineBot.BotState state = MineBot.BotState.None;
    public int currentMineIndex = -1;
    public int gems = 0;
    public DateTime allTimePassed = new DateTime();
    public DateTime endTime = new DateTime();
    public int mineTokens = 0;
    public int enemyKilled = 0;
    public int minesCleared = 0;
    public int xp = 0;
    private float miningSpeed = 0.22f;
    public float miningTimer = 0.0f;
    public float estimatedCompletionTime = 0.0f;
    public float timePassed = 0.0f;
    public static float enemyPriorityMode = 0.0f;
    public string bufferWorld = "BMOD";
    public string bufferEntry = "";
    public string webhook = "";
    public HardReconnectReason dcReason = (HardReconnectReason) 0;
    public string dcRealReason = "not defined";
    public static List<World.BlockType> shitStones = new List<World.BlockType>()
    {
      (World.BlockType) 4047,
      (World.BlockType) 4048,
      (World.BlockType) 4017,
      (World.BlockType) 4018,
      (World.BlockType) 4037
    };
    private Random roll = new Random();

    public MineBot.EnemyPriority EPriority
    {
      get
      {
        try
        {
          return (MineBot.EnemyPriority) Math.Round((double) MineBot.enemyPriorityMode, 0);
        }
        catch
        {
          return MineBot.EnemyPriority.Ignore;
        }
      }
    }

    public static void Stop(string reason, bool byUser = false, bool remote = false)
    {
      MineBot.active = false;
      Globals.mineBot.endTime = DateTime.Now;
      if (!byUser)
      {
        Console.WriteLine("MineBot :: " + reason);
        if (MineBot.hibernateEnd)
          Process.Start(new ProcessStartInfo("cmd.exe", "/c shutdown /h"));
        InfoPopupUI.SetupInfoPopup(TextManager.Capitalize(nameof (MineBot)), "was deactivated due to\n" + reason + " | " + Globals.world.worldName == "MINEWORLD" ? string.Format("MINEWORLD, {0}LVL", (object) (Globals.mineBot.currentMineIndex + 1)) : Globals.world.worldName);
        InfoPopupUI.ForceShowMenu();
      }
      else
        Console.WriteLine("MineBot :: deactivated by user.");
      Globals.iIgnore.Clear();
    }

    public static void Start()
    {
      if (Object.op_Equality((Object) Globals.player, (Object) null) || Globals.playerData == null || Globals.world == null)
        return;
      MineBot.pickaxe = (World.BlockType) 0;
      if (((IEnumerable<World.BlockType>) Globals.pickaxes).Contains<World.BlockType>(Globals.player.GetTopArmBlockType()))
        MineBot.pickaxe = Globals.player.GetTopArmBlockType();
      if (MineBot.pickaxe == 0)
        MineBot.FailedToStart("Pickaxe not Found");
      else if (Globals.playerData.GetInventoryData(new PlayerData.InventoryKey(MineBot.pickaxe, (PlayerData.InventoryItemType) 5)) == null)
        MineBot.FailedToStart("Pickaxe Data NULL");
      else if (Globals.playerData.GetWornWeaponDurability() < 50000)
      {
        MineBot.FailedToStart("Inf. Durability required!");
      }
      else
      {
        DateTime utcNow = DateTime.UtcNow;
        TimeSpan timeSpan = ((DateTime) ref utcNow).Subtract(Globals.playerData.accountAge);
        if (((TimeSpan) ref timeSpan).TotalDays < 1.0)
        {
          MineBot.FailedToStart("Account is too new");
          BluePopupUI.SetPopupValue((PopupMode) 1, "", "Account is to new!", "0d age accounts can't exit mineworld\n(even if you play without hacks)\nPlease login another account", "Okay", "Okay", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
          Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
        }
        else
        {
          if (Globals.world.worldName != "MINEWORLD" && ConfigData.IsBlockPortalWireable(Globals.world.GetBlockType(Globals.player.currentPlayerMapPoint)))
          {
            if (string.IsNullOrWhiteSpace(BSONValue.op_Implicit(((BSONValue) Globals.world.GetWorldItemData(Globals.player.currentPlayerMapPoint).GetAsBSON())["entryPointID"])))
            {
              MineBot.FailedToStart("Portal has no entry!");
              return;
            }
          }
          else if (Globals.world.worldName != "MINEWORLD")
          {
            BluePopupUI.SetPopupValue((PopupMode) 1, "", "Portal required!", "Please stand close to any wireable portal with entry point to start", "Okay", "Okay", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
            Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
            MineBot.FailedToStart("Stand near portal!");
            return;
          }
          if (Globals.world.worldName != "MINEWORLD" && ProfanityFilter.Validate(Globals.mineBot.bufferWorld))
          {
            Globals.mineBot.bufferWorld = Globals.world.worldName;
            if (ConfigData.IsBlockPortalWireable(Globals.world.GetBlockType(Globals.player.currentPlayerMapPoint)))
            {
              WorldItemBase worldItemData = Globals.world.GetWorldItemData(Globals.player.currentPlayerMapPoint);
              if (worldItemData != null)
              {
                BSONObject asBson = worldItemData.GetAsBSON();
                int num = string.IsNullOrWhiteSpace(((BSONValue) asBson)["entryPointID"].stringValue) ? 0 : (Globals.world.DoesPlayerHaveRightToModifyItemData(Globals.player.currentPlayerMapPoint, Globals.playerData, false) ? 1 : 0);
                Globals.mineBot.bufferEntry = num == 0 ? "" : ((BSONValue) asBson)["entryPointID"].stringValue;
              }
            }
            else
              Globals.mineBot.bufferEntry = "";
          }
          else if (!ProfanityFilter.Validate(Globals.mineBot.bufferWorld))
            Globals.mineBot.bufferWorld = Globals.playerData.playerId;
          if (Globals.world.worldName == Globals.mineBot.bufferWorld)
          {
            Globals.mineBot.JoinMine(Globals.mineBot.GetMineLevelPriority());
            MelonLogger.Msg("Bot started, joining the Mineworld. Relax and enjoy!");
          }
          if (Globals.world.worldName == "MINEWORLD")
          {
            if (Globals.mineBot.bufferWorld == "BMOD")
            {
              MineBot.FailedToStart("BufferWorldNotSet");
              BluePopupUI.SetPopupValue((PopupMode) 1, "", "Start in Your world!", "Buffer world was not set, go your world and launch the bot there.", "Okay", "Okay", (PopupEvent) null, (PopupEvent) null, 0.0f, 0, false, false, false);
              Globals.rootUI.OnOrOffMenu(Il2CppType.Of<BluePopupUI>());
              return;
            }
            if (Globals.mineBot.path.Count > 0)
            {
              if (Vector2i.op_Inequality(Globals.player.currentPlayerMapPoint, Globals.mineBot.lastMP))
              {
                Globals.mineBot.path.InsertRange(0, (IEnumerable<PNode>) Globals.teleport.pather.SkidMiningPath(Globals.player.currentPlayerMapPoint, Globals.mineBot.lastMP));
                Globals.mineBot.state = MineBot.BotState.Moving;
              }
            }
            else
              Globals.mineBot.StartMining();
          }
          else
          {
            Globals.mineBot.allTimePassed = DateTime.Now;
            Globals.mineBot.gems = 0;
            Globals.mineBot.xp = 0;
            Globals.mineBot.minesCleared = 0;
            Globals.mineBot.enemyKilled = 0;
          }
          MineBot.active = true;
          Globals.mineBot.timePassed = 0.0f;
          Globals.giveawayMode = false;
          Globals.colorfulNames = false;
          Globals.aiAimBot = false;
          Globals.playerAimBot = false;
          Globals.iIgnore.Add("LW");
        }
      }
    }

    private static void FailedToStart(string why)
    {
      InfoPopupUI.SetupInfoPopup("LAUNCH FAILED", why);
      InfoPopupUI.ForceShowMenu();
    }

    private int GetHitsRequired()
    {
      int num1 = 0;
      int weaponAndBlockClass = ConfigData.GetHitForceFromWeaponAndBlockClass(MineBot.pickaxe, Globals.player.GetTopArmBlockType(), Globals.playerData.GetDamageMultiplier());
      int num2 = ConfigData.GetHitsRequired(MineBot.pickaxe) / weaponAndBlockClass;
      if ((double) ConfigData.GetHitsRequired(MineBot.pickaxe) / (double) weaponAndBlockClass > (double) (ConfigData.GetHitsRequired(MineBot.pickaxe) / weaponAndBlockClass))
        num1 = 1;
      return num2 + num1;
    }

    public void JoinMine(int level)
    {
      MineBot.joiningMine = true;
      Globals.mineBot.state = MineBot.BotState.JoinMine;
      this.currentMineIndex = level;
      BSONArray bsonArray = new BSONArray();
      ((BSONValue) bsonArray).Add(BSONValue.op_Implicit(level));
      BSONObject bsonObject = new BSONObject();
      ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("wlA");
      ((BSONValue) bsonObject)["WCSD"] = (BSONValue) bsonArray;
      OutgoingMessages.AddOneMessageToList(bsonObject);
      ChatCommand.Execute("/w mineworld");
      MelonLogger.Msg("Joining mine LVL" + (level + 1).ToString());
    }

    public void HandleJoinWorldResult(BSONObject bson)
    {
      WorldJoinResult int32Value = (WorldJoinResult) (int) (byte) ((BSONValue) bson)["JR"].int32Value;
      string str = BSONValue.op_Implicit(((BSONValue) bson)["WN"]);
      if (int32Value == 0)
      {
        if (!(str == this.bufferWorld))
          return;
        this.state = MineBot.BotState.InBufferWorld;
      }
      else
      {
        if (MineBot.useWebhook)
          this.SendErrorFatal("WorldJoinFailed", string.Format("The bot could not join the world: {0} (TO: {1})", (object) int32Value, (object) str), "not supported");
        MineBot.Stop(string.Format("Join World failed: world {0}, reason: {1}", (object) str, (object) int32Value));
      }
    }

    public void StartMining()
    {
      if (Globals.world.worldName != "MINEWORLD")
        return;
      MineBot.joiningMine = false;
      this.state = MineBot.BotState.SearchPath;
      this.path = Globals.teleport.pather.SkidBestMinePath();
      this.path.RemoveAt(0);
      this.state = MineBot.BotState.Moving;
      if (!MineBot.showPath)
        return;
      foreach (PNode pnode in this.path)
        Globals.worldController.RemoteSetBlockBackground((World.BlockType) 3433, new Vector2i(pnode.X, pnode.Y));
    }

    public void Mine_OnFixedUpdate()
    {
      if (!MineBot.active)
        return;
      this.allTimePassed.AddMilliseconds((double) Time.fixedDeltaTime);
      if (Globals.world == null || Object.op_Equality((Object) ControllerHelper.rootUI, (Object) null) || Globals.world.worldName != "MINEWORLD" || this.state == MineBot.BotState.None || this.state != MineBot.BotState.Moving && this.state != MineBot.BotState.Breaking && this.state != MineBot.BotState.EnemyTriggered || this.path == null || this.path.Count <= 0)
        return;
      Globals.antiBounce = true;
      Globals.godMode = true;
      Globals.keysToFly = false;
      Globals.antiCollect = false;
      Globals.giveawayMode = false;
      if (Type.op_Inequality(Globals.rootUI.GetTopMostMenuItem(), Il2CppType.Of<GameplayUI>()))
        Globals.rootUI.MakeMenuTopMostInOrder(Il2CppType.Of<GameplayUI>());
      this.timePassed += Time.fixedDeltaTime;
      this.miningTimer += Time.fixedDeltaTime;
      if ((double) this.timePassed > 20.0 && ControllerHelper.networkClient.playerConnectionStatus == 7)
      {
        SceneLoader.GoFromMainMenuToWorld("SWEETHOUSE", "");
        this.timePassed = 0.0f;
        this.state = MineBot.BotState.JoinBufferWorld;
      }
      if ((double) this.timePassed > 300.0)
      {
        MelonLogger.Warning("It took too many time, Bugged?");
        this.StartMining();
        this.timePassed = 0.0f;
      }
      if ((double) this.miningTimer < (double) this.miningSpeed || this.path.Count <= 0)
        return;
      this.miningTimer = 0.0f;
      if (MineBot.nigaMode)
        Globals.invisHack = true;
      if (this.HitEnemyBasedOnPriority())
      {
        Il2CppArrayBase<AIEnemyMonoBehaviourBase> objectsOfType = Object.FindObjectsOfType<AIEnemyMonoBehaviourBase>();
        AIEnemyMonoBehaviourBase monoBehaviourBase1 = (AIEnemyMonoBehaviourBase) null;
        float num1 = 1f;
        foreach (AIEnemyMonoBehaviourBase monoBehaviourBase2 in objectsOfType)
        {
          AIEnemyMonoBehaviourBase monoBehaviourBase3 = monoBehaviourBase2;
          if (Object.op_Implicit((Object) monoBehaviourBase3) && AIEnemyConfigData.GetMaxHitPoints(monoBehaviourBase2.aiBase.enemyType) != 50)
          {
            float num2 = Vector3.Distance(monoBehaviourBase2.tempPosition, Globals.player.currentPlayerPosition);
            if ((double) num2 < (double) num1 && AIEnemies.CanPlayerHitAIEnemy(monoBehaviourBase2.aiBase.id))
            {
              num1 = num2;
              monoBehaviourBase1 = monoBehaviourBase3;
            }
          }
        }
        if (Object.op_Implicit((Object) monoBehaviourBase1) && monoBehaviourBase1.aiBase.health > 0 && Utils.GetMapPointsGridInRange(((Il2CppArrayBase<int>) ConfigData.range)[(int) Globals.player.GetTopArmBlockType()]).Contains(monoBehaviourBase1.aiBase.GetRoundedMapPoint()))
        {
          Globals.mineBot.state = MineBot.BotState.EnemyTriggered;
          OutgoingMessages.SendHitAIEnemyMessage(monoBehaviourBase1.aiBase.GetRoundedMapPoint(), monoBehaviourBase1.aiBase.id, -1);
          return;
        }
      }
      if (!Globals.teleport.provider.IsTileWalkable(this.path[0].X, this.path[0].Y))
      {
        Globals.mineBot.state = MineBot.BotState.Breaking;
        if (Globals.world.GetBlockHitsLeft(this.path[0].X, this.path[0].Y) < 0)
        {
          if (this.roll.Next(0, 41) % 9 != 0 && this.path.Count > 1)
          {
            List<Vector2i> pointsGridInRange = Utils.GetMapPointsGridInRange(((Il2CppArrayBase<int>) ConfigData.range)[(int) Globals.player.GetTopArmBlockType()]);
            if (Globals.world.GetBlockHitsLeft(this.path[1].X, this.path[1].Y) > 0 && !Globals.teleport.provider.IsTileWalkable(this.path[1].X, this.path[1].Y))
              Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[1], Globals.player.GetTopArmBlockType(), true, false);
            else if (Globals.world.GetBlockHitsLeft(this.path[2].X, this.path[2].Y) > 0 && !Globals.teleport.provider.IsTileWalkable(this.path[2].X, this.path[2].Y) && pointsGridInRange.Contains((Vector2i) this.path[2]))
              Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[2], Globals.player.GetTopArmBlockType(), true, false);
            else if (Globals.world.GetBlockHitsLeft(this.path[3].X, this.path[3].Y) > 0 && !Globals.teleport.provider.IsTileWalkable(this.path[3].X, this.path[3].Y) && pointsGridInRange.Contains((Vector2i) this.path[3]))
              Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[3], Globals.player.GetTopArmBlockType(), true, false);
            else if (Globals.world.GetBlockHitsLeft(this.path[4].X, this.path[4].Y) > 0 && !Globals.teleport.provider.IsTileWalkable(this.path[4].X, this.path[4].Y) && pointsGridInRange.Contains((Vector2i) this.path[4]))
              Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[4], Globals.player.GetTopArmBlockType(), true, false);
            else
              Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[0], Globals.player.GetTopArmBlockType(), true, false);
            pointsGridInRange.Clear();
          }
          else
            Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[0], Globals.player.GetTopArmBlockType(), true, false);
        }
        else
          Globals.worldController.MineBlockWithTool(0.0f, false, (Vector2i) this.path[0], Globals.player.GetTopArmBlockType(), true, false);
      }
      else
      {
        Globals.mineBot.state = MineBot.BotState.Moving;
        try
        {
          ((Component) Globals.player).transform.position = Globals.worldController.ConvertPlayerMapPointToWorldPoint((Vector2i) this.path[0]);
          OutgoingMessages.recentMapPoints.Add((Vector2i) this.path[0]);
          this.lastMP = (Vector2i) this.path[0];
          if (!MineBot.nigaMode)
          {
            if (this.path.Count > 1 && this.lastMP.y != this.path[1].Y && this.lastMP.x == this.path[1].X)
              this.miningTimer += this.miningSpeed / 1.25f;
            else
              this.miningTimer += this.miningSpeed / 2f;
          }
          else
            this.miningTimer += 1.42857146f;
          if (Globals.world.GetBlockType((Vector2i) this.path[0]) == 3966)
            OutgoingMessages.SendRequestGeneratedMineExitMessage((Vector2i) this.path[0]);
        }
        catch (Exception ex)
        {
        }
        this.path.RemoveAt(0);
        if (MineBot.showPath)
          Globals.worldController.RemoteSetBlockBackground((World.BlockType) 0, (Vector2i) this.path[0]);
      }
    }

    public static bool IsMiningRelatedItem(World.BlockType bt)
    {
      return ConfigData.IsMiningNugget(bt) || ConfigData.IsBlockMiningGemstone(bt) || ConfigData.IsMiningIngredient(bt) && (ConfigData.GetMiningIngredientRarity(new PlayerData.InventoryKey(bt, (PlayerData.InventoryItemType) 7)) == 5 || MineBot.pickShit) || bt == 4593 || ConfigData.IsConsumableTreasureChest(bt) || ConfigData.IsConsumableTreasurePouch(bt) || MineBot.pickShit && ConfigData.IsMiningIngredient(bt);
    }

    public int GetMineLevelPriority()
    {
      if (Globals.playerData == null)
        return -1;
      if (Globals.playerData.HasItemAmountInInventory((World.BlockType) 3972, (PlayerData.InventoryItemType) 7, (short) 1) && Globals.mineBot.mineLevels[4])
        return 4;
      if (Globals.playerData.HasItemAmountInInventory((World.BlockType) 3971, (PlayerData.InventoryItemType) 7, (short) 1) && Globals.mineBot.mineLevels[3])
        return 3;
      if (Globals.playerData.HasItemAmountInInventory((World.BlockType) 3970, (PlayerData.InventoryItemType) 7, (short) 1) && Globals.mineBot.mineLevels[2])
        return 2;
      return Globals.playerData.HasItemAmountInInventory((World.BlockType) 3969, (PlayerData.InventoryItemType) 7, (short) 1) && Globals.mineBot.mineLevels[1] ? 1 : 0;
    }

    public bool HitEnemyBasedOnPriority()
    {
      return this.EPriority != MineBot.EnemyPriority.Ignore && (this.EPriority != MineBot.EnemyPriority.IfLevel5AndNear || this.currentMineIndex == 4) && (this.EPriority != MineBot.EnemyPriority.KillIfLevel5 || this.currentMineIndex == 4);
    }

    public bool CheckOverload(bool action = true)
    {
      bool flag = false;
      foreach (PlayerData.InventoryKey inventoryKey in (Il2CppArrayBase<PlayerData.InventoryKey>) Globals.playerData.GetInventoryAsOrderedByInventoryItemType())
      {
        if (ConfigData.IsMiningNugget(inventoryKey.blockType) && Globals.playerData.GetCount(inventoryKey) >= (short) 100)
        {
          flag = true;
          if (!action)
            return true;
          OutgoingMessages.ConvertItems(inventoryKey);
          Globals.playerData.RemoveItemsFromInventory(inventoryKey, (short) 100);
          Globals.playerData.AddItemToInventory(ConfigData.GetConversionResultBySourceType(inventoryKey.blockType), (InventoryItemBase) null);
          MelonLogger.Msg("Crafting " + TextManager.GetBlockTypeName(ConfigData.GetConversionResultBySourceType(inventoryKey.blockType).blockType));
        }
        else if (inventoryKey.blockType == 4171 && Globals.playerData.GetCount(inventoryKey) >= (short) 998)
        {
          if (!action)
            return true;
          OutgoingMessages.SendSpinMiningRouletteMessage();
          Globals.playerData.RemoveItemFromInventory(inventoryKey);
          MelonLogger.Msg("Token OVERSTACK, spinning 1 roulette");
        }
        else if (Globals.world.worldName == "MINEWORLD" && ConfigData.IsBlockMiningGemstone(inventoryKey.blockType) && Globals.playerData.GetCount(inventoryKey) >= (short) 987)
        {
          flag = true;
          if (!action)
            return true;
          OutgoingMessages.RecycleMiningGemstone(inventoryKey, (short) 12);
          Globals.playerData.RemoveItemsFromInventory(inventoryKey, (short) 12);
          Globals.playerData.AddGems(ConfigData.GetGemstoneRecycleValueForMiningGemstoneRecycler(inventoryKey.blockType) * 12);
          MelonLogger.Msg("Gem Overload [InMine], recycling x12 of " + TextManager.GetBlockTypeName(inventoryKey.blockType));
        }
        else if ((ConfigData.IsBlockMiningGemstone(inventoryKey.blockType) || ConfigData.IsConsumableTreasureChest(inventoryKey.blockType) || ConfigData.IsConsumableTreasurePouch(inventoryKey.blockType) || ConfigData.IsMiningIngredient(inventoryKey.blockType)) && Globals.playerData.GetCount(inventoryKey) >= (short) 950)
        {
          flag = true;
          if (!action)
            return true;
          if (Globals.world.worldName != "MINEWORLD")
          {
            Vector2i bufferWorldDropPos = this.GetBufferWorldDropPos();
            if (bufferWorldDropPos.x >= 0)
            {
              MelonLogger.Msg(string.Format("Overload: dropping {0} x{1}", (object) TextManager.GetBlockTypeName(inventoryKey.blockType), (object) Globals.playerData.GetCount(inventoryKey)));
              Globals.worldController.DropStuffFromInventory(bufferWorldDropPos, inventoryKey, Globals.playerData.GetCount(inventoryKey), (InventoryItemBase) null);
            }
          }
        }
        else if (Globals.world.worldName == this.bufferWorld && (inventoryKey.blockType == 3969 || inventoryKey.blockType == 3970 || inventoryKey.blockType == 3971 || inventoryKey.blockType == 3972) && Globals.playerData.GetCount(inventoryKey) >= (short) 999)
        {
          flag = true;
          if (!action)
            return true;
          Vector2i bufferWorldDropPos = this.GetBufferWorldDropPos();
          if (bufferWorldDropPos.x >= 0)
          {
            MelonLogger.Msg(string.Format("Overload: dropping {0} x{1}", (object) TextManager.GetBlockTypeName(inventoryKey.blockType), (object) Globals.playerData.GetCount(inventoryKey)));
            Globals.worldController.DropStuffFromInventory(bufferWorldDropPos, inventoryKey, Globals.playerData.GetCount(inventoryKey), (InventoryItemBase) null);
          }
        }
      }
      return flag;
    }

    public Vector2i GetBufferWorldDropPos()
    {
      Vector2i currentPlayerMapPoint = Globals.player.currentPlayerMapPoint;
      if (Globals.world.IsCollectableAmountFull())
      {
        if (MineBot.useWebhook)
          this.SendErrorFatal("WorldCollectableLimit", "World collectables amount reached in buffer world (" + Globals.world.worldName + ")", "");
        MineBot.Stop("WorldCollectableLimit: world collectables amount reached in buffer world (" + Globals.world.worldName + ")");
        return new Vector2i(-1, -1);
      }
      if (Globals.world.IsValidItemDropPoint(new Vector2i(currentPlayerMapPoint.x - 1, currentPlayerMapPoint.y)) && !Globals.world.IsCollectableAmountFullInMapPoint(new Vector2i(currentPlayerMapPoint.x - 1, currentPlayerMapPoint.y)))
        return new Vector2i(currentPlayerMapPoint.x - 1, currentPlayerMapPoint.y);
      if (Globals.world.IsValidItemDropPoint(new Vector2i(currentPlayerMapPoint.x + 1, currentPlayerMapPoint.y)) && !Globals.world.IsCollectableAmountFullInMapPoint(new Vector2i(currentPlayerMapPoint.x + 1, currentPlayerMapPoint.y)))
        return new Vector2i(currentPlayerMapPoint.x + 1, currentPlayerMapPoint.y);
      if (MineBot.useWebhook)
        this.SendErrorFatal("DropPointNotValid", "there was no valid drop point in buffer world (" + Globals.world.worldName + ")", "");
      MineBot.Stop("DropPointNotValid: there was no valid drop point in buffer world (" + Globals.world.worldName + ")");
      return new Vector2i(-1, -1);
    }

    public void SetWebhook(string url)
    {
      if (!url.ToLower().StartsWith("https://discord.com/api/webhooks") && !url.ToLower().StartsWith("https://discordapp.com/api/webhooks"))
      {
        Utils.Error("Not a discord webhook!");
        Utils.DoCustomNotification("Invalid webhook url", Globals.player.currentPlayerMapPoint);
      }
      else
      {
        if (this.webhook == url)
          return;
        this.webhook = url;
        MelonLogger.Msg("Webhook set: " + url);
        try
        {
          DiscordManager.SendToWebhook(this.webhook, JsonConvert.SerializeObject((object) new
          {
            username = "BModSlave",
            avatar_url = "https://cdn.discordapp.com/app-assets/1216137919857819710/1216320682485612594.png",
            embeds = new \u003C\u003Ef__AnonymousType1<string, int, \u003C\u003Ef__AnonymousType2<string>>[1]
            {
              new
              {
                title = "This is test message.",
                color = 2071552,
                author = new
                {
                  name = "Webhook has been set"
                }
              }
            }
          }));
        }
        catch (Exception ex)
        {
          MelonLogger.BigError("Webhook", ex.Message);
        }
      }
    }

    public void SendErrorFatal(string why, string desc, string map)
    {
      if (this.webhook == "")
        return;
      DiscordManager.SendToWebhook(this.webhook, JsonConvert.SerializeObject((object) new
      {
        username = "BModSlave",
        avatar_url = "https://cdn.discordapp.com/app-assets/1216137919857819710/1216320682485612594.png",
        embeds = new \u003C\u003Ef__AnonymousType3<string, string, int, \u003C\u003Ef__AnonymousType4<string>>[1]
        {
          new
          {
            title = "An error occurred that forced the bot to stop",
            description = desc + "\n\n**Mining Results**\n" + string.Format("> <:clock:1236047865256742952> {0}mins\n", (object) Math.Round(DateTime.Now.Subtract(this.allTimePassed).TotalMinutes, 0)) + string.Format("> <:minegems:1236040783476559873> {0}\n", (object) this.gems) + string.Format("> <:xp:1236040794281082972> {0}\n", (object) this.xp) + string.Format("> <:exit:1236047872277876756> {0}\n", (object) this.minesCleared) + string.Format("> <:coin:1236047867378929844> {0}\n", (object) Globals.playerData.GetCount(new PlayerData.InventoryKey((World.BlockType) 4171, (PlayerData.InventoryItemType) 7))) + string.Format("> <:darkstone:1236047869421686847> {0}\n", (object) Globals.playerData.GetCount(new PlayerData.InventoryKey((World.BlockType) 4170, (PlayerData.InventoryItemType) 7))) + string.Format("> <:enemy:1236040776828457020> {0}\n", (object) this.enemyKilled) + "Positions (beta)\n" + map,
            color = 16532992,
            footer = new
            {
              text = (Globals.playerData == null ? "Unknown username" : StaticPlayer.theRealPlayername) + " | " + why + " | " + (Globals.world.worldName == "MINEWORLD" ? string.Format("MINEWORLD, {0}LVL", (object) (this.currentMineIndex + 1)) : Globals.world.worldName)
            }
          }
        }
      }));
    }

    public string GenerateGridMapAsJSON(short range)
    {
      string gridMapAsJson = "";
      List<Vector2i> pointsGridInRange = Utils.GetMapPointsGridInRange((int) range);
      int num = 0;
      foreach (Vector2i vector2i in pointsGridInRange)
      {
        World.BlockType blockType = Globals.world.GetBlockType(vector2i);
        gridMapAsJson = blockType != null || Globals.world.IsCollectableAmountFullInMapPoint(vector2i) ? (!ConfigData.IsBlockPlatform(blockType) ? (!ConfigData.IsAnyDoor(blockType) ? (!ConfigData.IsBlockPortalWireable(blockType) ? (blockType != 110 ? (!ConfigData.IsBlockMiningGemstoneBlock(blockType) ? (blockType != 3980 && blockType != 3981 && blockType != 3982 && blockType != 3983 && blockType != 3984 ? (blockType != 3992 && blockType != 3985 && blockType != 3986 ? (blockType != 3989 ? (blockType != 3991 ? (blockType != 3994 ? (blockType != 3993 && blockType != 3990 && blockType != 3988 ? (blockType != 3 && blockType != 344 && blockType != 343 ? (blockType != 3974 && blockType != 3978 && blockType != 3979 ? (!ConfigData.DoesBlockHaveCollider(blockType) ? (Globals.world.IsCollectableAmountFullInMapPoint(vector2i) ? gridMapAsJson + "<:full:1236395410684383255>" : gridMapAsJson + "<:none:1236040785229647902>") : gridMapAsJson + "<:obstacle:1236040787196645387>") : gridMapAsJson + "<:light:1236416224666058762>") : gridMapAsJson + "<:end:1236416222229168251>") : gridMapAsJson + "<:minebedrock:1236416226792570960>") : gridMapAsJson + "<:woodblock:1236416232694091937>") : gridMapAsJson + "<:rockmedium:1236416228906631218>") : gridMapAsJson + "<:rocksoft:1236416231024754738>") : gridMapAsJson + "<:rockhard:1236413415396999258>") : gridMapAsJson + "<:soilmining:1236413417351545054>") : gridMapAsJson + "<:gemstone:1236040781299454012>") : gridMapAsJson + "<:entrance:1236040778850107473>") : gridMapAsJson + "<:portal:1236040791697391677>") : gridMapAsJson + "<:door:1236040774068732025>") : gridMapAsJson + "<:platform:1236040788375502951>") : gridMapAsJson + "<:none:1236040785229647902>";
        ++num;
        if (num >= (int) range * 2 + 1)
        {
          num = 0;
          gridMapAsJson += "\n";
        }
      }
      return gridMapAsJson;
    }

    public void ResetStats()
    {
      this.allTimePassed = new DateTime();
      this.gems = 0;
      this.xp = 0;
      this.minesCleared = 0;
      this.enemyKilled = 0;
    }

    public void WorldRejoinFail(WorldJoinResult wjr)
    {
      MineBot.Stop(string.Format("Could not rejoin after disconnect, join world fail: {0}", (object) wjr));
    }

    public enum BotState
    {
      None,
      SearchPath,
      Moving,
      Breaking,
      JoinMine,
      JoinBufferWorld,
      InBufferWorld,
      EnemyTriggered,
      ReconnectIn20s,
      Reconnecting,
    }

    public enum EnemyPriority
    {
      Ignore,
      IfLevel5AndNear,
      KillIfNear,
      KillIfLevel5,
      AlwaysKill,
    }
  }
}
