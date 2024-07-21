namespace ConnectFour;

internal record GameState(
    string Id,
    Grid Grid,
    GameStatus Status,
    Player NextPlayer,
    int NumberOfTurns,
    Player Winner)
{
    internal static GameState Zero =>
        new(
            new(Guid.NewGuid().ToString()),
            new Grid(0, 0),
            GameStatus.Zero,
            Player.One,
            0,
            Player.None);
}