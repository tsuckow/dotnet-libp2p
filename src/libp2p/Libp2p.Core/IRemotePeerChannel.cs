// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

namespace Nethermind.Libp2p.Core;

/// <summary>
/// Represents a single communication stream to a remote peer
/// </summary>
public interface IRemotePeerChannel
{
    bool Incoming { get; }
    /// <summary>
    /// Connection this channel is from
    /// </summary>
    IRemotePeerConnection Connection {get;}
    IChannel Channel { get; }
}
