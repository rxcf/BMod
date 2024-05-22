

using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;
using System.Text;

 
namespace DiscordRPC
{
  [JsonObject]
  [Serializable]
  public class BaseRichPresence
  {
    protected internal string _state;
    protected internal string _details;

    [JsonProperty("state")]
    public string State
    {
      get => this._state;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._state, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (State), 0, 128);
      }
    }

    [JsonProperty("details")]
    public string Details
    {
      get => this._details;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._details, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    [JsonProperty("timestamps")]
    public Timestamps Timestamps { get; set; }

    [JsonProperty("assets")]
    public Assets Assets { get; set; }

    [JsonProperty("party")]
    public Party Party { get; set; }

    [JsonProperty("secrets")]
    public Secrets Secrets { get; set; }

    [JsonProperty("instance")]
    [Obsolete("This was going to be used, but was replaced by JoinSecret instead")]
    private bool Instance { get; set; }

    public bool HasTimestamps()
    {
      int num;
      if (this.Timestamps != null)
      {
        DateTime? nullable = this.Timestamps.Start;
        if (!nullable.HasValue)
        {
          nullable = this.Timestamps.End;
          num = nullable.HasValue ? 1 : 0;
        }
        else
          num = 1;
      }
      else
        num = 0;
      return num != 0;
    }

    public bool HasAssets() => this.Assets != null;

    public bool HasParty() => this.Party != null && this.Party.ID != null;

    public bool HasSecrets()
    {
      return this.Secrets != null && (this.Secrets.JoinSecret != null || this.Secrets.SpectateSecret != null);
    }

    internal static bool ValidateString(
      string str,
      out string result,
      int bytes,
      Encoding encoding)
    {
      result = str;
      if (str == null)
        return true;
      string str1 = str.Trim();
      if (!str1.WithinLength(bytes, encoding))
        return false;
      result = str1.GetNullOrString();
      return true;
    }

    public static implicit operator bool(BaseRichPresence presesnce) => presesnce != null;

    internal virtual bool Matches(RichPresence other)
    {
      if (other == null || this.State != other.State || this.Details != other.Details)
        return false;
      if (this.Timestamps != null)
      {
        int num;
        if (other.Timestamps != null)
        {
          ulong? unixMilliseconds1 = other.Timestamps.StartUnixMilliseconds;
          ulong? unixMilliseconds2 = this.Timestamps.StartUnixMilliseconds;
          if ((long) unixMilliseconds1.GetValueOrDefault() == (long) unixMilliseconds2.GetValueOrDefault() & unixMilliseconds1.HasValue == unixMilliseconds2.HasValue)
          {
            unixMilliseconds2 = other.Timestamps.EndUnixMilliseconds;
            unixMilliseconds1 = this.Timestamps.EndUnixMilliseconds;
            num = !((long) unixMilliseconds2.GetValueOrDefault() == (long) unixMilliseconds1.GetValueOrDefault() & unixMilliseconds2.HasValue == unixMilliseconds1.HasValue) ? 1 : 0;
            goto label_7;
          }
        }
        num = 1;
label_7:
        if (num != 0)
          return false;
      }
      else if (other.Timestamps != null)
        return false;
      if (this.Secrets != null)
      {
        if (other.Secrets == null || other.Secrets.JoinSecret != this.Secrets.JoinSecret || other.Secrets.MatchSecret != this.Secrets.MatchSecret || other.Secrets.SpectateSecret != this.Secrets.SpectateSecret)
          return false;
      }
      else if (other.Secrets != null)
        return false;
      if (this.Party != null)
      {
        if (other.Party == null || other.Party.ID != this.Party.ID || other.Party.Max != this.Party.Max || other.Party.Size != this.Party.Size || other.Party.Privacy != this.Party.Privacy)
          return false;
      }
      else if (other.Party != null)
        return false;
      if (this.Assets != null)
      {
        if (other.Assets == null || other.Assets.LargeImageKey != this.Assets.LargeImageKey || other.Assets.LargeImageText != this.Assets.LargeImageText || other.Assets.SmallImageKey != this.Assets.SmallImageKey || other.Assets.SmallImageText != this.Assets.SmallImageText)
          return false;
      }
      else if (other.Assets != null)
        return false;
      return this.Instance == other.Instance;
    }

    public RichPresence ToRichPresence()
    {
      RichPresence richPresence = new RichPresence();
      richPresence.State = this.State;
      richPresence.Details = this.Details;
      richPresence.Party = !this.HasParty() ? this.Party : (Party) null;
      richPresence.Secrets = !this.HasSecrets() ? this.Secrets : (Secrets) null;
      if (this.HasAssets())
        richPresence.Assets = new Assets()
        {
          SmallImageKey = this.Assets.SmallImageKey,
          SmallImageText = this.Assets.SmallImageText,
          LargeImageKey = this.Assets.LargeImageKey,
          LargeImageText = this.Assets.LargeImageText
        };
      if (this.HasTimestamps())
      {
        richPresence.Timestamps = new Timestamps();
        if (this.Timestamps.Start.HasValue)
          richPresence.Timestamps.Start = this.Timestamps.Start;
        if (this.Timestamps.End.HasValue)
          richPresence.Timestamps.End = this.Timestamps.End;
      }
      return richPresence;
    }
  }
}
