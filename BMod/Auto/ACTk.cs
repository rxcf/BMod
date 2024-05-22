
using Il2Cpp;
using MelonLoader;
using System;

namespace BMod.Auto
{
  internal class ACTk
  {
    internal static int punishType = 0;
    internal static int m_invis = 0;
    internal static string lc_invis = "";
    internal static bool active = false;
    internal static bool chatSubmit = false;
    internal static bool bypassStaff = true;

    public static void Start() => ACTk.active = true;

    public static void Stop(string reason, bool byUser = false, bool remote = false)
    {
      ACTk.active = false;
      if (!byUser)
      {
        Console.WriteLine("AntiCheat :: " + reason);
        InfoPopupUI.SetupInfoPopup(TextManager.Capitalize("ANTI-CHEAT"), "was deactivated due to\n" + reason);
        InfoPopupUI.ForceShowMenu();
      }
      else
      {
        Console.WriteLine("AntiCheat :: was deactivated user.");
        InfoPopupUI.ForceShowMenu();
      }
    }

    public static void Punish(string id, string reason = "")
    {
      string message = string.Format("Punishing({0}) {1}({2}) due to {3}", (object) ACTk.punishType, (object) NetworkPlayers.GetNameWithId(id, false), (object) id, (object) reason);
      MelonLogger.Warning(message);
      Utils.Msg(message);
      string str = "detected violating";
      switch (ACTk.punishType)
      {
        case 0:
          str = "detected violating";
          break;
        case 1:
          OutgoingMessages.KickPlayer(id);
          str = "kicked";
          break;
        case 2:
          OutgoingMessages.BanAndKickPlayer(id);
          str = "banned";
          break;
      }
      if (!ACTk.chatSubmit || (double) PlayerCheats.submitTimer < 8.0)
        return;
      PlayerCheats.submitTimer = 0.0f;
      Globals.chatUI.Submit("[ACTk] " + NetworkPlayers.GetNameWithId(id, false) + " was " + str + " due to " + reason);
    }

    public static bool CanBypass(string playerId)
    {
      return ACTk.bypassStaff && Utils.FindNetworkPlayers(Utils.SearchType.WorldStaff).Contains(playerId);
    }
  }
}
