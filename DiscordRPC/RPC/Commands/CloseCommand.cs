﻿
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

 
namespace DiscordRPC.RPC.Commands
{
  internal class CloseCommand : ICommand
  {
    [JsonProperty("close_reason")]
    public string value = "Unity 5.5 doesn't handle thread aborts. Can you please close me discord?";

    [JsonProperty("pid")]
    public int PID { get; set; }

    public IPayload PreparePayload(long nonce)
    {
      ArgumentPayload argumentPayload = new ArgumentPayload();
      argumentPayload.Command = Command.Dispatch;
      argumentPayload.Nonce = (string) null;
      argumentPayload.Arguments = (JObject) null;
      return (IPayload) argumentPayload;
    }
  }
}
