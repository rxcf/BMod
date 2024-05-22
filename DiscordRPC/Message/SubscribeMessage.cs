
using DiscordRPC.RPC.Payload;

 
namespace DiscordRPC.Message
{
  public class SubscribeMessage : IMessage
  {
    public override MessageType Type => MessageType.Subscribe;

    public EventType Event { get; internal set; }

    internal SubscribeMessage(ServerEvent evt)
    {
      switch (evt)
      {
        case ServerEvent.ActivitySpectate:
          this.Event = EventType.Spectate;
          break;
        case ServerEvent.ActivityJoinRequest:
          this.Event = EventType.JoinRequest;
          break;
        default:
          this.Event = EventType.Join;
          break;
      }
    }
  }
}
