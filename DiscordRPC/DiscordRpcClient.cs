
using DiscordRPC.Events;
using DiscordRPC.Exceptions;
using DiscordRPC.IO;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using System;
using System.Diagnostics;

 
namespace DiscordRPC
{
  public sealed class DiscordRpcClient : IDisposable
  {
    private ILogger _logger;
    private RpcConnection connection;
    private bool _shutdownOnly = true;
    private object _sync = new object();

    public bool HasRegisteredUriScheme { get; private set; }

    public string ApplicationID { get; private set; }

    public string SteamID { get; private set; }

    public int ProcessID { get; private set; }

    public int MaxQueueSize { get; private set; }

    public bool IsDisposed { get; private set; }

    public ILogger Logger
    {
      get => this._logger;
      set
      {
        this._logger = value;
        if (this.connection == null)
          return;
        this.connection.Logger = value;
      }
    }

    public bool AutoEvents { get; private set; }

    public bool SkipIdenticalPresence { get; set; }

    public int TargetPipe { get; private set; }

    public RichPresence CurrentPresence { get; private set; }

    public EventType Subscription { get; private set; }

    public User CurrentUser { get; private set; }

    public Configuration Configuration { get; private set; }

    public bool IsInitialized { get; private set; }

    public bool ShutdownOnly
    {
      get => this._shutdownOnly;
      set
      {
        this._shutdownOnly = value;
        if (this.connection == null)
          return;
        this.connection.ShutdownOnly = value;
      }
    }

    public event OnReadyEvent OnReady;

    public event OnCloseEvent OnClose;

    public event OnErrorEvent OnError;

    public event OnPresenceUpdateEvent OnPresenceUpdate;

    public event OnSubscribeEvent OnSubscribe;

    public event OnUnsubscribeEvent OnUnsubscribe;

    public event OnJoinEvent OnJoin;

    public event OnSpectateEvent OnSpectate;

    public event OnJoinRequestedEvent OnJoinRequested;

    public event OnConnectionEstablishedEvent OnConnectionEstablished;

    public event OnConnectionFailedEvent OnConnectionFailed;

    public event OnRpcMessageEvent OnRpcMessage;

    public DiscordRpcClient(string applicationID)
      : this(applicationID, -1, (ILogger) null, true, (INamedPipeClient) null)
    {
    }

    public DiscordRpcClient(
      string applicationID,
      int pipe = -1,
      ILogger logger = null,
      bool autoEvents = true,
      INamedPipeClient client = null)
    {
      this.ApplicationID = !string.IsNullOrEmpty(applicationID) ? applicationID.Trim() : throw new ArgumentNullException(nameof (applicationID));
      this.TargetPipe = pipe;
      this.ProcessID = Process.GetCurrentProcess().Id;
      this.HasRegisteredUriScheme = false;
      this.AutoEvents = autoEvents;
      this.SkipIdenticalPresence = true;
      this._logger = logger ?? (ILogger) new NullLogger();
      this.connection = new RpcConnection(this.ApplicationID, this.ProcessID, this.TargetPipe, client ?? (INamedPipeClient) new ManagedNamedPipeClient(), autoEvents ? 0U : 128U)
      {
        ShutdownOnly = this._shutdownOnly,
        Logger = this._logger
      };
      this.connection.OnRpcMessage += (OnRpcMessageEvent) ((sender, msg) =>
      {
        if (this.OnRpcMessage != null)
          this.OnRpcMessage((object) this, msg);
        if (!this.AutoEvents)
          return;
        this.ProcessMessage(msg);
      });
    }

    public IMessage[] Invoke()
    {
      if (this.AutoEvents)
      {
        this.Logger.Error("Cannot Invoke client when AutomaticallyInvokeEvents has been set.");
        return new IMessage[0];
      }
      IMessage[] imessageArray = this.connection.DequeueMessages();
      for (int index = 0; index < imessageArray.Length; ++index)
        this.ProcessMessage(imessageArray[index]);
      return imessageArray;
    }

