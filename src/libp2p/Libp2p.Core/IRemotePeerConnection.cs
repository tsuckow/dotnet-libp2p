// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

namespace Nethermind.Libp2p.Core;

public interface IRemotePeerConnection
{
    IRemotePeer RemotePeer {get;}

    IChannelMultiplexer? Mux { get; }
    Task DisconnectAsync();
}
