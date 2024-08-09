// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using Multiformats.Address;

namespace Nethermind.Libp2p.Core;

// TODO: Try the synchronous approach
public interface IProtocol
{
    /// <summary>
    ///     Id used to during connection establishedment, exchanging information about protocol versions and so on
    /// </summary>
    string Id { get; }  

    /// <summary>
    ///     Actively dials a peer
    /// </summary>
    /// <param name="downChannel">A channel to communicate with a bottom layer protocol</param>
    /// <param name="upChannelFactory">Factory that spawns new channels used to interact with top layer protocols</param>
    /// <param name="context">Holds information about local and remote peers</param>
    /// <returns></returns>
    Task<IRemotePeerConnection> DialAsync(IPeerFactory peerFactory, IChannelFactory? upChannelFactory);

    /// <summary>
    ///     Opens a channel to listen to a remote peer
    /// </summary>
    /// <param name="downChannel">A channel to communicate with a bottom layer protocol</param>
    /// <param name="upChannelFactory">Factory that spawns new channels used to interact with top layer protocols</param>
    /// <param name="context">Holds information about local and remote peers</param>
    /// <returns></returns>
    Task ListenAsync(IChannel downChannel, IChannelFactory? upChannelFactory, IPeerContext context);
}

public interface ITransport {
     Task<IRemotePeerConnection> DialAsync(Multiaddress address);

     Task ListenAsync(Multiaddress localAddress, Action<IChannel, Multiaddress> onConnection, CancellationToken token);
}

public interface IMuxerProtocol {
    Task<IChannelMultiplexer> DialAsync(IRemotePeerChannel channel);

    //Handle an incoming connection
    Task<IChannelMultiplexer> ListenAsync(IRemotePeerChannel channel);
}

public interface ISecurityProtocol {
    Task<IChannel> DialAsync(IRemotePeerConnection connection, IChannel channel);
}

public interface IIdentityProtocol {}