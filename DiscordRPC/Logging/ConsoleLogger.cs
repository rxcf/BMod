
using System;

 
namespace DiscordRPC.Logging
{
  public class ConsoleLogger : ILogger
  {
    public LogLevel Level { get; set; }

    public bool Coloured { get; set; }

    [Obsolete("Use Coloured")]
    public bool Colored
    {
      get => this.Coloured;
      set => this.Coloured = value;
    }

    public ConsoleLogger()
    {
      this.Level = LogLevel.Info;
      this.Coloured = false;
    }

    public ConsoleLogger(LogLevel level)
      : this()
    {
      this.Level = level;
    }

    public ConsoleLogger(LogLevel level, bool coloured)
    {
      this.Level = level;
      this.Coloured = coloured;
    }

    public void Trace(string message, params object[] args)
    {
      if (this.Level > LogLevel.Trace)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.Gray;
      string format = "TRACE: " + message;
      if (args.Length != 0)
        Console.WriteLine(format, args);
      else
        Console.WriteLine(format);
    }

    public void Info(string message, params object[] args)
    {
      if (this.Level > LogLevel.Info)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.White;
      string format = "INFO: " + message;
      if (args.Length != 0)
        Console.WriteLine(format, args);
      else
        Console.WriteLine(format);
    }

    public void Warning(string message, params object[] args)
    {
      if (this.Level > LogLevel.Warning)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.Yellow;
      string format = "WARN: " + message;
      if (args.Length != 0)
        Console.WriteLine(format, args);
      else
        Console.WriteLine(format);
    }

    public void Error(string message, params object[] args)
    {
      if (this.Level > LogLevel.Error)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.Red;
      string format = "ERR : " + message;
      if (args.Length != 0)
        Console.WriteLine(format, args);
      else
        Console.WriteLine(format);
    }
  }
}
