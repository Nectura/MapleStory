namespace Common.Models.Structs;

public record struct Position
{
    public ushort X { get; set; }
    public ushort Y { get; set; }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}