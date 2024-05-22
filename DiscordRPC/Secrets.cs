
using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
  [Serializable]
  public class Secrets
  {
    private string _matchSecret;
    private string _joinSecret;
    private string _spectateSecret;

    [Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.")]
    [JsonProperty("match")]
    public string MatchSecret
    {
      get => this._matchSecret;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._matchSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    [JsonProperty("join")]
    public string JoinSecret
    {
      get => this._joinSecret;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._joinSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    [JsonProperty("spectate")]
    public string SpectateSecret
    {
      get => this._spectateSecret;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._spectateSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    public static Encoding Encoding => Encoding.UTF8;

    public static int SecretLength => 128;

    public static string CreateSecret(Random random)
    {
      byte[] numArray = new byte[Secrets.SecretLength];
      random.NextBytes(numArray);
      return Secrets.Encoding.GetString(numArray);
    }

    public static string CreateFriendlySecret(Random random)
    {
      string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < Secrets.SecretLength; ++index)
        stringBuilder.Append(str[random.Next(str.Length)]);
      return stringBuilder.ToString();
    }
  }
}
