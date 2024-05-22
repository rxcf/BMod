

using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

 
namespace DiscordRPC.IO
{
  public sealed class ManagedNamedPipeClient : INamedPipeClient, IDisposable
  {
    private const string PIPE_NAME = "discord-ipc-{0}";
    private int _connectedPipe;
    private NamedPipeClientStream _stream;
    private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];
    private Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();
    private object _framequeuelock = new object();
    private volatile bool _isDisposed = false;
    private volatile bool _isClosed = true;
    private object l_stream = new object();

    public ILogger Logger { get; set; }

    public bool IsConnected
    {
      get
      {
        if (this._isClosed)
          return false;
        lock (this.l_stream)
          return this._stream != null && this._stream.IsConnected;
      }
    }

    public int ConnectedPipe => this._connectedPipe;

    public ManagedNamedPipeClient()
    {
      this._buffer = new byte[PipeFrame.MAX_SIZE];
      this.Logger = (ILogger) new NullLogger();
      this._stream = (NamedPipeClientStream) null;
    }

    public bool Connect(int pipe)
    {
      this.Logger.Trace("ManagedNamedPipeClient.Connection({0})", (object) pipe);
      if (this._isDisposed)
        throw new ObjectDisposedException("NamedPipe");
      if (pipe > 9)
        throw new ArgumentOutOfRangeException(nameof (pipe), "Argument cannot be greater than 9");
      if (pipe < 0)
      {
        for (int pipe1 = 0; pipe1 < 10; ++pipe1)
        {
          if (this.AttemptConnection(pipe1) || this.AttemptConnection(pipe1, true))
          {
            this.BeginReadStream();
            return true;
          }
        }
      }
      else if (this.AttemptConnection(pipe) || this.AttemptConnection(pipe, true))
      {
        this.BeginReadStream();
        return true;
      }
      return false;
    }

