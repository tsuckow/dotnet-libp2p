
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace Nethermind.Libp2p.Core;

public class DIProtocolRegistry(IServiceProvider _serviceProvider) : IProtocolRegistry
{
    public IEnumerable<ITransport> Transports { get => _serviceProvider.GetRequiredService<IEnumerable<ITransport>>(); }
    public IEnumerable<IMuxerProtocol> MuxerProtocols { get=> _serviceProvider.GetRequiredService<IEnumerable<IMuxerProtocol>>(); }
    public IEnumerable<ISecurityProtocol> SecurityProtocols { get=> _serviceProvider.GetRequiredService<IEnumerable<ISecurityProtocol>>(); }
    public IEnumerable<IIdentityProtocol> IdentityProtocols { get=> _serviceProvider.GetRequiredService<IEnumerable<IIdentityProtocol>>(); }
    public IEnumerable<IProtocol> OtherProtocols { get=> _serviceProvider.GetRequiredService<IEnumerable<IProtocol>>(); }
}