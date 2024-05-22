
using BMod.pathfinding;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppKernys.Bson;
using Il2CppSystem;
using MelonLoader;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

 
namespace BMod
{
  internal class Teleport
  {
    public bool isTeleporting = false;
    public bool fromSpawn = false;
    public bool fromSpawnSent = false;
    public bool backFromSpawn = false;
    public bool backFromSpawnSent = false;
    public int sentTicks = 0;
    public bool toTake = false;
    public bool giftTake = false;
    public bool takeSent = false;
    public bool itemDrop = false;
    public PlayerData.InventoryKey itemToDrop = PlayerData.InventoryKey.GetNoneBlockKey();
    public GiftBoxData giftData = new GiftBoxData(1);
    public List<PNode> teleportingPath;
    public List<PNode> teleportingBackPath;
    internal Pathfinding pather;
    public TileProvider provider;
    public readonly int MAX_IN_TICK = 15;

    public Teleport()
    {
      this.pather = new Pathfinding();
      this.provider = (TileProvider) new ShiukiAI(1, 1);
    }

    public List<PNode> GetPath(Vector2i targetMP, out PathfindingResult pathfindingResult)
    {
      this.ResetMap();
      this.toTake = false;
      List<PNode> path;
      PathfindingResult pathfindingResult1 = this.pather.Run(Globals.player.currentPlayerMapPoint.x, Globals.player.currentPlayerMapPoint.y, targetMP.x, targetMP.y, this.provider, out path);
      pathfindingResult = pathfindingResult1;
      return pathfindingResult1 == PathfindingResult.SUCCESSFUL ? path : (List<PNode>) null;
    }

    public List<Vector2i> GetPathToPortal(Vector2i targetMP)
    {
      BSONObject asBson1 = Globals.world.GetWorldItemData(targetMP).GetAsBSON();
      List<Vector2i> pathToPortal = new List<Vector2i>();
      for (int index1 = 0; index1 < Globals.world.worldSizeY; ++index1)
      {
        for (int index2 = 0; index2 < Globals.world.worldSizeX; ++index2)
        {
          World.BlockType blockType = Globals.world.GetBlockType(new Vector2i(index2, index1));
          if (blockType != null && Globals.world.GetWorldItemData(new Vector2i(index2, index1)) != null && ConfigData.DoesBlockHaveCollider(blockType))
          {
            if (Globals.world.DoesPlayerHaveRightToModifyItemData(new Vector2i(index2, index1), Globals.playerData, false))
              pathToPortal.Add(new Vector2i(index2, index1));
            else if (blockType == 1799 || blockType == 4373 || blockType == 2001)
              pathToPortal.Add(new Vector2i(index2, index1));
            else if (ConfigData.IsBlockPortal(blockType) && blockType != 110)
            {
              BSONObject asBson2 = Globals.world.GetWorldItemData(new Vector2i(index2, index1)).GetAsBSON();
              if (BSONValue.op_Inequality((BSONValue) asBson2, (Object) null) && BSONValue.op_Inequality((BSONValue) asBson1, (Object) null))
              {
                if (((BSONValue) asBson2)["targetEntryPointID"].stringValue == ((BSONValue) asBson1)["entryPointID"].stringValue && Globals.world.DoesPlayerHaveRightToGoPortal(Globals.playerData, targetMP))
                  pathToPortal.Add(new Vector2i(index2, index1));
                else if (!((BSONValue) asBson2)["isLocked"].boolValue && ((BSONValue) asBson2)["targetWorldID"].stringValue.ToUpper() != Globals.world.worldName.ToUpper() && !string.IsNullOrWhiteSpace(((BSONValue) asBson2)["targetWorldID"].stringValue))
                  pathToPortal.Add(new Vector2i(index2, index1));
              }
            }
          }
        }
      }
      return pathToPortal;
    }

    private void ResetMap()
    {
      this.provider.ResetSize(Globals.world.worldSizeX, Globals.world.worldSizeY);
    }

