﻿

using System;

 
namespace DiscordRPC.Exceptions
{
  public class StringOutOfRangeException : Exception
  {
    public int MaximumLength { get; private set; }

    public int MinimumLength { get; private set; }

    internal StringOutOfRangeException(string message, int min, int max)
      : base(message)
    {
      this.MinimumLength = min;
      this.MaximumLength = max;
    }

    internal StringOutOfRangeException(int minumum, int max)
      : this(string.Format("Length of string is out of range. Expected a value between {0} and {1}", (object) minumum, (object) max), minumum, max)
    {
    }

    internal StringOutOfRangeException(int max)
      : this(string.Format("Length of string is out of range. Expected a value with a maximum length of {0}", (object) max), 0, max)
    {
    }
  }
}
