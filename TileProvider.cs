
using BMod;
using Il2Cpp;

public abstract class TileProvider
{
  public int Width { get; private set; }

  public int Height { get; private set; }

  public TileProvider(int width, int height)
  {
    this.Width = width;
    this.Height = height;
  }

  public virtual void ResetSize(int width, int height)
  {
    this.Width = width;
    this.Height = height;
  }

  public virtual bool TileInBounds(int x, int y)
  {
    return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
  }

  public abstract bool IsTileWalkable(int x, int y);

  public abstract bool IsBlockInstaKillOn(int x, int y);

  public bool IsBlockInstakill(World.BlockType blockType) => ConfigData.IsBlockInstakill(blockType);

  public bool IsBlockCloud(World.BlockType blockType) => blockType == 656 || blockType == 956;

  public bool IsBlockCloudOn(int x, int y) => this.IsBlockCloud(Globals.world.GetBlockType(x, y));
}