    public PathfindingResult tryTeleportTo(int fromX, int fromY, int toX, int toY, bool log = true)
    {
      if (this.isTeleporting)
      {
        if (log)
          Utils.Msg("Already teleporting...");
        return PathfindingResult.CANCELLED;
      }
      this.ResetMap();
      this.toTake = false;
      Vector2i vector2i;
      // ISSUE: explicit constructor call
      ((Vector2i) ref vector2i).\u002Ector(Globals.world.playerStartPosition.x, Globals.world.playerStartPosition.y);
      List<PNode> path1;
      PathfindingResult pathfindingResult1 = this.pather.Run(fromX, fromY, toX, toY, this.provider, out path1);
      List<PNode> path2;
      PathfindingResult pathfindingResult2 = this.pather.Run(vector2i.x, vector2i.y, toX, toY, this.provider, out path2);
      bool flag = false;
      if (pathfindingResult2 == PathfindingResult.SUCCESSFUL && (pathfindingResult1 != PathfindingResult.SUCCESSFUL || path2.Count < path1.Count))
      {
        path1 = path2;
        pathfindingResult1 = pathfindingResult2;
        flag = true;
      }
      if (pathfindingResult1 != 0)
      {
        if (log)
          Utils.Msg("PathTo not found: " + pathfindingResult1.ToString());
        return pathfindingResult1;
      }
      path1.RemoveAt(0);
      if (log)
        Utils.Msg(string.Format("Teleporting {0} Tiles... (~{1}s.)", (object) path1.Count, (object) (float) ((double) (path1.Count / this.MAX_IN_TICK) * 1.5)));
      this.fromSpawn = flag;
      this.fromSpawnSent = false;
      this.teleportingPath = path1;
      this.isTeleporting = true;
      this.sentTicks = 0;
      Teleport.onTeleportTimer((object) null, (ElapsedEventArgs) null);
      return pathfindingResult1;
    }

