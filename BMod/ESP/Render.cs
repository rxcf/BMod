
using UnityEngine;

 
namespace BMod.ESP
{
  public class Render : MonoBehaviour
  {
    public static Texture2D lineTex;

    public static Color Color
    {
      get => GUI.color;
      set => GUI.color = value;
    }

    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
      Matrix4x4 matrix = GUI.matrix;
      if (!Object.op_Implicit((Object) Render.lineTex))
        Render.lineTex = new Texture2D(1, 1);
      Color color1 = GUI.color;
      GUI.color = color;
      float num = Vector3.Angle(Vector2.op_Implicit(Vector2.op_Subtraction(pointB, pointA)), Vector2.op_Implicit(Vector2.right));
      if ((double) pointA.y > (double) pointB.y)
        num = -num;
      Vector2 vector2 = Vector2.op_Subtraction(pointB, pointA);
      GUIUtility.ScaleAroundPivot(new Vector2(((Vector2) ref vector2).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
      GUIUtility.RotateAroundPivot(num, pointA);
      GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), (Texture) Render.lineTex);
      GUI.matrix = matrix;
      GUI.color = color1;
    }

    public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
    {
      Render.DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
      Render.DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
      Render.DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
      Render.DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
    }

    public static void DrawBoxESP(Vector3 footpos, Vector3 headpos, Color color)
    {
      float h = headpos.y - footpos.y;
      float num = 2f;
      float w = h / num;
      Render.DrawBox(footpos.x - w / 2f, (float) Screen.height - footpos.y - h, w, h, color, 2f);
      Render.DrawLine(new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2)), new Vector2(footpos.x, (float) Screen.height - footpos.y), color, 2f);
    }
  }
}
