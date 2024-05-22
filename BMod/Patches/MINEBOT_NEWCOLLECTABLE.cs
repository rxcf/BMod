

using BMod.Auto;
using HarmonyLib;
using Il2Cpp;
using Il2CppBasicTypes;
using System;
using System.Collections.Generic;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (WorldController), "DoCollectableBlock", new Type[] {typeof (CollectableData)})]
  internal static class MINEBOT_NEWCOLLECTABLE
  {
    private static void Postfix(CollectableData collectableData)
    {
      if (!MineBot.active || !(Globals.world.worldName == "MINEWORLD") || LoadingScreenController.instance.IsLoading() || !MineBot.IsMiningRelatedItem(collectableData.blockType))
        return;
      OutgoingMessages.SendCollectCollectableMessage(collectableData.id);
      if (Vector2i.op_Inequality(Globals.player.currentPlayerMapPoint, collectableData.mapPoint))
      {
        List<PNode> collection = Globals.teleport.pather.SkidMiningPath(Globals.mineBot.lastMP, collectableData.mapPoint);
        collection.AddRange((IEnumerable<PNode>) collection);
        collection.Reverse(collection.Count / 2, collection.Count / 2);
        Globals.mineBot.path.InsertRange(0, (IEnumerable<PNode>) collection);
      }
    }
  }
}
