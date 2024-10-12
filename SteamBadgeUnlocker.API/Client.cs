using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SteamBadgeUnlocker.API
{
    public class Client : IDisposable
    {
        public Wrappers.SteamClient018 SteamClient;
        public Wrappers.SteamUser012 SteamUser;
        public Wrappers.SteamUserStats013 SteamUserStats;
        public Wrappers.SteamUtils005 SteamUtils;
        public Wrappers.SteamApps001 SteamApps001;
        public Wrappers.SteamApps008 SteamApps008;

        private bool _IsDisposed = false;
        private int _Pipe;
        private int _User;

        private readonly List<ICallback> _Callbacks = new();

        public void Initialize(long appId)
        {
            if (string.IsNullOrEmpty(Steam.GetInstallPath()) == true)
            {
                throw new ClientInitializeException(ClientInitializeFailure.GetInstallPath, "failed to get Steam install path");
            }

            if (appId != 0)
            {
                Environment.SetEnvironmentVariable("SteamAppId", appId.ToString(CultureInfo.InvariantCulture));
            }

            if (Steam.Load() == false)
            {
                throw new ClientInitializeException(ClientInitializeFailure.Load, "failed to load SteamClient");
            }

            this.SteamClient = Steam.CreateInterface<Wrappers.SteamClient018>("SteamClient018");
            if (this.SteamClient == null)
            {
                throw new ClientInitializeException(ClientInitializeFailure.CreateSteamClient, "failed to create ISteamClient018");
            }

            this._Pipe = this.SteamClient.CreateSteamPipe();
            if (this._Pipe == 0)
            {
                throw new ClientInitializeException(ClientInitializeFailure.CreateSteamPipe, "failed to create pipe");
            }

            this._User = this.SteamClient.ConnectToGlobalUser(this._Pipe);
            if (this._User == 0)
            {
                throw new ClientInitializeException(ClientInitializeFailure.ConnectToGlobalUser, "failed to connect to global user");
            }

            this.SteamUtils = this.SteamClient.GetSteamUtils004(this._Pipe);
            if (appId > 0 && this.SteamUtils.GetAppId() != (uint)appId)
            {
                throw new ClientInitializeException(ClientInitializeFailure.AppIdMismatch, "appID mismatch");
            }

            this.SteamUser = this.SteamClient.GetSteamUser012(this._User, this._Pipe);
            this.SteamUserStats = this.SteamClient.GetSteamUserStats013(this._User, this._Pipe);
            this.SteamApps001 = this.SteamClient.GetSteamApps001(this._User, this._Pipe);
            this.SteamApps008 = this.SteamClient.GetSteamApps008(this._User, this._Pipe);
        }

        ~Client()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._IsDisposed == true)
            {
                return;
            }

            if (this.SteamClient != null && this._Pipe > 0)
            {
                if (this._User > 0)
                {
                    this.SteamClient.ReleaseUser(this._Pipe, this._User);
                    this._User = 0;
                }

                this.SteamClient.ReleaseSteamPipe(this._Pipe);
                this._Pipe = 0;
            }

            this._IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TCallback CreateAndRegisterCallback<TCallback>()
            where TCallback : ICallback, new()
        {
            TCallback callback = new();
            this._Callbacks.Add(callback);
            return callback;
        }

        private bool _RunningCallbacks;

        public void RunCallbacks(bool server)
        {
            if (this._RunningCallbacks == true)
            {
                return;
            }

            this._RunningCallbacks = true;

            Types.CallbackMessage message;
            while (Steam.GetCallback(this._Pipe, out message, out _) == true)
            {
                var callbackId = message.Id;
                foreach (ICallback callback in this._Callbacks.Where(
                    candidate => candidate.Id == callbackId &&
                                 candidate.IsServer == server))
                {
                    callback.Run(message.ParamPointer);
                }
                Steam.FreeLastCallback(this._Pipe);
            }

            this._RunningCallbacks = false;
        }
    }
}