    private void ProcessMessage(IMessage message)
    {
      if (message == null)
        return;
      switch (message.Type)
      {
        case MessageType.Ready:
          if (message is ReadyMessage readyMessage)
          {
            lock (this._sync)
            {
              this.Configuration = readyMessage.Configuration;
              this.CurrentUser = readyMessage.User;
            }
            this.SynchronizeState();
          }
          if (this.OnReady == null)
            break;
          this.OnReady((object) this, message as ReadyMessage);
          break;
        case MessageType.Close:
          if (this.OnClose == null)
            break;
          this.OnClose((object) this, message as CloseMessage);
          break;
        case MessageType.Error:
          if (this.OnError == null)
            break;
          this.OnError((object) this, message as ErrorMessage);
          break;
        case MessageType.PresenceUpdate:
          lock (this._sync)
          {
            if (message is PresenceMessage presenceMessage)
            {
              if (presenceMessage.Presence == null)
                this.CurrentPresence = (RichPresence) null;
              else if (this.CurrentPresence == null)
                this.CurrentPresence = new RichPresence().Merge(presenceMessage.Presence);
              else
                this.CurrentPresence.Merge(presenceMessage.Presence);
              presenceMessage.Presence = (BaseRichPresence) this.CurrentPresence;
            }
          }
          if (this.OnPresenceUpdate == null)
            break;
          this.OnPresenceUpdate((object) this, message as PresenceMessage);
          break;
        case MessageType.Subscribe:
          lock (this._sync)
            this.Subscription |= (message as SubscribeMessage).Event;
          if (this.OnSubscribe == null)
            break;
          this.OnSubscribe((object) this, message as SubscribeMessage);
          break;
        case MessageType.Unsubscribe:
          lock (this._sync)
            this.Subscription &= ~(message as UnsubscribeMessage).Event;
          if (this.OnUnsubscribe == null)
            break;
          this.OnUnsubscribe((object) this, message as UnsubscribeMessage);
          break;
        case MessageType.Join:
          if (this.OnJoin == null)
            break;
          this.OnJoin((object) this, message as JoinMessage);
          break;
        case MessageType.Spectate:
          if (this.OnSpectate == null)
            break;
          this.OnSpectate((object) this, message as SpectateMessage);
          break;
        case MessageType.JoinRequest:
          if (this.Configuration != null && message is JoinRequestMessage joinRequestMessage)
            joinRequestMessage.User.SetConfiguration(this.Configuration);
          if (this.OnJoinRequested == null)
            break;
          this.OnJoinRequested((object) this, message as JoinRequestMessage);
          break;
        case MessageType.ConnectionEstablished:
          if (this.OnConnectionEstablished == null)
            break;
          this.OnConnectionEstablished((object) this, message as ConnectionEstablishedMessage);
          break;
        case MessageType.ConnectionFailed:
          if (this.OnConnectionFailed == null)
            break;
          this.OnConnectionFailed((object) this, message as ConnectionFailedMessage);
          break;
        default:
          this.Logger.Error("Message was queued with no appropriate handle! {0}", (object) message.Type);
          break;
      }
    }

    public void Respond(JoinRequestMessage request, bool acceptRequest)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      if (!this.IsInitialized)
        throw new UninitializedException();
      this.connection.EnqueueCommand((ICommand) new RespondCommand()
      {
        Accept = acceptRequest,
        UserID = request.User.ID.ToString()
      });
    }

