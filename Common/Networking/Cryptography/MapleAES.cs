namespace Common.Networking.Cryptography;

public static class MapleAES
{
    private static readonly FastAES sTransformer;

    private static readonly byte[] sUserKey =
    {
        0x13, 0x00, 0x00, 0x00,
        0x08, 0x00, 0x00, 0x00,
        0x06, 0x00, 0x00, 0x00,
        0xb4, 0x00, 0x00, 0x00,
        0x1b, 0x00, 0x00, 0x00,
        0x0f, 0x00, 0x00, 0x00,
        0x33, 0x00, 0x00, 0x00,
        0x52, 0x00, 0x00, 0x00
    };

    static MapleAES()
    {
        sTransformer = new FastAES(sUserKey);
    }

    public static void Transform(byte[] buffer, MapleIV iv)
    {
        int remaining = buffer.Length;
        int length = 0x5B0;
        int start = 0;
        int index;
        byte[] realIV = new byte[16];
        byte[] IVBytes = BitConverter.GetBytes(iv.Value);
        while (remaining > 0)
        {
            for (index = 0; index < realIV.Length; ++index)
                realIV[index] = IVBytes[index % 4];
            if (remaining < length)
                length = remaining;
            for (index = start; index < (start + length); ++index)
            {
                int sub = index - start;
                if ((sub % realIV.Length) == 0)
                    sTransformer.TransformBlock(realIV);
                buffer[index] ^= realIV[sub % realIV.Length];
            }
            start += length;
            remaining -= length;
            length = 0x5B4;
        }
        iv.Shuffle();
    }

    public static unsafe void GetHeader(byte[] data, MapleIV iv, ushort majorVer)
    {
        fixed (byte* pData = data)
        {
            *(ushort*)pData = (ushort)(-(majorVer + 1) ^ iv.HIWORD);
            *((ushort*)pData + 1) = (ushort)(*(ushort*)pData ^ (data.Length - 4));
        }
    }

    public static unsafe int GetLength(byte[] data)
    {
        fixed (byte* pData = data)
            return *(ushort*)pData ^ *((ushort*)pData + 1);
    }
}