namespace Common.Models.Structs;

public record struct Position
{
    public short X { get; init; }
    public short Y { get; init; }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}