
using System.Security.Cryptography.X509Certificates;

namespace Nethermind.Libp2p.Core;

public class DIProtocolRegistry : IProtocolRegistry
{
    public IEnumerable<ITransport> Transports { get; init; }
    public IEnumerable<IMuxerProtocol> MuxerProtocols { get; init; }
    public IEnumerable<ISecurityProtocol> SecurityProtocols { get; init; }
    public IEnumerable<IIdentityProtocol> IdentityProtocols { get; init; }
    public IEnumerable<IProtocol> OtherProtocols { get; init; }

    public DIProtocolRegistry(IEnumerable<ITransport> transports,
                              IEnumerable<IMuxerProtocol> muxerProtocols,
                              IEnumerable<ISecurityProtocol> securityProtocols,
                              IEnumerable<IIdentityProtocol> identityProtocols,
                              IEnumerable<IProtocol> otherProtocols)
    {
        Transports = transports;
        MuxerProtocols = muxerProtocols;
        SecurityProtocols = securityProtocols;
        IdentityProtocols = identityProtocols;
        OtherProtocols = otherProtocols;

    }
}