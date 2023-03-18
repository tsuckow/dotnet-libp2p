// SPDX-FileCopyrightText: 2023 Demerzel Solutions Limited
// SPDX-License-Identifier: MIT

using System.Text;

namespace Nethermind.Libp2p.Core;

public interface IWriter
{
    ValueTask WriteLineAsync(string str, bool prependedWithSize = true)
    {
        int len = Encoding.UTF8.GetByteCount(str) + 1;
        byte[] buf = new byte[VarInt.GetSizeInBytes(len) + len];
        int offset = 0;
        VarInt.Encode(len, buf, ref offset);
        Encoding.UTF8.GetBytes(str, 0, str.Length, buf, offset);
        buf[^1] = 0x0a;
        return WriteAsync(buf);
    }

    ValueTask WriteVarintAsync(int val)
    {
        byte[] buf = new byte[VarInt.GetSizeInBytes(val)];
        int offset = 0;
        VarInt.Encode(val, buf, ref offset);
        return WriteAsync(buf);
    }

    ValueTask WriteSizeAndDataAsync(byte[] data)
    {
        byte[] buf = new byte[VarInt.GetSizeInBytes(data.Length) + data.Length];
        int offset = 0;
        VarInt.Encode(data.Length, buf, ref offset);
        Array.ConstrainedCopy(data, 0, buf, offset, data.Length);
        return WriteAsync(buf);
    }

    ValueTask WriteAsync(ArraySegment<byte> bytes);
}
