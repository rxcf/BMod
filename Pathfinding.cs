
using BMod;
using BMod.Auto;
using BMod.KrakPath;
using Il2Cpp;
using Il2CppBasicTypes;
using MelonLoader;
using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
  public const int MAX = 5000;
  public const float DIAGONAL_DST = 1.41421354f;
  private FastPriorityQueue<PNode> open = new FastPriorityQueue<PNode>(5000);
  private Dictionary<PNode, PNode> cameFrom = new Dictionary<PNode, PNode>();
  private Dictionary<PNode, float> costSoFar = new Dictionary<PNode, float>();
  private List<PNode> near = new List<PNode>();
  private bool left;
  private bool right;
  private bool below;
  private bool above;
  public SkiddedMinePath skiddedMinePath = new SkiddedMinePath();
  public List<PNode> path = new List<PNode>();

  public PathfindingResult Run(
    int startX,
    int startY,
    int endX,
    int endY,
    TileProvider provider,
    out List<PNode> path)
  {
    if (provider == null)
    {
      path = (List<PNode>) null;
      return PathfindingResult.Path_Not_Found;
    }
    if (provider.TileInBounds(startX, startY))
      ;
    if (!provider.TileInBounds(endX, endY))
    {
      path = (List<PNode>) null;
      return PathfindingResult.ERROR_END_OUT_OF_BOUNDS;
    }
    if (provider.IsTileWalkable(startX, startY))
      ;
    if (!provider.IsTileWalkable(endX, endY) && !provider.IsBlockCloudOn(endX, endY) && !provider.IsBlockInstaKillOn(endX, endY) && !ConfigData.IsBlockPlatform(Globals.world.GetBlockType(endX, endY)))
    {
      path = (List<PNode>) null;
      return PathfindingResult.Invalid_Ending_Pos;
    }
    this.Clear();
    PNode pnode1 = PNode.Create(startX, startY);
    PNode end = PNode.Create(endX, endY);
    this.open.Enqueue(pnode1, 0.0f);
    this.cameFrom[pnode1] = pnode1;
    this.costSoFar[pnode1] = 0.0f;
    int count;
    while ((count = this.open.Count) > 0)
    {
      if (count >= 4992)
      {
        path = (List<PNode>) null;
        return PathfindingResult.ERROR_PATH_TOO_LONG;
      }
      PNode pnode2 = this.open.Dequeue();
      Globals.lastpos = new Vector2i(pnode2.X, pnode2.Y);
      if (pnode2.Equals((object) end))
      {
        path = this.TracePath(end);
        return PathfindingResult.SUCCESSFUL;
      }
      foreach (PNode pnode3 in this.GetNear(pnode2, provider))
      {
        float num = this.costSoFar[pnode2] + this.GetCost(pnode2, pnode3);
        if (!this.costSoFar.ContainsKey(pnode3) || (double) num < (double) this.costSoFar[pnode3])
        {
          this.costSoFar[pnode3] = num;
          float priority = num + this.Heuristic(pnode2, pnode3);
          this.open.Enqueue(pnode3, priority);
          this.cameFrom[pnode3] = pnode2;
        }
      }
    }
    path = (List<PNode>) null;
    return PathfindingResult.Path_Not_Found;
  }

  private List<PNode> TracePath(PNode end)
  {
    List<PNode> pnodeList = new List<PNode>();
    PNode key = end;
    bool flag = true;
    while (flag)
    {
      PNode pnode = this.cameFrom[key];
      pnodeList.Add(key);
      if (pnode != null && key != pnode)
        key = pnode;
      else
        flag = false;
    }
    pnodeList.Reverse();
    return pnodeList;
  }

  public void Clear()
  {
    this.costSoFar.Clear();
    this.cameFrom.Clear();
    this.near.Clear();
    this.open.Clear();
  }

  private float Abs(float x) => (double) x < 0.0 ? -x : x;

  private float Heuristic(PNode a, PNode b)
  {
    return this.Abs((float) (a.X - b.X)) + this.Abs((float) (a.Y - b.Y));
  }

  private float GetCost(PNode a, PNode b)
  {
    return (double) this.Abs((float) (a.X - b.X)) == 1.0 && a.Y == b.Y || (double) this.Abs((float) (a.Y - b.Y)) == 1.0 && a.X == b.X ? 1f : 1.41421354f;
  }

  private List<PNode> GetNear(PNode node, TileProvider provider)
  {
    this.near.Clear();
    this.left = provider.IsTileWalkable(node.X - 1, node.Y);
    this.right = provider.IsTileWalkable(node.X + 1, node.Y);
    this.above = provider.IsTileWalkable(node.X, node.Y + 1);
    this.below = provider.IsTileWalkable(node.X, node.Y - 1);
    bool flag1 = provider.IsBlockInstaKillOn(node.X - 1, node.Y + 1) || provider.IsTileWalkable(node.X - 1, node.Y + 1);
    bool flag2 = provider.IsBlockInstaKillOn(node.X + 1, node.Y + 1) || provider.IsTileWalkable(node.X + 1, node.Y + 1);
    bool flag3 = provider.IsBlockInstaKillOn(node.X - 1, node.Y - 1) || provider.IsTileWalkable(node.X - 1, node.Y - 1);
    bool flag4 = provider.IsBlockInstaKillOn(node.X + 1, node.Y - 1) || provider.IsTileWalkable(node.X + 1, node.Y - 1);
    if (this.above)
      this.near.Add(PNode.Create(node.X, node.Y + 1));
    if (this.below && Globals.world.GetBlockType(node.X, node.Y) != 110)
      this.near.Add(PNode.Create(node.X, node.Y - 1));
    if (this.left)
      this.near.Add(PNode.Create(node.X - 1, node.Y));
    if (this.right)
      this.near.Add(PNode.Create(node.X + 1, node.Y));
    if (flag1 && (this.above || this.left))
      this.near.Add(PNode.Create(node.X - 1, node.Y + 1));
    if (flag2 && (this.above || this.right))
      this.near.Add(PNode.Create(node.X + 1, node.Y + 1));
    if (flag3 && (this.below || this.left))
      this.near.Add(PNode.Create(node.X - 1, node.Y - 1));
    if (flag4 && (this.below || this.right))
      this.near.Add(PNode.Create(node.X + 1, node.Y - 1));
    return this.near;
  }

  public List<Vector2i> GetDroppedItemsAndGemstonesPositions()
  {
    List<Vector2i> gemstonesPositions = new List<Vector2i>();
    if (Globals.mineBot.currentMineIndex != 0)
    {
      for (int index1 = Globals.world.worldSizeY - 1; index1 >= 0; --index1)
      {
        for (int index2 = 0; index2 < Globals.world.worldSizeX; ++index2)
        {
          if (ConfigData.IsBlockMiningGemstoneBlock(Globals.world.GetBlockType(index2, index1)))
            gemstonesPositions.Add(new Vector2i(index2, index1));
        }
      }
    }
    foreach (CollectableData collectable in Globals.world.collectables)
      gemstonesPositions.Add(new Vector2i(collectable.mapPoint.x, collectable.mapPoint.y));
    if (Globals.mineBot.EPriority == MineBot.EnemyPriority.KillIfLevel5 && Globals.mineBot.currentMineIndex == 4 || Globals.mineBot.EPriority == MineBot.EnemyPriority.AlwaysKill)
    {
      foreach (AIEnemyMonoBehaviourBase monoBehaviourBase1 in Object.FindObjectsOfType<AIEnemyMonoBehaviourBase>())
      {
        AIEnemyMonoBehaviourBase monoBehaviourBase2 = monoBehaviourBase1;
        if (Object.op_Implicit((Object) monoBehaviourBase2) && monoBehaviourBase2.isActive && AIEnemies.CanPlayerHitAIEnemy(monoBehaviourBase1.aiBase.id))
          gemstonesPositions.Add(monoBehaviourBase2.aiBase.GetRoundedMapPoint());
      }
    }
    return gemstonesPositions;
  }

  public List<PNode> SkidBestMinePath()
  {
    if (Globals.world.worldName != "MINEWORLD")
      return new List<PNode>();
    List<Vector2i> gemstonesPositions = this.GetDroppedItemsAndGemstonesPositions();
    List<Vector2i> vector2iList = new List<Vector2i>();
    Vector2i vector2i = Globals.player.currentPlayerMapPoint;
    this.path = new List<PNode>();
    if (gemstonesPositions.Count == 0)
    {
      this.path.AddRange((IEnumerable<PNode>) this.SkidMiningPath(vector2i, this.getMineExit()));
      MelonLogger.Msg("Bot not on exit? Something went wrong..");
      return this.path;
    }
    int count = gemstonesPositions.Count;
    for (int index = 0; vector2iList.Count < gemstonesPositions.Count && index < count; ++index)
    {
      float num1 = float.MaxValue;
      Vector2i end = new Vector2i();
      foreach (Vector2i point2 in gemstonesPositions)
      {
        if (!vector2iList.Contains(point2))
        {
          float num2 = Utils.Distance(vector2i, point2);
          if ((double) num2 < (double) num1)
          {
            num1 = num2;
            end = point2;
          }
        }
      }
      if (!end.Equals((object) new Vector2i()))
      {
        List<PNode> collection = this.SkidMiningPath(vector2i, end);
        vector2i = end;
        vector2iList.Add(end);
        this.path.AddRange((IEnumerable<PNode>) collection);
      }
      else
        break;
    }
    this.path.AddRange((IEnumerable<PNode>) this.SkidMiningPath(vector2i, this.getMineExit()));
    return this.path;
  }

  public Vector2i getMineExit()
  {
    for (int index1 = Globals.world.worldSizeY - 1; index1 >= 0; --index1)
    {
      for (int index2 = 0; index2 < Globals.world.worldSizeX; ++index2)
      {
        if (Globals.world.GetBlockType(index2, index1) == 3966)
          return new Vector2i(index2, index1);
      }
    }
    return new Vector2i();
  }

  public List<PNode> SkidMiningPath(Vector2i start, Vector2i end)
  {
    return this.skiddedMinePath.FindPath(PNode.Create(start.x, start.y), PNode.Create(end.x, end.y));
  }

  internal IEnumerable<PNode> SkidMiningPath(PNode pNode, Vector2i mapPoint)
  {
    throw new NotImplementedException();
  }
}
