
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

 
namespace BMod.pathfinding
{
  public class ShiukiAI : TileProvider
  {
    public bool[,] map;

    public ShiukiAI(int width, int height)
      : base(width, height)
    {
      this.map = new bool[width, height];
    }

    public override void ResetSize(int width, int height)
    {
      base.ResetSize(width, height);
      this.map = new bool[width, height];
    }

    public override bool IsTileWalkable(int x, int y)
    {
      World.BlockType blockType = Globals.world.GetBlockType(new Vector2i(x, y));
      if (!Globals.world.IsMapPointInWorld(new Vector2i(x, y)))
        return false;
      if (this.IsBlockCloud(blockType) || (ConfigData.IsBlockPlatform(blockType) || blockType == 110) && Globals.lastpos.y <= y || ConfigData.IsAnyDoor(blockType) && Globals.worldController.DoesPlayerHaveRightToGoDoorForCollider(new Vector2i(x, y)))
        return true;
      if (ConfigData.IsBlockBattleBarrier(blockType))
      {
        BattleBarrierBasicData barrierBasicData = new BattleBarrierBasicData(1);
        ((WorldItemBase) barrierBasicData).SetViaBSON(Globals.world.GetWorldItemData(new Vector2i(x, y)).GetAsBSON());
        if (barrierBasicData.isOpen)
          return true;
      }
      if (ConfigData.IsBlockDisappearingBlock(blockType))
      {
        DisappearingBlockData disappearingBlockData = new DisappearingBlockData(1);
        ((WorldItemBase) disappearingBlockData).SetViaBSON(Globals.world.GetWorldItemData(new Vector2i(x, y)).GetAsBSON());
        if (disappearingBlockData.isOpen)
          return true;
      }
      return blockType == 1420 || blockType == 4286 || blockType == 4366 || blockType == 4372 || blockType == 4103 || blockType == 3966 || !((Il2CppArrayBase<bool>) ConfigData.doesBlockHaveCollider)[(int) blockType];
    }

    public override bool IsBlockInstaKillOn(int x, int y)
    {
      return this.IsBlockInstakill(Globals.world.GetBlockType(new Vector2i(x, y)));
    }
  }
}
