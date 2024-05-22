
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BMod.Auto
{
  public class AutoASP
  {
    public static bool active = false;
    public static List<int> shit = new List<int>()
    {
      19,
      21,
      22,
      26,
      32,
      41,
      49,
      58,
      92,
      93,
      101,
      102,
      115,
      117,
      118,
      120,
      121,
      129,
      133,
      135,
      136,
      137,
      138,
      139,
      143,
      144,
      145,
      152,
      158,
      159,
      160,
      161,
      162,
      163,
      164,
      165,
      166,
      167,
      169,
      170,
      171
    };

    public static async void NewPack()
    {
      await Task.Run((Action) (() =>
      {
        Thread.Sleep(300);
        OutgoingMessages.BuyItemPack("AdvancedSeeds");
      }));
    }

    public static void Start()
    {
      if (Object.op_Equality((Object) Globals.player, (Object) null) || Globals.world == null)
        return;
      if (Globals.playerData.inventorySlots < (short) 150)
      {
        AutoASP.StartFailed("Not enought slots");
      }
      else
      {
        foreach (PlayerData.InventoryKey inventoryKey in (Il2CppArrayBase<PlayerData.InventoryKey>) Globals.playerData.GetInventoryAsOrderedByInventoryItemType())
        {
          if (inventoryKey.itemType != 2)
          {
            AutoASP.StartFailed("Inappropriate items in the inventory");
            return;
          }
        }
        AutoASP.active = true;
        OutgoingMessages.BuyItemPack("AdvancedSeeds");
      }
    }

    public static void Stop(string reason, bool byUser = false)
    {
      AutoASP.active = false;
      if (byUser)
        return;
      MelonLogger.Error("AutoASP stopped :: " + reason);
      InfoPopupUI.SetupInfoPopup(nameof (AutoASP), "Stopped\n" + reason);
      InfoPopupUI.ForceShowMenu();
    }

    public static void StartFailed(string why)
    {
      InfoPopupUI.SetupInfoPopup(nameof (AutoASP), why);
      InfoPopupUI.ForceShowMenu();
    }

    public static void HandleItemPackBought(BSONObject p)
    {
      if (((BSONValue) p).ContainsKey("S"))
      {
        string stringValue = ((BSONValue) p)["IPId"].stringValue;
        List<PlayerData.InventoryKey> list = new List<PlayerData.InventoryKey>();
        ItemPacks.ItemPack itemPack = ItemPacks.GetItemPack(stringValue);
        if (!((BSONValue) p).ContainsKey(NetStrings.ItemPackRolls))
          return;
        Il2CppStructArray<PlayerData.InventoryKey> randomDropsBucket1 = itemPack.randomDropsBucket1;
        foreach (int num in ((BSONValue) p)["IPRs"].int32ListValue)
        {
          Globals.playerData.AddItemToInventory(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num], (InventoryItemBase) null);
          while (Globals.world.IsCollectableAmountFullInMapPoint(Globals.player.GetNextPlayerPositionBasedOnLookDirection(0)))
            Globals.player.WarpPlayer(Globals.player.currentPlayerMapPoint.x - 2, Globals.player.currentPlayerMapPoint.y);
          if (Globals.playerData.GetCount(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num]) > (short) 990)
            Globals.player.DropItems(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num], Globals.playerData.GetCount(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num]));
          if (AutoASP.shit.Contains(num))
          {
            Globals.player.TrashItems(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num], Globals.playerData.GetCount(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num]));
            Globals.playerData.RemoveItemFromInventory(((Il2CppArrayBase<PlayerData.InventoryKey>) randomDropsBucket1)[num]);
          }
        }
        OutgoingMessages.BuyItemPack("AdvancedSeeds");
      }
      else if (((BSONValue) p).ContainsKey("ER"))
        AutoASP.Stop("Error on HandleBought: " + ((BSONValue) p)["ER"].stringValue);
      else
        AutoASP.Stop("Error on HandleBought, unknown reason");
    }
  }
}
