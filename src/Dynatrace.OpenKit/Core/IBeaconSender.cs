//
// Copyright 2018-2021 Dynatrace LLC
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

using Dynatrace.OpenKit.Core.Configuration;
using Dynatrace.OpenKit.Core.Objects;

namespace Dynatrace.OpenKit.Core
{
    internal interface IBeaconSender
    {

        bool IsInitialized { get; }

        void Initialize();

        bool WaitForInitCompletion();

        bool WaitForInitCompletion(int timeoutMillis);

        void Shutdown();

        IServerConfiguration LastServerConfiguration { get; }

        int CurrentServerId { get; }

        void AddSession(ISessionInternals session);
    }
}