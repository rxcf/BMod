
using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;

 
namespace DiscordRPC
{
  public class Button
  {
    private string _label;
    private string _url;

    [JsonProperty("label")]
    public string Label
    {
      get => this._label;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._label, 32, Encoding.UTF8))
          throw new StringOutOfRangeException(32);
      }
    }

    [JsonProperty("url")]
    public string Url
    {
      get => this._url;
      set
      {
        if (!BaseRichPresence.ValidateString(value, out this._url, 512, Encoding.UTF8))
          throw new StringOutOfRangeException(512);
        if (!Uri.TryCreate(this._url, UriKind.Absolute, out Uri _))
          throw new ArgumentException("Url must be a valid URI");
      }
    }
  }
}
