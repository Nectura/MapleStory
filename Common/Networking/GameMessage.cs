using Common.Networking.OperationCodes;
using System.Collections;
using static Common.Networking.GameMessageType;

namespace Common.Networking
{
    public sealed class GameMessage : IEnumerable<Tuple<GameMessageType, object>>
    {
        private readonly List<Tuple<GameMessageType, object>> _data;

        public GameMessage(EServerOperationCode op) => _data = new() { new(u16, op) };

        public void Add(GameMessageType type, object value) => _data.Add(new(type, value));

        public IEnumerator<Tuple<GameMessageType, object>> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public GameMessageBuffer GetMessageBuffer()
        {
            GameMessageBuffer buffer = new();
            foreach (Tuple<GameMessageType, object> item in _data)
                switch (item.Item1)
                {
                    case i8: buffer.Write8((sbyte)item.Item2); break;
                    case i16: buffer.Write16((short)item.Item2); break;
                    case i32: buffer.Write32((int)item.Item2); break;
                    case i64: buffer.Write64((long)item.Item2); break;
                    case u8: buffer.Write8U((byte)item.Item2); break;
                    case u16: buffer.Write16U((ushort)item.Item2); break;
                    case u32: buffer.Write32U((uint)item.Item2); break;
                    case u64: buffer.Write64U((ulong)item.Item2); break;
                    case str: buffer.WriteString((string)item.Item2); break;
                }
            return buffer;
        }
    }
}
