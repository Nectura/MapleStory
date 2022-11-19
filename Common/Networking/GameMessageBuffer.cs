using System.Text;
using Common.Networking.Packets.Enums;

namespace Common.Networking;

public sealed class GameMessageBuffer
{
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;
    private readonly BinaryWriter _writer;

    public GameMessageBuffer(byte[] buffer)
    {
        _stream = new MemoryStream(buffer);
        _reader = new BinaryReader(_stream);
        _writer = new BinaryWriter(_stream);
    }

    public GameMessageBuffer(EServerOperationCode opcode, int capacity = 1024)
    {
        _stream = new MemoryStream(capacity);
        _reader = new BinaryReader(_stream);
        _writer = new BinaryWriter(_stream);
        WriteUShort((ushort) opcode);
    }
    
    public byte[] GetBytes()
    {
        return _stream.ToArray();
    }

    public byte ReadByte() => _reader.ReadByte();
    public ushort ReadUShort() => _reader.ReadUInt16();
    public uint ReadUInt() => _reader.ReadUInt32();
    public ulong ReadULong() => _reader.ReadUInt64();
    public sbyte ReadSByte() => _reader.ReadSByte();
    public short ReadShort() => _reader.ReadInt16();
    public int ReadInt() => _reader.ReadInt32();
    public long ReadLong() => _reader.ReadInt64();
    public string ReadString()
    {
        var size = ReadUShort();
        var sb = new StringBuilder(size);
        for (var i = 0; i < size; i++)
            sb.Append((char)ReadByte());
        return sb.ToString();
    }
    public byte[] Read(int count) => _reader.ReadBytes(count);
    public bool CanRead(int size) => _reader.BaseStream.Length - _reader.BaseStream.Position >= size;

    public GameMessageBuffer WriteByte(byte value = 0)
    {
        _writer.Write(value);
        return this;
    }
    
    public GameMessageBuffer WriteBool(bool value = false)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteUShort(ushort value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteUInt(uint value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteULong(ulong value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteSByte(sbyte value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteShort(short value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteInt(int value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteLong(long value = 0)
    {
        _writer.Write(value);
        return this;
    }

    public GameMessageBuffer WriteString(string value)
    {
        WriteUShort((ushort)value.Length);
        foreach (char c in value)
            WriteByte((byte)c);
        return this;
    }

    public bool TryRead(Type conversionType, int? readLength, out object? value)
    {
        value = null;
        
        switch (conversionType)
        {
            case var _ when conversionType == typeof(sbyte) && CanRead(1):
                value = ReadSByte();
                return true;
            case var _ when conversionType == typeof(short) && CanRead(2):
                value = ReadShort();
                return true;
            case var _ when conversionType == typeof(int) && CanRead(4):
                value = ReadInt();
                return true;
            case var _ when conversionType == typeof(long) && CanRead(8):
                value = ReadLong();
                return true;
            
            case var _ when conversionType == typeof(byte) && CanRead(1):
                value = ReadByte();
                return true;
            case var _ when conversionType == typeof(ushort) && CanRead(2):
                value = ReadUShort();
                return true;
            case var _ when conversionType == typeof(uint) && CanRead(4):
                value = ReadUInt();
                return true;
            case var _ when conversionType == typeof(ulong) && CanRead(8):
                value = ReadULong();
                return true;
            
            case var _ when conversionType == typeof(string) && CanRead(2):
                value = ReadString();
                return true;
            
            case var _ when conversionType == typeof(byte[]) && readLength.HasValue && CanRead(readLength.Value):
                value = Read(readLength.Value);
                return true;
        }

        return false;
    }
}