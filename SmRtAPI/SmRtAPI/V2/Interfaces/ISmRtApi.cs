using System;
using System.Threading;
using Speechmatics.Realtime.Client.V2.Config;

namespace Speechmatics.Realtime.Client.V2.Interfaces
{
    internal interface ISmRtApi
    {
        /// <summary>
        /// All configuration for the stream format and callbacks
        /// </summary>
        SmRtApiConfig Configuration { get; }

        /// <summary>
        /// Cancellation token for async operations
        /// </summary>
        CancellationToken CancelToken { get; }

        /// <summary>
        /// The websocket URL this API instance is connected to
        /// </summary>
        Uri WsUrl { get; }
    }
}