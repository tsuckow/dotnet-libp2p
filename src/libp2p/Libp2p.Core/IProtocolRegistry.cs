interface IProtocolRegistry {
            public IEnumerable<ITransport> Transports { get; }

        public IEnumerable<IMuxerProtocol> MuxerProtocols { get; }

        public IEnumerable<ISecurityProtocol> SecurityProtocols { get; }

        public IEnumerable<IIdentityProtocol> IdentityProtocols { get; }
 
        public IEnumerable<IProtocol> OtherProtocols { get; }
 
}