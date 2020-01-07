//
// Copyright 2018-2019 Dynatrace LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Dynatrace.OpenKit.API;
using Dynatrace.OpenKit.Core.Caching;
using Dynatrace.OpenKit.Core.Configuration;
using Dynatrace.OpenKit.Protocol;
using Dynatrace.OpenKit.Providers;

namespace Dynatrace.OpenKit.Core.Objects
{
    public class SessionCreator : ISessionCreator, IBeaconInitializer
    {
        // OpenKit related configuration
        private readonly IOpenKitConfiguration openKitConfiguration;
        // privacy related configuration
        private readonly IPrivacyConfiguration privacyConfiguration;


        private readonly int serverId;

        internal SessionCreator(ISessionCreatorInput input, string clientIpAddress)
        {
            Logger = input.Logger;
            openKitConfiguration = input.OpenKitConfiguration;
            privacyConfiguration = input.PrivacyConfiguration;
            BeaconCache = input.BeaconCache;
            ThreadIdProvider = input.ThreadIdProvider;
            TimingProvider = input.TimingProvider;
            ClientIpAddress = clientIpAddress;

            serverId = input.CurrentServerId;
            SessionIdProvider = new FixedSessionIdProvider(input.SessionIdProvider);
            RandomNumberGenerator = new FixedPrnGenerator(new DefaultPrnGenerator());
        }

        ISessionInternals ISessionCreator.CreateSession(IOpenKitComposite parent)
        {
            var configuration = BeaconConfiguration.From(openKitConfiguration, privacyConfiguration, serverId);
            var beacon = new Beacon(this, configuration);

            var session = new Session(Logger, parent, beacon);
            SessionSequenceNumber++;

            return session;
        }

        #region IBeaconInitializer implementation

        public ILogger Logger { get; }

        public IBeaconCache BeaconCache { get; }

        public string ClientIpAddress { get; }

        public ISessionIdProvider SessionIdProvider { get; }

        public int SessionSequenceNumber { get; private set; }

        public IThreadIdProvider ThreadIdProvider { get; }

        public ITimingProvider TimingProvider { get; }

        public IPrnGenerator RandomNumberGenerator { get; }

        #endregion
    }
}