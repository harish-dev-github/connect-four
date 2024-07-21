using System.Collections.Immutable;

namespace ConnectFour;

public record Grid
{
    public Grid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Size = rows * columns;
        PlayerPositions = [
            ..Enumerable
                .Range(1, rows)
                .Select(_ =>
                    Enumerable
                        .Range(1, columns)
                        .Select(__ => new PositionInfo(Player.None, 0, 0, 0, 0))
                        .ToImmutableArray())
        ];
    }
    
    public int Size { get; }
    public int Rows { get; }
    public int Columns { get; }
    public ImmutableArray<ImmutableArray<PositionInfo>> PlayerPositions { get; init; }

    public readonly record struct Position(int Row, int Column);

    public readonly record struct PositionInfo(
        Player Player,
        int Vertical,
        int Horizontal,
        int DiagonalLeft,
        int DiagonalRight);
}