    public void tryTeleportToTake(int fromX, int fromY, int toX, int toY, bool log = true, bool toDrop = false)
    {
      if (this.isTeleporting)
      {
        if (!log)
          return;
        Utils.Msg("Already teleporting...");
      }
      else
      {
        this.ResetMap();
        this.itemDrop = false;
        this.toTake = false;
        this.giftTake = false;
        this.backFromSpawn = false;
        Vector2i vector2i1;
        // ISSUE: explicit constructor call
        ((Vector2i) ref vector2i1).\u002Ector(Globals.world.playerStartPosition.x, Globals.world.playerStartPosition.y);
        List<PNode> path1;
        PathfindingResult pathfindingResult1 = this.pather.Run(fromX, fromY, toX, toY, this.provider, out path1);
        List<PNode> path2;
        PathfindingResult pathfindingResult2 = this.pather.Run(vector2i1.x, vector2i1.y, toX, toY, this.provider, out path2);
        bool flag = false;
        if (pathfindingResult2 == PathfindingResult.SUCCESSFUL && (pathfindingResult1 != PathfindingResult.SUCCESSFUL || path2.Count < path1.Count))
        {
          path1 = path2;
          pathfindingResult1 = pathfindingResult2;
          flag = true;
        }
        WorldItemBase worldItemData = Globals.world.GetWorldItemData(new Vector2i(toX, toY));
        if (worldItemData != null)
        {
          World.BlockType blockType = worldItemData.blockType;
          if (blockType == 966 || blockType == 1392 || blockType == 2066)
          {
            ((WorldItemBase) this.giftData).SetViaBSON(worldItemData.GetAsBSON());
            if (this.giftData.takeAmount < 1 || this.giftData.itemAmount < 1)
            {
              if (!log)
                return;
              Utils.Msg("GiftBox is empty!");
              return;
            }
            this.giftTake = true;
            this.toTake = true;
          }
        }
        if (!toDrop && !this.giftTake)
        {
          Vector2i vector2i2;
          // ISSUE: explicit constructor call
          ((Vector2i) ref vector2i2).\u002Ector(toX, toY);
          foreach (CollectableData collectable in Globals.world.collectables)
          {
            if (collectable.mapPoint.Equals((object) vector2i2))
            {
              this.toTake = true;
              break;
            }
          }
        }
        if (!toDrop && !this.toTake)
        {
          World.BlockType blockType = Globals.world.GetBlockType(new Vector2i(toX, toY));
          if (blockType <= 329)
          {
            if (blockType != 295 && blockType != 329)
              goto label_30;
          }
          else if (blockType != 3438 && blockType - 3600 > 1 && blockType != 3964)
            goto label_30;
          PortalData portalData = new PortalData(1);
          ((WorldItemBase) portalData).SetViaBSON(worldItemData.GetAsBSON());
          if (portalData.entryPointID == "")
          {
            Utils.Msg("PortalId is null.");
            return;
          }
          SceneLoader.CheckIfWeCanGoFromWorldToWorld(Globals.world.worldName, portalData.entryPointID, (Action<WorldJoinResult>) null, false, (Action<WorldJoinResult>) null);
          MelonLogger.Msg("Trying join " + Globals.world.worldName + " " + portalData.entryPointID);
          return;
label_30:
          if (!log)
            return;
          Utils.Msg("Nothing found...");
        }
        else
        {
          try
          {
            if (pathfindingResult1 != 0)
            {
              if (!log)
                return;
              Utils.Msg("PathTo not found: " + pathfindingResult1.ToString());
            }
            else
            {
              PNode pnode = PNode.Create(0, 0);
              for (int index = 0; index < path1.Count; ++index)
                pnode = path1[index];
              List<PNode> path3;
              PathfindingResult pathfindingResult3 = this.pather.Run(pnode.X, pnode.Y, fromX, fromY, this.provider, out path3);
              List<PNode> path4;
              PathfindingResult pathfindingResult4 = this.pather.Run(vector2i1.x, vector2i1.y, fromX, fromY, this.provider, out path4);
              if (pathfindingResult4 == PathfindingResult.SUCCESSFUL && (pathfindingResult3 != PathfindingResult.SUCCESSFUL || path4.Count < path3.Count))
              {
                path3 = path4;
                pathfindingResult3 = pathfindingResult4;
                this.backFromSpawn = true;
              }
              if (pathfindingResult3 != 0)
              {
                if (!log)
                  return;
                Utils.Msg("PathToBack not found: " + path3.ToString());
              }
              else
              {
                path1.RemoveAt(0);
                if (log)
                  Utils.Msg("Trying to " + (toDrop ? "drop" : "collect") + string.Format(". {0} Tiles... (~{1}s.)", (object) (path1.Count + path3.Count), (object) (float) ((double) ((path1.Count + path3.Count) / this.MAX_IN_TICK) * 1.5)));
                this.fromSpawn = flag;
                this.fromSpawnSent = false;
                this.teleportingPath = path1;
                this.teleportingBackPath = path3;
                this.takeSent = false;
                this.sentTicks = 0;
                if (toDrop)
                {
                  this.itemDrop = true;
                  this.toTake = true;
                  this.giftTake = false;
                }
                this.backFromSpawnSent = false;
                this.isTeleporting = true;
                Teleport.onTeleportTimer((object) null, (ElapsedEventArgs) null);
              }
            }
          }
          catch
          {
            Utils.Msg("PathToBack not found.");
          }
        }
      }
    }

    public PathfindingResult tryTeleportTo(Vector2i from, Vector2i to, bool log = true)
    {
      return this.tryTeleportTo(from.x, from.y, to.x, to.y, log);
    }

    public PathfindingResult tryTeleportTo(Vector2i from, Vector2 to, bool log = true)
    {
      return this.tryTeleportTo(from.x, from.y, (int) to.x, (int) to.y, log);
    }

    public void tryTeleportToTake(Vector2i from, Vector2i to, bool log = true, bool toDrop = false)
    {
      this.tryTeleportToTake(from.x, from.y, to.x, to.y, log, toDrop);
    }

    public void tryTeleportToTake(Vector2i from, Vector2 to, bool log = true, bool toDrop = false)
    {
      this.tryTeleportToTake(from.x, from.y, (int) to.x, (int) to.y, log, toDrop);
    }

