// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using Multiformats.Address;
using System.Collections.Concurrent;

namespace Nethermind.Libp2p.Core;

/// <summary>
///  Extra information about an address for a peer
/// </summary>
public class PeerAddressAccounting
{
    int failedDials;
    int FailedDials { get => failedDials; }
    internal void RecordFailedDial() {
        Interlocked.Add(ref failedDials, 1);
    }
}

public interface IPeer
{
    Identity Identity { get; }

    public ConcurrentDictionary<Multiaddress, PeerAddressAccounting> Addresses { get; }
    public PeerAddressAccounting GetOrAddAddress(Multiaddress address);
}
