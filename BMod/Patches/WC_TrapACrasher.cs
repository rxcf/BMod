
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (WorldController), "GetTrapFrequencyFixedUpdateCount", new Type[] {typeof (Vector2i)})]
  internal static class WC_TrapACrasher
  {
    private static bool Prefix(ref int __result, ref Vector2i mapPoint)
    {
      BSONValue bsonValue = BSONValue.op_Implicit(((BSONValue) ((Il2CppArrayBase<WorldItemBase>) ((Il2CppArrayBase<Il2CppReferenceArray<WorldItemBase>>) Globals.world.worldItemsData)[mapPoint.x])[mapPoint.y].GetAsBSON())["trapFrequencyType"].int32Value);
      if (BSONValue.op_Implicit(bsonValue) >= 0 && BSONValue.op_Implicit(bsonValue) <= 4)
        return true;
      __result = 1;
      return false;
    }
  }
}