    public static void onTeleportTimer(object source, ElapsedEventArgs e)
    {
      if (!Globals.teleport.isTeleporting)
        return;
      ++Globals.teleport.sentTicks;
      if (Globals.teleport.sentTicks > 2)
      {
        Globals.teleport.sentTicks = 0;
      }
      else
      {
        if (Globals.teleport.fromSpawn && !Globals.teleport.fromSpawnSent)
        {
          Globals.teleport.fromSpawnSent = true;
          OutgoingMessages.SendResurrect(KukouriTime.Get(), Globals.world.playerStartPosition);
          OutgoingMessages.recentMapPoints.Add(Globals.world.playerStartPosition);
        }
        PNode pnode1 = PNode.Create(0, 0);
        PNode pnode2 = PNode.Create(0, 0);
        List<PNode> pnodeList1 = new List<PNode>((IEnumerable<PNode>) Globals.teleport.teleportingPath);
        int num1 = Globals.teleport.MAX_IN_TICK;
        if (pnodeList1.Count < num1)
          num1 = pnodeList1.Count;
        for (int index = 0; index < num1; ++index)
        {
          pnode1 = pnode2;
          PNode pnode3 = pnodeList1[index];
          OutgoingMessages.recentMapPoints.Add(new Vector2i(pnode3.X, pnode3.Y));
          pnode2 = pnode3;
          Globals.teleport.teleportingPath.RemoveAt(0);
        }
        if (Globals.teleport.teleportingPath.Count >= 1)
          return;
        if (Globals.teleport.toTake)
        {
          if (!Globals.teleport.takeSent)
          {
            Globals.teleport.takeSent = true;
            if (Globals.teleport.giftTake)
            {
              OutgoingMessages.SendRequestItemFromGiftBoxMessage(new Vector2i(pnode2.X, pnode2.Y));
            }
            else
            {
              if (Globals.teleport.itemDrop)
              {
                OutgoingMessages.SendDropItemMessage(new Vector2i(pnode2.X, pnode2.Y), Globals.teleport.itemToDrop, (short) 1, Globals.playerData.GetInventoryData(Globals.teleport.itemToDrop));
                return;
              }
              Vector2 vector2_1;
              // ISSUE: explicit constructor call
              ((Vector2) ref vector2_1).\u002Ector((float) pnode2.X, (float) pnode2.Y);
              foreach (CollectableData collectable in Globals.world.collectables)
              {
                Vector2 vector2_2;
                // ISSUE: explicit constructor call
                ((Vector2) ref vector2_2).\u002Ector(collectable.posX, collectable.posY);
                if ((double) Vector2.Distance(vector2_2, vector2_1) < 5.0)
                  OutgoingMessages.SendCollectCollectableMessage(collectable.id);
              }
            }
          }
          if (Globals.teleport.backFromSpawn && !Globals.teleport.backFromSpawnSent)
          {
            Globals.teleport.backFromSpawnSent = true;
            OutgoingMessages.SendResurrect(KukouriTime.Get(), Globals.world.playerStartPosition);
            OutgoingMessages.recentMapPoints.Add(Globals.world.playerStartPosition);
          }
          List<PNode> pnodeList2 = new List<PNode>((IEnumerable<PNode>) Globals.teleport.teleportingBackPath);
          int num2 = Globals.teleport.MAX_IN_TICK;
          if (pnodeList2.Count < num2)
            num2 = pnodeList2.Count;
          for (int index = 0; index < num2; ++index)
          {
            PNode pnode4 = pnodeList2[index];
            OutgoingMessages.recentMapPoints.Add(new Vector2i(pnode4.X, pnode4.Y));
            pnode2 = pnode4;
            Globals.teleport.teleportingBackPath.RemoveAt(0);
          }
          if (Globals.teleport.teleportingBackPath.Count < 1)
          {
            Vector2 vector2 = Vector2.op_Implicit(Globals.worldController.ConvertMapPointToWorldPoint(new Vector2i(pnode2.X, pnode2.Y)));
            Globals.player.myTransform.position = new Vector3(vector2.x, vector2.y, 0.0f);
            Globals.teleport.isTeleporting = false;
          }
        }
        else
        {
          Vector2 vector2 = Vector2.op_Implicit(Globals.worldController.ConvertMapPointToWorldPoint(new Vector2i(pnode2.X, pnode2.Y)));
          Globals.player.myTransform.position = new Vector3(vector2.x, vector2.y, 0.0f);
          Globals.teleport.isTeleporting = false;
        }
      }
    }
  }
}
