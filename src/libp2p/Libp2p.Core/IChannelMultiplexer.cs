using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nethermind.Libp2p.Core;

    public interface IChannelMultiplexer
    {
        Task<IChannel> DialAsync(CancellationToken cancellationToken = default);
    }