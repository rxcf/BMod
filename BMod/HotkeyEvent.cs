
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppKernys.Bson;
using System;
using System.IO;
using UnityEngine;

 
namespace BMod
{
  public class HotkeyEvent : MonoBehaviour
  {
    public static bool AltPressed => Input.GetKey((KeyCode) 308) || Input.GetKey((KeyCode) 307);

    public static bool CtrlPressed => Input.GetKey((KeyCode) 306) || Input.GetKey((KeyCode) 305);

    public static bool ShiftPressed => Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303);

    public static void SwitchHack(ref bool toggle)
    {
      if (toggle)
      {
        double num1 = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 192, 0.0f, -1);
      }
      else
      {
        double num2 = (double) ControllerHelper.audioManager.PlaySFX((AudioManager.SoundType) 191, 0.0f, -1);
      }
      toggle = !toggle;
    }

    public static void Update()
    {
      if (Input.GetKeyDown((KeyCode) 283))
        Globals.dataTooltip = !Globals.dataTooltip;
      if (Input.GetKeyDown((KeyCode) 284))
      {
        Globals.freeCam = !Globals.freeCam;
        if (!Globals.freeCam)
        {
          Vector3 position = ((Component) Globals.player).transform.position;
          ((Component) Camera.main).transform.position = new Vector3(position.x, position.y, -0.5f);
          ControllerHelper.kukouriCamera.ForceUpdatePosition(false);
        }
      }
      if (Input.GetKeyDown((KeyCode) 286))
      {
        Globals.lagHack = !Globals.lagHack;
        Utils.Msg("Lag Hack is now " + Globals.lagHack.ToString());
      }
      if (Input.GetKeyDown((KeyCode) 287))
      {
        Globals.invisHack = !Globals.invisHack;
        Utils.Msg("Invis Hack is now " + Globals.invisHack.ToString());
      }
      if (Input.GetKeyDown((KeyCode) 289) && Globals.world != null)
      {
        if (Globals.world.worldName != "TUTORIAL2")
        {
          SceneLoader.GoFromWorldToMainMenu();
        }
        else
        {
          OutgoingMessages.SendTutorialStateMessage((PlayerData.TutorialState) 5);
          OutgoingMessages.SendTutorialStateMessage((PlayerData.TutorialState) 6);
          OutgoingMessages.BuyItemPack("BasicClothes");
          OutgoingMessages.SendTutorialStateMessage((PlayerData.TutorialState) 7);
          OutgoingMessages.LeaveWorld();
          OutgoingMessages.SendTutorialStateMessage((PlayerData.TutorialState) 3);
        }
      }
      if ((Input.GetKey((KeyCode) 306) || Input.GetKey((KeyCode) 305)) && (Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303)) && Input.GetKeyDown((KeyCode) 290))
      {
        UserIdent.LogOut();
        SceneLoader.GoToWelcomeScene();
      }
      else if ((Input.GetKey((KeyCode) 304) || Input.GetKey((KeyCode) 303)) && Input.GetKeyDown((KeyCode) 290))
        ControllerHelper.networkClient.DoHardReconnect((HardReconnectReason) 22, -1);
      else if (Input.GetKeyDown((KeyCode) 290))
        SceneLoader.ReloadWorld();
      else if (Input.GetKeyDown((KeyCode) 259))
      {
        ChatUI.SendLogMessage("PrettyCheeat has entered the world.");
        ChatUI.SendLogMessage("RORRE has entered the world.");
      }
      if (Input.GetKey((KeyCode) 304) && Input.GetKeyDown((KeyCode) 292))
        Console.WriteLine(Utils.DumpBSON(SimpleBSON.Load(Il2CppStructArray<byte>.op_Implicit(File.ReadAllBytes("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Pixel Worlds\\UserData\\MyPacket.json")))));
      else if (Input.GetKeyDown((KeyCode) 292))
        LoadingScreenController.HideLoading();
      if (Input.GetKeyDown((KeyCode) 293) && Globals.useOldTP)
      {
        try
        {
          Vector2i mapPoint = Utils.ConvertWorldPointToMapPoint(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
          Globals.player.WarpPlayer(mapPoint.x, mapPoint.y);
        }
        catch
        {
        }
      }
      if (HotkeyEvent.CtrlPressed && Input.GetKey((KeyCode) 102) && Input.GetKey((KeyCode) 103) && Object.op_Equality((Object) Object.FindObjectOfType<FishingRecyclerUI>(), (Object) null))
        ControllerHelper.rootUI.OnOrOffMenu(Il2CppType.Of<FishingRecyclerUI>());
      else if (HotkeyEvent.CtrlPressed && Input.GetKey((KeyCode) 109) && Input.GetKey((KeyCode) 103) && Object.op_Equality((Object) Object.FindObjectOfType<MiningGemstoneRecyclerUI>(), (Object) null))
        ControllerHelper.rootUI.OnOrOffMenu(Il2CppType.Of<MiningGemstoneRecyclerUI>());
      if (Input.GetKeyDown((KeyCode) 274) || Input.GetKeyDown((KeyCode) 115))
      {
        WorldHologramUI objectOfType1 = Object.FindObjectOfType<WorldHologramUI>();
        if (Object.op_Inequality((Object) objectOfType1, (Object) null))
          objectOfType1.NextPageButtonPressed();
        AuctionHouseUI objectOfType2 = Object.FindObjectOfType<AuctionHouseUI>();
        if (!Object.op_Inequality((Object) objectOfType2, (Object) null) || objectOfType2.searchField.isFocused)
          return;
        objectOfType2.NextPageButtonPressed();
      }
      else if (Input.GetKeyDown((KeyCode) 273) || Input.GetKeyDown((KeyCode) 119))
      {
        WorldHologramUI objectOfType3 = Object.FindObjectOfType<WorldHologramUI>();
        if (Object.op_Inequality((Object) objectOfType3, (Object) null))
          objectOfType3.PreviousPageButtonPressed();
        AuctionHouseUI objectOfType4 = Object.FindObjectOfType<AuctionHouseUI>();
        if (!Object.op_Inequality((Object) objectOfType4, (Object) null) || objectOfType4.searchField.isFocused)
          return;
        objectOfType4.PreviousPageButtonPressed();
      }
      else if (Input.GetKeyDown((KeyCode) 32) || Input.GetKeyDown((KeyCode) 13))
      {
        OpenItemPackUI objectOfType = Object.FindObjectOfType<OpenItemPackUI>();
        if (!Object.op_Inequality((Object) objectOfType, (Object) null))
          return;
        objectOfType.ReBuy();
      }
      else if ((Input.GetKey((KeyCode) 308) || Input.GetKey((KeyCode) 307)) && Input.GetKeyDown((KeyCode) 116))
      {
        if (!Object.op_Equality((Object) Object.FindObjectOfType<AdminToolsTeleportOverlayUI>(), (Object) null))
          return;
        ControllerHelper.rootUI.OnOrOffMenu(Il2CppType.Of<AdminToolsTeleportOverlayUI>());
        ((BaseMenuUI) ControllerHelper.gameplayUI).DoHideAnimation(false);
      }
      else if (HotkeyEvent.AltPressed && Input.GetKeyDown((KeyCode) 51))
        HotkeyEvent.SwitchHack(ref Globals.antiBounce);
      else if (HotkeyEvent.AltPressed && Input.GetKeyDown((KeyCode) 52))
        HotkeyEvent.SwitchHack(ref Globals.godMode);
      else if (HotkeyEvent.AltPressed && Input.GetKeyDown((KeyCode) 53))
        HotkeyEvent.SwitchHack(ref Globals.ignoreVortex);
      else if (HotkeyEvent.AltPressed && Input.GetKeyDown((KeyCode) 54))
        HotkeyEvent.SwitchHack(ref Globals.blockOnPlayer);
      else if (HotkeyEvent.AltPressed && Input.GetKeyDown((KeyCode) 56))
      {
        HotkeyEvent.SwitchHack(ref Globals.antiCollect);
      }
      else
      {
        if (!HotkeyEvent.AltPressed || !Input.GetKeyDown((KeyCode) 96))
          return;
        HotkeyEvent.SwitchHack(ref Globals.fly);
      }
    }
  }
}
