using Common.Networking.Extensions;
using Common.Networking.OperationCodes;

namespace Common.Networking;

using System;
using System.IO;
using System.Net;

public sealed class ByteBuffer : IDisposable
{
    private const long FT_UT_OFFSET = 116444592000000000L; // EDT
    private const long DEFAULT_TIME = 150842304000000000L; //00 80 05 BB 46 E6 17 02
    private const long ZERO_TIME = 94354848000000000L; //00 40 E0 FD 3B 37 4F 01
    private const long PERMANENT = 150841440000000000L; // 00 C0 9B 90 7D E5 17 02

    private int _position;
    private MemoryStream Stream { get; set; }
    private BinaryWriter Writer { get; set; }
    private BinaryReader Reader { get; set; }

    public byte[] Array { get; private set; }
    public int Offset { get; private set; }
    public int Capacity { get; private set; }
    public int Limit { get; set; }
    public bool HasFlipped { get; private set; }

    public int Position
    {
        get => _position;
        set
        {
            _position = value;
            Stream.Position = Position + Offset;
        }
    }

    public int Remaining => Limit - Position;

    public ByteBuffer(EServerOperationCode serverOpcode, int capacity = 128)
    {
        Capacity = capacity;
        Array = new byte[Capacity];

        Stream = new MemoryStream(Array);
        Writer = new BinaryWriter(Stream);
        Reader = new BinaryReader(Stream);

        Limit = Capacity;
        Offset = 0;
        Position = 0;

        WriteShort((short)serverOpcode);
    }

    public ByteBuffer(byte[] data)
    {
        Capacity = data.Length;
        Array = data;

        Stream = new MemoryStream(Array);
        Writer = new BinaryWriter(Stream);
        Reader = new BinaryReader(Stream);

        Limit = Capacity;
        Offset = 0;
        Position = 0;
    }

    private ByteBuffer(byte[] array, int offset, int capacity)
    {
        Array = array;

        Stream = new MemoryStream(Array);
        Writer = new BinaryWriter(Stream);
        Reader = new BinaryReader(Stream);

        Offset = offset;
        Capacity = capacity;
        Limit = Capacity;
        Position = 0;
    }

    public byte this[int index]
    {
        get => Array[index];
        set => Array[index] = value;
    }

    public byte[] GetContent()
    {
        var ba = new byte[Remaining];
        Buffer.BlockCopy(Array, Position + Offset, ba, 0, Remaining);
        return ba;
    }

    public byte[] GetTrimmedPacket() => Array.TrimByteArray(Position);

    public ByteBuffer Skip(int count)
    {
        Position += count;

        return this;
    }

    public void Flip()
    {
        Limit = Position;
        Position = 0;
        HasFlipped = true;
    }

    public void SafeFlip()
    {
        if (!HasFlipped)
        {
            Flip();
        }
    }

    public ByteBuffer Slice()
    {
        return new ByteBuffer(Array, Position, Remaining);
    }

    public void Dispose()
    {
        Reader.Dispose();
        Writer.Dispose();
        Stream.Dispose();
    }

    public ByteBuffer WriteBytes(params byte[] collection)
    {
        Writer.Write(collection);
        Position += collection.Length;

        return this;
    }

    public ByteBuffer WriteByte(byte item = 0)
    {
        Writer.Write(item);
        Position += sizeof(byte);

        return this;
    }

    public ByteBuffer WriteSByte(sbyte item = 0)
    {
        Writer.Write(item);
        Position += sizeof(sbyte);

        return this;
    }

    public ByteBuffer WriteShort(short item = 0)
    {
        Writer.Write(item);
        Position += sizeof(short);

        return this;
    }

    public ByteBuffer WriteUShort(ushort item = 0)
    {
        Writer.Write(item);
        Position += sizeof(ushort);

        return this;
    }

    public ByteBuffer WriteInt(int item = 0)
    {
        Writer.Write(item);
        Position += sizeof(int);

        return this;
    }

    public ByteBuffer WriteUInt(uint item = 0)
    {
        Writer.Write(item);
        Position += sizeof(uint);

        return this;
    }

