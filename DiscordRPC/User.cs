
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
  public class User
  {
    [JsonProperty("id")]
    public ulong ID { get; private set; }

    [JsonProperty("username")]
    public string Username { get; private set; }

    [JsonProperty("discriminator")]
    [Obsolete("Discord no longer uses discriminators.")]
    public int Discriminator { get; private set; }

    [JsonProperty("global_name")]
    public string DisplayName { get; private set; }

    [JsonProperty("avatar")]
    public string Avatar { get; private set; }

    [JsonProperty("flags")]
    public User.Flag Flags { get; private set; }

    [JsonProperty("premium_type")]
    public User.PremiumType Premium { get; private set; }

    public string CdnEndpoint { get; private set; }

    internal User() => this.CdnEndpoint = "cdn.discordapp.com";

    internal void SetConfiguration(Configuration configuration)
    {
      this.CdnEndpoint = configuration.CdnHost;
    }

    public string GetAvatarURL(User.AvatarFormat format)
    {
      return this.GetAvatarURL(format, User.AvatarSize.x128);
    }

    public string GetAvatarURL(User.AvatarFormat format, User.AvatarSize size)
    {
      string str = string.Format("/avatars/{0}/{1}", (object) this.ID, (object) this.Avatar);
      if (string.IsNullOrEmpty(this.Avatar))
      {
        if (format != 0)
          throw new BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");
        int num = (int) ((this.ID >> 22) % 6UL);
        if (this.Discriminator > 0)
          num = this.Discriminator % 5;
        str = string.Format("/embed/avatars/{0}", (object) num);
      }
      return string.Format("https://{0}{1}{2}?size={3}", (object) this.CdnEndpoint, (object) str, (object) this.GetAvatarExtension(format), (object) (int) size);
    }

    public string GetAvatarExtension(User.AvatarFormat format)
    {
      return "." + format.ToString().ToLowerInvariant();
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(this.DisplayName))
        return this.DisplayName;
      return this.Discriminator != 0 ? this.Username + "#" + this.Discriminator.ToString("D4") : this.Username;
    }

    public enum AvatarFormat
    {
      PNG,
      JPEG,
      WebP,
      GIF,
    }

    public enum AvatarSize
    {
      x16 = 16, // 0x00000010
      x32 = 32, // 0x00000020
      x64 = 64, // 0x00000040
      x128 = 128, // 0x00000080
      x256 = 256, // 0x00000100
      x512 = 512, // 0x00000200
      x1024 = 1024, // 0x00000400
      x2048 = 2048, // 0x00000800
    }

    [System.Flags]
    public enum Flag
    {
      None = 0,
      Employee = 1,
      Partner = 2,
      HypeSquad = 4,
      BugHunter = 8,
      HouseBravery = 64, // 0x00000040
      HouseBrilliance = 128, // 0x00000080
      HouseBalance = 256, // 0x00000100
      EarlySupporter = 512, // 0x00000200
      TeamUser = 1024, // 0x00000400
    }

    public enum PremiumType
    {
      None,
      NitroClassic,
      Nitro,
    }
  }
}
