
using DiscordRPC;
using Il2Cpp;
using System.IO;
using System.Net;
using UnityEngine;

 
namespace BMod.Discord
{
  internal static class DiscordManager
  {
    public static bool initialized = false;
    public static bool showName = false;
    public static bool showWorld = true;
    internal static string firstPlayerName = "Unknown";
    private static float updateTimer = 0.0f;
    public static DiscordRpcClient client;
    public static Timestamps timestamps = Timestamps.Now;

    public static void Init()
    {
      DiscordManager.initialized = true;
      DiscordManager.client = new DiscordRpcClient("1216137919857819710");
      DiscordManager.client.Initialize();
      DiscordManager.timestamps = Timestamps.Now;
      DiscordManager.Update();
    }

    public static void Update()
    {
      try
      {
        DiscordRpcClient client = DiscordManager.client;
        RichPresence richPresence = new RichPresence();
        richPresence.Details = DiscordManager.GetUsername();
        richPresence.State = DiscordManager.GetWorldName();
        richPresence.Timestamps = DiscordManager.timestamps;
        richPresence.Assets = new Assets()
        {
          LargeImageKey = "pw0",
          LargeImageText = "Pixel Worlds",
          SmallImageKey = "logo0",
          SmallImageText = "BMod Premium"
        };
        richPresence.Buttons = new Button[1]
        {
          new Button()
          {
            Label = "Get BMod",
            Url = "https://discord.gg/7DaEF34tCk"
          }
        };
        RichPresence presence = richPresence;
        client.SetPresence(presence);
      }
      catch
      {
      }
    }

    public static string GetWorldName()
    {
      if (Object.op_Inequality((Object) ControllerHelper.networkClient, (Object) null))
      {
        PlayerConnectionStatus connectionStatus = ControllerHelper.networkClient.playerConnectionStatus;
        if (connectionStatus == 0)
          return "Not Connected";
        if (connectionStatus == 8)
          return "Joining World";
        if (connectionStatus == 7)
          return "In Menus";
      }
      if (Globals.world == null || NetworkPlayers.otherPlayers == null || !Object.op_Inequality((Object) Globals.player, (Object) null))
        return "In Menus";
      return DiscordManager.showWorld ? string.Format("{0} ({1}/50)", (object) Globals.world.worldName, (object) (NetworkPlayers.otherPlayers.Count + 1)) : string.Format("In World ({0}/50)", (object) (NetworkPlayers.otherPlayers.Count + 1));
    }

    public static string GetUsername()
    {
      return !string.IsNullOrWhiteSpace(StaticPlayer.theRealPlayername) ? (DiscordManager.showName ? StaticPlayer.theRealPlayername : "Hidden Username") : "Unknown Username";
    }

    public static void FixedUpdate()
    {
      DiscordManager.updateTimer += Time.fixedDeltaTime;
      if ((double) DiscordManager.updateTimer < 10.0)
        return;
      DiscordManager.updateTimer -= 10f;
      DiscordManager.Update();
    }

    public static string SendToWebhook(string url, string data)
    {
      WebRequest webRequest = WebRequest.Create(url);
      webRequest.ContentType = "application/json";
      webRequest.Method = "POST";
      using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
        streamWriter.Write(data);
      return ((HttpWebResponse) webRequest.GetResponse()).StatusDescription;
    }
  }
}
