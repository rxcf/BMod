
using Newtonsoft.Json;

 
namespace DiscordRPC
{
  public sealed class RichPresence : BaseRichPresence
  {
    [JsonProperty("buttons")]
    public Button[] Buttons { get; set; }

    public bool HasButtons() => this.Buttons != null && this.Buttons.Length != 0;

    public RichPresence WithState(string state)
    {
      this.State = state;
      return this;
    }

    public RichPresence WithDetails(string details)
    {
      this.Details = details;
      return this;
    }

    public RichPresence WithTimestamps(Timestamps timestamps)
    {
      this.Timestamps = timestamps;
      return this;
    }

    public RichPresence WithAssets(Assets assets)
    {
      this.Assets = assets;
      return this;
    }

    public RichPresence WithParty(Party party)
    {
      this.Party = party;
      return this;
    }

    public RichPresence WithSecrets(Secrets secrets)
    {
      this.Secrets = secrets;
      return this;
    }

    public RichPresence Clone()
    {
      RichPresence richPresence1 = new RichPresence();
      richPresence1.State = this._state != null ? this._state.Clone() as string : (string) null;
      richPresence1.Details = this._details != null ? this._details.Clone() as string : (string) null;
      richPresence1.Buttons = !this.HasButtons() ? (Button[]) null : this.Buttons.Clone() as Button[];
      RichPresence richPresence2 = richPresence1;
      Secrets secrets;
      if (this.HasSecrets())
        secrets = new Secrets()
        {
          JoinSecret = this.Secrets.JoinSecret != null ? this.Secrets.JoinSecret.Clone() as string : (string) null,
          SpectateSecret = this.Secrets.SpectateSecret != null ? this.Secrets.SpectateSecret.Clone() as string : (string) null
        };
      else
        secrets = (Secrets) null;
      richPresence2.Secrets = secrets;
      RichPresence richPresence3 = richPresence1;
      Timestamps timestamps;
      if (this.HasTimestamps())
        timestamps = new Timestamps()
        {
          Start = this.Timestamps.Start,
          End = this.Timestamps.End
        };
      else
        timestamps = (Timestamps) null;
      richPresence3.Timestamps = timestamps;
      RichPresence richPresence4 = richPresence1;
      Assets assets;
      if (this.HasAssets())
        assets = new Assets()
        {
          LargeImageKey = this.Assets.LargeImageKey != null ? this.Assets.LargeImageKey.Clone() as string : (string) null,
          LargeImageText = this.Assets.LargeImageText != null ? this.Assets.LargeImageText.Clone() as string : (string) null,
          SmallImageKey = this.Assets.SmallImageKey != null ? this.Assets.SmallImageKey.Clone() as string : (string) null,
          SmallImageText = this.Assets.SmallImageText != null ? this.Assets.SmallImageText.Clone() as string : (string) null
        };
      else
        assets = (Assets) null;
      richPresence4.Assets = assets;
      RichPresence richPresence5 = richPresence1;
      Party party;
      if (this.HasParty())
        party = new Party()
        {
          ID = this.Party.ID,
          Size = this.Party.Size,
          Max = this.Party.Max,
          Privacy = this.Party.Privacy
        };
      else
        party = (Party) null;
      richPresence5.Party = party;
      return richPresence1;
    }

    internal RichPresence Merge(BaseRichPresence presence)
    {
      this._state = presence.State;
      this._details = presence.Details;
      this.Party = presence.Party;
      this.Timestamps = presence.Timestamps;
      this.Secrets = presence.Secrets;
      if (presence.HasAssets())
      {
        if (!this.HasAssets())
          this.Assets = presence.Assets;
        else
          this.Assets.Merge(presence.Assets);
      }
      else
        this.Assets = (Assets) null;
      return this;
    }

    internal override bool Matches(RichPresence other)
    {
      if (!base.Matches(other) || this.Buttons == null ^ other.Buttons == null)
        return false;
      if (this.Buttons != null)
      {
        if (this.Buttons.Length != other.Buttons.Length)
          return false;
        for (int index = 0; index < this.Buttons.Length; ++index)
        {
          Button button1 = this.Buttons[index];
          Button button2 = other.Buttons[index];
          if (button1.Label != button2.Label || button1.Url != button2.Url)
            return false;
        }
      }
      return true;
    }

    public static implicit operator bool(RichPresence presesnce) => presesnce != null;
  }
}
