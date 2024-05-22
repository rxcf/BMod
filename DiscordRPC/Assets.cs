
using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;

 
namespace DiscordRPC
{
  [Serializable]
  public class Assets
  {
    private string _largeimagekey;
    private bool _islargeimagekeyexternal;
    private string _largeimagetext;
    private string _smallimagekey;
    private bool _issmallimagekeyexternal;
    private string _smallimagetext;
    private ulong? _largeimageID;
    private ulong? _smallimageID;

    [JsonProperty("large_image")]
    public string LargeImageKey
    {
      get => this._largeimagekey;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._largeimagekey, 256, Encoding.UTF8))
          throw new StringOutOfRangeException(256);
        string largeimagekey = this._largeimagekey;
        this._islargeimagekeyexternal = largeimagekey != null && largeimagekey.StartsWith("mp:external/");
        this._largeimageID = new ulong?();
      }
    }

    [JsonIgnore]
    public bool IsLargeImageKeyExternal => this._islargeimagekeyexternal;

    [JsonProperty("large_text")]
    public string LargeImageText
    {
      get => this._largeimagetext;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._largeimagetext, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    [JsonProperty("small_image")]
    public string SmallImageKey
    {
      get => this._smallimagekey;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._smallimagekey, 256, Encoding.UTF8))
          throw new StringOutOfRangeException(256);
        string smallimagekey = this._smallimagekey;
        this._issmallimagekeyexternal = smallimagekey != null && smallimagekey.StartsWith("mp:external/");
        this._smallimageID = new ulong?();
      }
    }

    [JsonIgnore]
    public bool IsSmallImageKeyExternal => this._issmallimagekeyexternal;

    [JsonProperty("small_text")]
    public string SmallImageText
    {
      get => this._smallimagetext;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._smallimagetext, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(128);
      }
    }

    [JsonIgnore]
    public ulong? LargeImageID => this._largeimageID;

    [JsonIgnore]
    public ulong? SmallImageID => this._smallimageID;

    internal void Merge(Assets other)
    {
      this._smallimagetext = other._smallimagetext;
      this._largeimagetext = other._largeimagetext;
      ulong result1;
      if (ulong.TryParse(other._largeimagekey, out result1))
      {
        this._largeimageID = new ulong?(result1);
      }
      else
      {
        this._largeimagekey = other._largeimagekey;
        this._largeimageID = new ulong?();
      }
      ulong result2;
      if (ulong.TryParse(other._smallimagekey, out result2))
      {
        this._smallimageID = new ulong?(result2);
      }
      else
      {
        this._smallimagekey = other._smallimagekey;
        this._smallimageID = new ulong?();
      }
    }
  }
}
