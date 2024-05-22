
using BMod.Auto;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (WorldController), "RemoveCollectable")]
  internal static class WC_RemoveCollectable
  {
    private static void Prefix(int id, bool isCalledByLocalPlayer)
    {
      if (FarmBot.active & isCalledByLocalPlayer)
      {
        for (int index = 0; index < Globals.worldController.currentCollectables.Count; ++index)
        {
          if (Object.op_Inequality((Object) Globals.worldController.currentCollectables[index], (Object) null) && Globals.worldController.currentCollectables[index].GetCollectableData().id == id)
          {
            CollectableData collectableData = Globals.worldController.currentCollectables[index].GetCollectableData();
            if (collectableData.blockType != FarmBot.blockType || collectableData.inventoryItemType != 2)
              break;
            ++FarmBot.seedsDropped;
            break;
          }
        }
      }
      else
      {
        if (!(MineBot.active & isCalledByLocalPlayer) || !(Globals.world.worldName == "MINEWORLD"))
          return;
        Globals.mineBot.CheckOverload();
        for (int index = 0; index < Globals.worldController.currentCollectables.Count; ++index)
        {
          if (Object.op_Inequality((Object) Globals.worldController.currentCollectables[index], (Object) null) && Globals.worldController.currentCollectables[index].GetCollectableData().id == id)
          {
            CollectableData collectableData = Globals.worldController.currentCollectables[index].GetCollectableData();
            if (ConfigData.IsBlockMiningGemstone(collectableData.blockType))
            {
              Globals.mineBot.gems += ConfigData.GetGemstoneRecycleValueForMiningGemstoneRecycler(collectableData.blockType) * (int) collectableData.amount;
              break;
            }
            break;
          }
        }
      }
    }
  }
}