    public ByteBuffer WriteLong(long item = 0)
    {
        Writer.Write(item);
        Position += sizeof(long);

        return this;
    }

    public ByteBuffer WriteFloat(float item = 0)
    {
        Writer.Write(item);
        Position += sizeof(float);

        return this;
    }

    public ByteBuffer WriteBool(bool item)
    {
        Writer.Write(item);
        Position += sizeof(bool);

        return this;
    }

    public ByteBuffer WriteInvertedBool(bool item)
    {
        Writer.Write(!item);
        Position += sizeof(bool);

        return this;
    }

    public ByteBuffer WriteString(string item, params object[] args)
    {
        item = item ?? string.Empty;

        if (item != null)
        {
            item = string.Format(item, args);
        }

        Writer.Write((short)item.Length);

        foreach (var c in item)
        {
            Writer.Write(c);
        }

        Position += item.Length + sizeof(short);

        return this;
    }

    public ByteBuffer WriteStringFixed(string item, int length)
    {
        foreach (var c in item)
        {
            Writer.Write(c);
        }

        for (var i = item.Length; i < length; i++)
        {
            Writer.Write((byte)0);
        }

        Position += length;

        return this;
    }

    public ByteBuffer WriteDateTime(DateTime item)
    {
        Writer.Write((long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .TotalMilliseconds);
        Position += sizeof(long);

        return this;
    }

    public ByteBuffer WriteTimeConstant(long realTimestamp)
    {
        switch (realTimestamp)
        {
            case -1:
                Writer.Write(DEFAULT_TIME);
                break;
            case -2:
                Writer.Write(ZERO_TIME);
                break;
            case -3:
                Writer.Write(PERMANENT);
                break;
            default:
                Writer.Write(realTimestamp * 10000 + FT_UT_OFFSET);
                break;
        }

        Position += sizeof(long);

        return this;
    }

    public ByteBuffer WriteKoreanDateTime(DateTime item)
    {
        Writer.Write(
            (long)(item.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds *
            10000 + 116444592000000000L);
        Position += sizeof(long);

        return this;
    }

    public ByteBuffer WriteIPAddress(IPAddress value)
    {
        Writer.Write(value.GetAddressBytes());
        Position += 4;

        return this;
    }

    public byte[] ReadBytes(int count)
    {
        var result = Reader.ReadBytes(count);
        Position += count;
        return result;
    }

    public byte[] ReadBytes()
    {
        return ReadBytes(Remaining);
    }

    public byte ReadByte()
    {
        var result = Reader.ReadByte();
        Position += sizeof(byte);
        return result;
    }

    public sbyte ReadSByte()
    {
        var result = Reader.ReadSByte();
        Position += sizeof(sbyte);
        return result;
    }

    public short ReadShort()
    {
        var result = Reader.ReadInt16();
        Position += sizeof(short);
        return result;
    }

    public ushort ReadUShort()
    {
        var result = Reader.ReadUInt16();
        Position += sizeof(ushort);
        return result;
    }

    public int ReadInt()
    {
        //var count = Reader.BaseStream.Length / sizeof(int);

        //for (var i = 0; i < count; i++)
        //{
        //    int v = this.Reader.ReadInt32();
        //}

        var result = Reader.ReadInt32();
        Position += sizeof(int);
        return result;
    }

    public uint ReadUInt()
    {
        var result = Reader.ReadUInt32();
        Position += sizeof(uint);
        return result;
    }

    public long ReadLong()
    {
        var result = Reader.ReadInt64();
        Position += sizeof(long);
        return result;
    }

    public float ReadFloat()
    {
        var result = Reader.ReadSingle();
        Position += sizeof(float);
        return result;
    }

    public bool ReadBool()
    {
        var result = Reader.ReadBoolean();
        Position += sizeof(bool);
        return result;
    }

    public string ReadString()
    {
        var count = Reader.ReadInt16();

        var result = new char[count];

        for (var i = 0; i < count; i++)
        {
            result[i] = (char)Reader.ReadByte();
        }

        Position += count + sizeof(short);

        return new string(result);
    }

    public IPAddress ReadIPAddress()
    {
        var result = new IPAddress(Reader.ReadBytes(4));
        Position += 4;
        return result;
    }
}