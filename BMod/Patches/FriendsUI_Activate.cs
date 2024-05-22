

using HarmonyLib;
using Il2Cpp;
using Il2CppKernys.Bson;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (FriendsUI), "Activate")]
  internal static class FriendsUI_Activate
  {
    private static void Postfix()
    {
      BSONObject bsonObject = new BSONObject();
      ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("GFLi");
      OutgoingMessages.AddOneMessageToList(bsonObject);
    }
  }
}
