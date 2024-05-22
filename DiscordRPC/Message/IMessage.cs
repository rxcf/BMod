

using System;

 
namespace DiscordRPC.Message
{
  public abstract class IMessage
  {
    private DateTime _timecreated;

    public abstract MessageType Type { get; }

    public DateTime TimeCreated => this._timecreated;

    public IMessage() => this._timecreated = DateTime.Now;
  }
}
