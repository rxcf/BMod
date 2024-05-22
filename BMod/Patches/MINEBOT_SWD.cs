
using BMod.Auto;
using BMod.Discord;
using HarmonyLib;
using Il2Cpp;
using Il2CppKernys.Bson;
using MelonLoader;
using Newtonsoft.Json;
using System;

 
namespace BMod.Patches
{
  [HarmonyPatch(typeof (NetworkClient), "HandleSpecialWorldDataPacket", new Type[] {typeof (BSONObject)})]
  internal static class MINEBOT_SWD
  {
    private static void Postfix(BSONObject currentMessage)
    {
      if (!MineBot.active || ((BSONValue) currentMessage)["SWDt"].int32Value != 4)
        return;
      World.SpecialWorldDataEvent int32Value = (World.SpecialWorldDataEvent) ((BSONValue) currentMessage)["SWDe"].int32Value;
      if (int32Value != 1)
      {
        if (int32Value != 4)
        {
          if (int32Value == 8)
            ++Globals.mineBot.enemyKilled;
        }
        else
        {
          MelonLogger.Msg(string.Format("Mine {0} has been cleared in {1}s", (object) (Globals.mineBot.currentMineIndex + 1), (object) Math.Round((double) Globals.mineBot.timePassed, 2)));
          Globals.mineBot.timePassed = 0.0f;
          ++Globals.mineBot.minesCleared;
          Globals.mineBot.CheckOverload();
          if (Globals.mineBot.CheckOverload(false))
          {
            Globals.mineBot.state = MineBot.BotState.JoinBufferWorld;
            ChatCommand.Execute(new string[3]
            {
              "/w",
              Globals.mineBot.bufferWorld,
              Globals.mineBot.bufferEntry
            });
          }
          else
          {
            BSONObject bsonObject = new BSONObject();
            ((BSONValue) bsonObject)["ID"] = BSONValue.op_Implicit("LW");
            OutgoingMessages.AddOneMessageToList(bsonObject);
            Globals.mineBot.JoinMine(Globals.mineBot.GetMineLevelPriority());
          }
          Globals.invisHack = false;
        }
      }
      else
      {
        Globals.mineBot.state = MineBot.BotState.None;
        Globals.mineBot.StartMining();
        if (Globals.mineBot.reconnecting)
        {
          Globals.mineBot.reconnecting = false;
          MelonLogger.Msg("CONNECTION LOOKS LIKE TO BE ESTABLISHED! :)");
          if (MineBot.useWebhook)
          {
            try
            {
              Utils.D(DiscordManager.SendToWebhook(Globals.mineBot.webhook, JsonConvert.SerializeObject((object) new
              {
                username = "BModSlave",
                avatar_url = "https://cdn.discordapp.com/app-assets/1216137919857819710/1216320682485612594.png",
                embeds = new \u003C\u003Ef__AnonymousType1<string, int, \u003C\u003Ef__AnonymousType2<string>>[1]
                {
                  new
                  {
                    title = "Connection has been established.",
                    color = 2071552,
                    author = new
                    {
                      name = "Everything is ok"
                    }
                  }
                }
              })));
            }
            catch (Exception ex)
            {
              MelonLogger.BigError("Webhook", ex.Message);
            }
          }
        }
      }
    }
  }
}
