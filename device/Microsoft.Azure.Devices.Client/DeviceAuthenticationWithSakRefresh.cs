﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client
{
    internal class DeviceAuthenticationWithSakRefresh : DeviceAuthenticationWithTokenRefresh
    {
        private IotHubConnectionString _connectionString;

        public DeviceAuthenticationWithSakRefresh(
            string deviceId, 
            IotHubConnectionString connectionString) : base(deviceId)
        {
            _connectionString = connectionString;
        }

        protected override Task<string> SafeCreateNewToken(string iotHub, int suggestedTimeToLive)
        {
            var builder = new SharedAccessSignatureBuilder()
            {
                Key = _connectionString.SharedAccessKey,
                TimeToLive = TimeSpan.FromSeconds(suggestedTimeToLive),
            };

            if (_connectionString.SharedAccessKeyName == null)
            {
                builder.Target = "{0}/devices/{1}".FormatInvariant(
                    iotHub, 
                    WebUtility.UrlEncode(DeviceId));
            }
            else
            {
                builder.KeyName = _connectionString.SharedAccessKeyName;
                builder.Target = _connectionString.Audience;
            }

            return Task.FromResult(builder.ToSignature());
        }
    }
}
