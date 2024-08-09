using System.Buffers;
using System.Runtime.CompilerServices;

namespace Nethermind.Libp2p.Core;

public class StreamChannel(Stream stream) : IChannel
{
    public async ValueTask<ReadResult> ReadAsync(int length, ReadBlockingMode blockingMode = ReadBlockingMode.WaitAll,
        CancellationToken token = default)
    {
        if (length == 0)
        {
            return ReadResult.Empty;
        }
        var buffer = new byte[length];

        try
        {
            var readlength = await stream.ReadAsync(buffer, token);


            if (readlength == 0)
            {
                return ReadResult.Ended;
            }
            ReadOnlySequence<byte> bytes = new(buffer, 0, readlength);
            return ReadResult.Ok(bytes);
        }
        catch (TaskCanceledException)
        {
            return ReadResult.Cancelled;
        }
    }

    public async ValueTask<IOResult> WriteAsync(ReadOnlySequence<byte> bytes, CancellationToken token = default)
    {
        try
        {
            await stream.WriteAsync(bytes.ToArray(), token);
        }
        catch (TaskCanceledException)
        {
            return IOResult.Cancelled;
        }
        catch (ObjectDisposedException)
        {
            return IOResult.Ended;
        }
        return IOResult.Ok;
    }

    public ValueTask<IOResult> WriteEofAsync(CancellationToken token = default) => Writer.WriteEofAsync(token);

    public TaskAwaiter GetAwaiter() => Completion.Task.GetAwaiter();

    public ValueTask CloseAsync()
    {
        return stream.DisposeAsync();
    }
}