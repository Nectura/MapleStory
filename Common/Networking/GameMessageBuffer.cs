using System.Text;

namespace Common.Networking;

public sealed class GameMessageBuffer
{
    private readonly MemoryStream _stream;
    private readonly BinaryReader _reader;
    private readonly BinaryWriter _writer;

    public GameMessageBuffer(byte[] buffer)
    {
        _stream = new(buffer);
        _reader = new(_stream);
        _writer = new(_stream);
    }

    public GameMessageBuffer(int capacity = 1024)
    {
        _stream = new(capacity);
        _reader = new(_stream);
        _writer = new(_stream);
    }
    
    public byte[] GetBytes()
    {
        return _stream.ToArray();
    }

    public byte Read8U() => _reader.ReadByte();
    public ushort Read16U() => _reader.ReadUInt16();
    public uint Read32U() => _reader.ReadUInt32();
    public ulong Read64U() => _reader.ReadUInt64();
    public sbyte Read8() => _reader.ReadSByte();
    public short Read16() => _reader.ReadInt16();
    public int Read32() => _reader.ReadInt32();
    public long Read64() => _reader.ReadInt64();
    public string ReadString()
    {
        ushort size = Read16U();
        StringBuilder sb = new(size);
        for (int i = 0; i < size; i++)
            sb.Append((char)Read8U());
        return sb.ToString();
    }

    public byte[] Read(int count) => _reader.ReadBytes(count);
    public void Write8U(byte value) => _writer.Write(value);
    public void Write16U(ushort value) => _writer.Write(value);
    public void Write32U(uint value) => _writer.Write(value);
    public void Write64U(ulong value) => _writer.Write(value);
    public void Write8(sbyte value) => _writer.Write(value);
    public void Write16(short value) => _writer.Write(value);
    public void Write32(int value) => _writer.Write(value);
    public void Write64(long value) => _writer.Write(value);
    public void WriteString(string value)
    {
        Write16U((ushort)value.Length);
        foreach (char c in value)
            Write8U((byte)c);
    }

    public bool TryRead(Type conversionType, int? readLength, out object? value)
    {
        value = null;
        
        switch (conversionType)
        {
            case var _ when conversionType == typeof(sbyte):
                value = Read8();
                return true;
            case var _ when conversionType == typeof(short):
                value = Read16();
                return true;
            case var _ when conversionType == typeof(int):
                value = Read32();
                return true;
            case var _ when conversionType == typeof(long):
                value = Read64();
                return true;
            
            case var _ when conversionType == typeof(byte):
                value = Read8U();
                return true;
            case var _ when conversionType == typeof(ushort):
                value = Read16U();
                return true;
            case var _ when conversionType == typeof(uint):
                value = Read32U();
                return true;
            case var _ when conversionType == typeof(ulong):
                value = Read64U();
                return true;
            
            case var _ when conversionType == typeof(string):
                value = ReadString();
                return true;
            
            case var _ when conversionType == typeof(byte[]) && readLength.HasValue:
                value = Read(readLength.Value);
                return true;
        }

        return false;
    }
}