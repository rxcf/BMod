
using Il2CppBasicTypes;
using Priority_Queue;

public class PNode : FastPriorityQueueNode
{
  public int X { get; private set; }

  public int Y { get; private set; }

  public static PNode Create(int x, int y) => new PNode(x, y);

  private PNode(int x, int y)
  {
    this.X = x;
    this.Y = y;
  }

  public static explicit operator Vector2i(PNode pn) => new Vector2i(pn.X, pn.Y);

  public override bool Equals(object obj)
  {
    PNode pnode = (PNode) obj;
    return this.X == pnode.X && this.Y == pnode.Y;
  }

  public override int GetHashCode() => this.X + this.Y * 7;

  public override string ToString() => "(" + this.X.ToString() + ", " + this.Y.ToString() + ")";
}
