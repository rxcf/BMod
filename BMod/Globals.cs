
using BMod.Auto;
using BMod.Patches;
using Il2Cpp;
using Il2CppBasicTypes;
using Il2CppSystem.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

 
namespace BMod
{
  internal static class Globals
  {
    public const string MOD_VERSION = "2.0.2";
    public static bool antiBounce;
    public static bool godMode;
    public static bool fly;
    public static bool ignoreVortex;
    public static bool noKnockBack;
    public static bool blockOnPlayer;
    public static bool antiCollect;
    public static bool instaRespawn;
    public static bool mouseFly;
    public static bool speedHack;
    public static bool freeCam;
    public static bool playerESP;
    public static bool enemyESP;
    public static bool gemstoneESP;
    public static bool collectableESP;
    public static bool netherBossESP;
    public static bool giftboxESP;
    internal static bool extraESP;
    public static bool aiAimBot;
    public static bool giveawayMode;
    public static bool autoClaimGift;
    public static bool playerAimBot;
    public static bool notifyBoxChanged;
    public static bool dataTooltip;
    public static bool keysToFly;
    public static bool invisHack;
    public static bool lagHack;
    public static bool leakPrizes;
    public static bool noBlockKill;
    public static bool isTeleporting;
    public static bool shouldShowWelcome;
    public static bool useOldTP;
    public static bool customInvis;
    public static bool handleWP;
    public static bool cmOutgoing;
    public static bool cmIncoming;
    public static bool cmPackets;
    public static bool repeatTroll;
    public static bool autoPlaceWL;
    public static bool noProfanityFilter;
    public static bool noAFK;
    public static bool noDash;
    public static bool interactionTroll;
    internal static bool colorfulNames;
    internal static bool ignoreDisconnect;
    internal static bool autoBan;
    public static bool handleKick;
    internal static bool safeAnnoySound;
    internal static bool annoySound;
    public static bool recipesHack;
    internal static bool bossFinder;
    internal static bool ACTk_invis;
    internal static bool bedHack;
    internal static bool ytmode;
    internal static bool dataSpam;
    public static string repeatTrollId;
    public static string ytmodeName;
    public static string textToSpam;
    public static int currentTp;
    public static int invisX;
    public static int invisY;
    public static float speedHackSpeed;
    public static float freeCamSpeed;
    public static float keysFlySpeed;
    public static float teleportTimer;
    public static float teleportSpeed;
    public static Vector2i targetTp;
    public static Vector2i lastpos;
    public static List<int> iceBlocks;
    public static List<int> glueBlocks;
    public static List<Vector2i> discoveredPrizes;
    public static List<Vector2i> extraESPbutterflies;
    public static List<PNode> teleportPath;
    public static List<string> iCapture;
    public static List<string> oCapture;
    public static List<string> iIgnore;
    public static List<string> oIgnore;
    public static List<string> recentMsgs;
    public static List<World.BlockType> annoyBlocks;
    public static List<World.BlockType> annoyBlocks2;
    public static World.BlockType[] pickaxes;
    public static string[] locksDataClasses;
    public static string[] portalsDataClasses;
    public static string[] signsDataClasses;
    public static string[] signsAdvancedDataClasses;
    public static string[] doorsDataClasses;
    public static string[] doorsAdvancedDataClasses;
    public static string[] chestsDataClasses;
    public static string[] triggersDataClasses;
    public static string[] giftboxesDataClasses;
    public static string[] guestbooksDataClasses;
    public static string[] displayDataClasses;
    public static string[] trapsDataClasses;
    public static List<string> ownerIds;
    public static Spirit current_spirit;
    public static Teleport teleport;
    public static SummonTimer timers;
    internal static MineBot mineBot;
    public static Vector3 originalCameraPosition;

    public static Player player
    {
      get
      {
        try
        {
          return ControllerHelper.worldController.player;
        }
        catch
        {
          return (Player) null;
        }
      }
    }

    public static PlayerData playerData
    {
      get
      {
        try
        {
          return Globals.player.myPlayerData;
        }
        catch
        {
          return (PlayerData) null;
        }
      }
    }

    public static World world
    {
      get
      {
        try
        {
          return ControllerHelper.worldController.world;
        }
        catch
        {
          return (World) null;
        }
      }
    }

    public static WorldController worldController
    {
      get
      {
        try
        {
          return ControllerHelper.worldController;
        }
        catch
        {
          return (WorldController) null;
        }
      }
    }

    public static ChatUI chatUI
    {
      get
      {
        try
        {
          return ControllerHelper.chatUI;
        }
        catch
        {
          return (ChatUI) null;
        }
      }
    }

    public static NotificationController notificationController
    {
      get
      {
        try
        {
          return ControllerHelper.notificationController;
        }
        catch
        {
          return (NotificationController) null;
        }
      }
    }

    public static GameplayUI gameplayUI
    {
      get
      {
        try
        {
          return ControllerHelper.gameplayUI;
        }
        catch
        {
          return (GameplayUI) null;
        }
      }
    }

    public static RootUI rootUI
    {
      get
      {
        try
        {
          return ControllerHelper.rootUI;
        }
        catch
        {
          return (RootUI) null;
        }
      }
    }

    public static Vector2i CurMP
    {
      get
      {
        try
        {
          return ControllerHelper.worldController.player.currentPlayerMapPoint;
        }
        catch
        {
          return new Vector2i(0, 0);
        }
      }
    }

    static Globals()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
