﻿namespace Common.Networking.Extensions;

public static class ByteExtensions
{
    public static byte[] TrimByteArray(this byte[] array, int capacity)
    {
        var ba = new byte[capacity];
        Buffer.BlockCopy(array, 0, ba, 0, capacity);
        return ba;
    }
}