    private bool AttemptConnection(int pipe, bool isSandbox = false)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      string pipeSandbox = isSandbox ? ManagedNamedPipeClient.GetPipeSandbox() : "";
      if (isSandbox && pipeSandbox == null)
      {
        this.Logger.Trace("Skipping sandbox connection.");
        return false;
      }
      this.Logger.Trace("Connection Attempt {0} ({1})", (object) pipe, (object) pipeSandbox);
      string pipeName = ManagedNamedPipeClient.GetPipeName(pipe, pipeSandbox);
      try
      {
        lock (this.l_stream)
        {
          this.Logger.Info("Attempting to connect to '{0}'", (object) pipeName);
          this._stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
          this._stream.Connect(0);
          this.Logger.Trace("Waiting for connection...");
          do
          {
            Thread.Sleep(10);
          }
          while (!this._stream.IsConnected);
        }
        this.Logger.Info("Connected to '{0}'", (object) pipeName);
        this._connectedPipe = pipe;
        this._isClosed = false;
      }
      catch (Exception ex)
      {
        this.Logger.Error("Failed connection to {0}. {1}", (object) pipeName, (object) ex.Message);
        this.Close();
      }
      this.Logger.Trace("Done. Result: {0}", (object) this._isClosed);
      return !this._isClosed;
    }

    private void BeginReadStream()
    {
      if (this._isClosed)
        return;
      try
      {
        lock (this.l_stream)
        {
          if (this._stream == null || !this._stream.IsConnected)
            return;
          this.Logger.Trace("Begining Read of {0} bytes", (object) this._buffer.Length);
          this._stream.BeginRead(this._buffer, 0, this._buffer.Length, new AsyncCallback(this.EndReadStream), (object) this._stream.IsConnected);
        }
      }
      catch (ObjectDisposedException ex)
      {
        this.Logger.Warning("Attempted to start reading from a disposed pipe");
      }
      catch (InvalidOperationException ex)
      {
        this.Logger.Warning("Attempted to start reading from a closed pipe");
      }
      catch (Exception ex)
      {
        this.Logger.Error("An exception occured while starting to read a stream: {0}", (object) ex.Message);
        this.Logger.Error(ex.StackTrace);
      }
    }

    private void EndReadStream(IAsyncResult callback)
    {
      this.Logger.Trace("Ending Read");
      int count = 0;
      try
      {
        lock (this.l_stream)
        {
          if (this._stream == null || !this._stream.IsConnected)
            return;
          count = this._stream.EndRead(callback);
        }
      }
      catch (IOException ex)
      {
        this.Logger.Warning("Attempted to end reading from a closed pipe");
        return;
      }
      catch (NullReferenceException ex)
      {
        this.Logger.Warning("Attempted to read from a null pipe");
        return;
      }
      catch (ObjectDisposedException ex)
      {
        this.Logger.Warning("Attemped to end reading from a disposed pipe");
        return;
      }
      catch (Exception ex)
      {
        this.Logger.Error("An exception occured while ending a read of a stream: {0}", (object) ex.Message);
        this.Logger.Error(ex.StackTrace);
        return;
      }
      this.Logger.Trace("Read {0} bytes", (object) count);
      if (count > 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(this._buffer, 0, count))
        {
          try
          {
            PipeFrame pipeFrame = new PipeFrame();
            if (pipeFrame.ReadStream((Stream) memoryStream))
            {
              this.Logger.Trace("Read a frame: {0}", (object) pipeFrame.Opcode);
              lock (this._framequeuelock)
                this._framequeue.Enqueue(pipeFrame);
            }
            else
            {
              this.Logger.Error("Pipe failed to read from the data received by the stream.");
              this.Close();
            }
          }
          catch (Exception ex)
          {
            this.Logger.Error("A exception has occured while trying to parse the pipe data: {0}", (object) ex.Message);
            this.Close();
          }
        }
      }
      else if (ManagedNamedPipeClient.IsUnix())
      {
        this.Logger.Error("Empty frame was read on {0}, aborting.", (object) Environment.OSVersion);
        this.Close();
      }
      else
        this.Logger.Warning("Empty frame was read. Please send report to Lachee.");
      if (this._isClosed || !this.IsConnected)
        return;
      this.Logger.Trace("Starting another read");
      this.BeginReadStream();
    }

    public bool ReadFrame(out PipeFrame frame)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      lock (this._framequeuelock)
      {
        if (this._framequeue.Count == 0)
        {
          frame = new PipeFrame();
          return false;
        }
        frame = this._framequeue.Dequeue();
        return true;
      }
    }

    public bool WriteFrame(PipeFrame frame)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      if (this._isClosed || !this.IsConnected)
      {
        this.Logger.Error("Failed to write frame because the stream is closed");
        return false;
      }
      try
      {
        frame.WriteStream((Stream) this._stream);
        return true;
      }
      catch (IOException ex)
      {
        this.Logger.Error("Failed to write frame because of a IO Exception: {0}", (object) ex.Message);
      }
      catch (ObjectDisposedException ex)
      {
        this.Logger.Warning("Failed to write frame as the stream was already disposed");
      }
      catch (InvalidOperationException ex)
      {
        this.Logger.Warning("Failed to write frame because of a invalid operation");
      }
      return false;
    }

    public void Close()
    {
      if (this._isClosed)
      {
        this.Logger.Warning("Tried to close a already closed pipe.");
      }
      else
      {
        try
        {
          lock (this.l_stream)
          {
            if (this._stream != null)
            {
              try
              {
                this._stream.Flush();
                this._stream.Dispose();
              }
              catch (Exception ex)
              {
              }
              this._stream = (NamedPipeClientStream) null;
              this._isClosed = true;
            }
            else
              this.Logger.Warning("Stream was closed, but no stream was available to begin with!");
          }
        }
        catch (ObjectDisposedException ex)
        {
          this.Logger.Warning("Tried to dispose already disposed stream");
        }
        finally
        {
          this._isClosed = true;
          this._connectedPipe = -1;
        }
      }
    }

    public void Dispose()
    {
      if (this._isDisposed)
        return;
      if (!this._isClosed)
        this.Close();
      lock (this.l_stream)
      {
        if (this._stream != null)
        {
          this._stream.Dispose();
          this._stream = (NamedPipeClientStream) null;
        }
      }
      this._isDisposed = true;
    }

    public static string GetPipeName(int pipe, string sandbox)
    {
      return !ManagedNamedPipeClient.IsUnix() ? sandbox + string.Format("discord-ipc-{0}", (object) pipe) : Path.Combine(ManagedNamedPipeClient.GetTemporaryDirectory(), sandbox + string.Format("discord-ipc-{0}", (object) pipe));
    }

    public static string GetPipeName(int pipe) => ManagedNamedPipeClient.GetPipeName(pipe, "");

    public static string GetPipeSandbox()
    {
      return Environment.OSVersion.Platform != PlatformID.Unix ? (string) null : "snap.discord/";
    }

    private static string GetTemporaryDirectory()
    {
      return (((((string) null ?? Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR")) ?? Environment.GetEnvironmentVariable("TMPDIR")) ?? Environment.GetEnvironmentVariable("TMP")) ?? Environment.GetEnvironmentVariable("TEMP")) ?? "/tmp";
    }

    public static bool IsUnix()
    {
      switch (Environment.OSVersion.Platform)
      {
        case PlatformID.Unix:
        case PlatformID.MacOSX:
          return true;
        default:
          return false;
      }
    }
  }
}
