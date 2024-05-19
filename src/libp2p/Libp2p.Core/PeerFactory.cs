// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using Multiformats.Address;
using Multiformats.Address.Protocols;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Nethermind.Libp2p.Core;

public class PeerFactory : IPeerFactory
{
    private readonly IServiceProvider _serviceProvider;

    private IProtocol _protocol;
    private IChannelFactory _upChannelFactory;
    private static int CtxId = 0;

    private ConcurrentDictionary<Identity, IRemotePeer> RemotePeers = new();

    public PeerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public virtual ILocalPeer Create(Identity identity, Multiaddress? localAddr = default)
    {
        var peer = new LocalPeer(this, identity);
        peer.GetOrAddAddress(localAddr ?? $"/ip4/0.0.0.0/tcp/0/{identity.PeerId}");
        return peer;
    }

    /// <summary>
    /// PeerFactory interface ctor
    /// </summary>
    /// <param name="upChannelFactory"></param>
    /// <param name="appFactory"></param>
    /// <param name="protocol"></param>
    /// <param name="appLayerProtocols"></param>
    public void Setup(IProtocol protocol, IChannelFactory upChannelFactory)
    {
        _protocol = protocol;
        _upChannelFactory = upChannelFactory;
    }

    private IRemotePeer GetRemotePeer(Identity identity) {
        return RemotePeers.GetOrAdd(identity, (identity) => new RemotePeer(this, identity));
    }
    private async Task<IListener> ListenAsync(LocalPeer peer, Multiaddress addr, CancellationToken token)
    {
        if (!addr.Has<P2P>())
        {
            addr = addr.Add<P2P>(peer.Identity.PeerId.ToString());
        }
        peer.GetOrAddAddress(addr);


        Channel chan = new();
        if (token != default)
        {
            token.Register(() => chan.CloseAsync());
        }

        TaskCompletionSource ts = new();


        PeerContext peerContext = new()
        {
            Id = $"ctx-{++CtxId}",
            LocalPeer = peer,
        };

        peerContext.OnListenerReady += OnListenerReady;

        void OnListenerReady()
        {
            ts.SetResult();
            peerContext.OnListenerReady -= OnListenerReady;
        }


        PeerListener result = new(chan, peer);
        peerContext.OnRemotePeerConnection += remotePeer =>
        {
            ConnectedTo(remotePeer, false)
                .ContinueWith(t => { result.RaiseOnConnection(remotePeer); }, token);
        };
        _ = _protocol.ListenAsync(chan, _upChannelFactory, peerContext);

        await ts.Task;
        return result;
    }

    protected virtual Task ConnectedTo(IRemotePeer peer, bool isDialer)
    {
        return Task.CompletedTask;
    }

    private Task DialAsync<TProtocol>(IPeerContext peerContext, CancellationToken token) where TProtocol : IProtocol
    {
        TaskCompletionSource cts = new(token);
        peerContext.SubDialRequests.Add(new ChannelRequest
        {
            SubProtocol = PeerFactoryBuilderBase.CreateProtocolInstance<TProtocol>(_serviceProvider),
            CompletionSource = cts
        });
        return cts.Task;
    }

    protected virtual async Task<IRemotePeer> DialAsync(LocalPeer peer, Multiaddress addr, CancellationToken token)
    {
        try
        {
            token.Register(() => _ = chan.CloseAsync());

            PeerContext context = new()
            {
                Id = $"ctx-{++CtxId}",
                LocalPeer = peer,
            };

            RemotePeer result = new(this, peer, context) { Address = addr };
            context.RemotePeer = result;

            TaskCompletionSource<bool> tcs = new();
            RemotePeerConnected remotePeerConnected = null!;

            remotePeerConnected = remotePeer =>
            {
                ConnectedTo(remotePeer, true).ContinueWith((t) => { tcs.TrySetResult(true); });
                context.OnRemotePeerConnection -= remotePeerConnected;
            };
            context.OnRemotePeerConnection += remotePeerConnected;

            _ = _protocol.DialAsync(chan, _upChannelFactory, context);

            await tcs.Task;
            return result;
        }
        catch
        {
            throw;
        }
    }

    private class PeerListener : IListener
    {
        private readonly Channel _chan;
        private readonly LocalPeer _localPeer;

        public PeerListener(Channel chan, LocalPeer localPeer)
        {
            _chan = chan;
            _localPeer = localPeer;
        }

        public event OnConnection? OnConnection;
        public Multiaddress Address => _localPeer.Address;

        public Task DisconnectAsync()
        {
            return _chan.CloseAsync();
        }

        public TaskAwaiter GetAwaiter()
        {
            return _chan.GetAwaiter();
        }

        internal void RaiseOnConnection(IRemotePeer peer)
        {
            OnConnection?.Invoke(peer);
        }
    }

    public class LocalPeer : ILocalPeer
    {

        public LocalPeer(Identity identity)
        {
            Identity = identity;
        }

        public Identity Identity { get; }
        public ConcurrentDictionary<Multiaddress, PeerAddressAccounting> Addresses { get; } = new();

        public PeerAddressAccounting GetOrAddAddress(Multiaddress address)
        {
            return Addresses.GetOrAdd(address, (_key) => new PeerAddressAccounting());
        }
    }

    

    internal class RemotePeerConnection : IRemotePeerConnection {
        public IRemotePeer RemotePeer { get; }
        private readonly IPeerContext peerContext;
        public Channel Channel { get; } = new();
        public RemotePeerConnection(IRemotePeer remotePeer, IPeerContext peerContext)
        {
            RemotePeer = remotePeer;
            this.peerContext = peerContext;
        }

        public Task DisconnectAsync()
        {
            return Channel.CloseAsync();
        }
    }
}