    public void SetPresence(RichPresence presence)
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      if (!this.IsInitialized)
        this.Logger.Warning("The client is not yet initialized, storing the presence as a state instead.");
      if (presence == null)
      {
        if (!this.SkipIdenticalPresence || this.CurrentPresence != null)
          this.connection.EnqueueCommand((ICommand) new PresenceCommand()
          {
            PID = this.ProcessID,
            Presence = (RichPresence) null
          });
      }
      else
      {
        if (presence.HasSecrets() && !this.HasRegisteredUriScheme)
          throw new BadPresenceException("Cannot send a presence with secrets as this object has not registered a URI scheme. Please enable the uri scheme registration in the DiscordRpcClient constructor.");
        if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
          throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
        if (presence.HasSecrets() && !presence.HasParty())
          this.Logger.Warning("The presence has set the secrets but no buttons will show as there is no party available.");
        if (!this.SkipIdenticalPresence || !presence.Matches(this.CurrentPresence))
          this.connection.EnqueueCommand((ICommand) new PresenceCommand()
          {
            PID = this.ProcessID,
            Presence = presence.Clone()
          });
      }
      lock (this._sync)
        this.CurrentPresence = presence?.Clone();
    }

    public RichPresence UpdateButtons(Button[] button = null)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Buttons = button;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence SetButton(Button button, int index = 0)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Buttons[index] = button;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateDetails(string details)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Details = details;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateState(string state)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.State = state;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateParty(Party party)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Party = party;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdatePartySize(int size)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Party == null)
        throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
      presence.Party.Size = size;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdatePartySize(int size, int max)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Party == null)
        throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
      presence.Party.Size = size;
      presence.Party.Max = max;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateLargeAsset(string key = null, string tooltip = null)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Assets == null)
        presence.Assets = new Assets();
      presence.Assets.LargeImageKey = key ?? presence.Assets.LargeImageKey;
      presence.Assets.LargeImageText = tooltip ?? presence.Assets.LargeImageText;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateSmallAsset(string key = null, string tooltip = null)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Assets == null)
        presence.Assets = new Assets();
      presence.Assets.SmallImageKey = key ?? presence.Assets.SmallImageKey;
      presence.Assets.SmallImageText = tooltip ?? presence.Assets.SmallImageText;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateSecrets(Secrets secrets)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Secrets = secrets;
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateStartTime() => this.UpdateStartTime(DateTime.UtcNow);

    public RichPresence UpdateStartTime(DateTime time)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Timestamps == null)
        presence.Timestamps = new Timestamps();
      presence.Timestamps.Start = new DateTime?(time);
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateEndTime() => this.UpdateEndTime(DateTime.UtcNow);

    public RichPresence UpdateEndTime(DateTime time)
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      if (presence.Timestamps == null)
        presence.Timestamps = new Timestamps();
      presence.Timestamps.End = new DateTime?(time);
      this.SetPresence(presence);
      return presence;
    }

    public RichPresence UpdateClearTime()
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      RichPresence presence;
      lock (this._sync)
        presence = this.CurrentPresence != null ? this.CurrentPresence.Clone() : new RichPresence();
      presence.Timestamps = (Timestamps) null;
      this.SetPresence(presence);
      return presence;
    }

    public void ClearPresence()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (!this.IsInitialized)
        throw new UninitializedException();
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      this.SetPresence((RichPresence) null);
    }

    public void Subscribe(EventType type) => this.SetSubscription(this.Subscription | type);

    [Obsolete("Replaced with Unsubscribe", true)]
    public void Unubscribe(EventType type) => this.SetSubscription(this.Subscription & ~type);

    public void Unsubscribe(EventType type) => this.SetSubscription(this.Subscription & ~type);

    public void SetSubscription(EventType type)
    {
      if (this.IsInitialized)
      {
        this.SubscribeToTypes(this.Subscription & ~type, true);
        this.SubscribeToTypes(~this.Subscription & type, false);
      }
      else
        this.Logger.Warning("Client has not yet initialized, but events are being subscribed too. Storing them as state instead.");
      lock (this._sync)
        this.Subscription = type;
    }

    private void SubscribeToTypes(EventType type, bool isUnsubscribe)
    {
      if (type == EventType.None)
        return;
      if (this.IsDisposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (!this.IsInitialized)
        throw new UninitializedException();
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      if (!this.HasRegisteredUriScheme)
        throw new InvalidConfigurationException("Cannot subscribe/unsubscribe to an event as this application has not registered a URI Scheme. Call RegisterUriScheme().");
      if ((type & EventType.Spectate) == EventType.Spectate)
        this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
        {
          Event = ServerEvent.ActivitySpectate,
          IsUnsubscribe = isUnsubscribe
        });
      if ((type & EventType.Join) == EventType.Join)
        this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
        {
          Event = ServerEvent.ActivityJoin,
          IsUnsubscribe = isUnsubscribe
        });
      if ((type & EventType.JoinRequest) != EventType.JoinRequest)
        return;
      this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
      {
        Event = ServerEvent.ActivityJoinRequest,
        IsUnsubscribe = isUnsubscribe
      });
    }

    public void SynchronizeState()
    {
      if (!this.IsInitialized)
        throw new UninitializedException();
      this.SetPresence(this.CurrentPresence);
      if (!this.HasRegisteredUriScheme)
        return;
      this.SubscribeToTypes(this.Subscription, false);
    }

    public bool Initialize()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.IsInitialized)
        throw new UninitializedException("Cannot initialize a client that is already initialized");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      return this.IsInitialized = this.connection.AttemptConnection();
    }

    public void Deinitialize()
    {
      if (!this.IsInitialized)
        throw new UninitializedException("Cannot deinitialize a client that has not been initalized.");
      this.connection.Close();
      this.IsInitialized = false;
    }

    public void Dispose()
    {
      if (this.IsDisposed)
        return;
      if (this.IsInitialized)
        this.Deinitialize();
      this.IsDisposed = true;
    }
  }
}
