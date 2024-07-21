using System.Collections.Immutable;

namespace ConnectFour;

internal static class Game
{
    public static GameState ApplyEvent(GameState currentState, IGameEvent gameEvent) =>
        gameEvent switch
        {
            GameInitialized gameInitialized => currentState with { Grid = gameInitialized.Grid, Status = GameStatus.Initialized },
            TurnTook took => Turn.When(currentState, took),
            GameEnded gameEnded => currentState with { Status = GameStatus.Ended, Winner = gameEnded.Winner },
            _ => currentState
        };

    public static IValidated<ImmutableArray<IGameEvent>> ExecuteCommand(GameState currentState, IGameCommand gameCommand) =>
        gameCommand switch
        {
            InitializeGame initialize => Successful(new GameInitialized(currentState.Id, new Grid(initialize.Rows, initialize.Columns))),
            TakeTurn takeTurn when currentState.Status == GameStatus.Ended => Failure("Game_Already_Ended"),
            TakeTurn takeTurn when currentState.NextPlayer != takeTurn.Player => Failure("Invalid_Player"),
            TakeTurn takeTurn when takeTurn.Position.Row < 0 || takeTurn.Position.Row >= currentState.Grid.Rows => Failure("Invalid_Row"),
            TakeTurn takeTurn when takeTurn.Position.Column < 0 || takeTurn.Position.Column >= currentState.Grid.Columns => Failure("Invalid_Column"),
            TakeTurn takeTurn when currentState.Grid.PlayerPositions[takeTurn.Position.Row][takeTurn.Position.Column].Player != Player.None => Failure("Invalid_Position"),
            // TakeTurn takeTurn when currentState.NumberOfTurns < 6 => Successful(new TurnTook(takeTurn.Player, takeTurn.Position)),
            TakeTurn takeTurn => CheckIfTheGameHasEnded(currentState, takeTurn),
            _ => Failure("Invalid_Command")
        };

    private static IValidated<ImmutableArray<IGameEvent>> CheckIfTheGameHasEnded(GameState currentState, TakeTurn takeTurn)
    {
        var isWin = Win.Check(currentState, takeTurn.Position, takeTurn.Player);
        var gameEnded = (currentState.NumberOfTurns + 1) == currentState.Grid.Size;
        var turnTook = new TurnTook(takeTurn.Player, takeTurn.Position);
        return (isWin, gameEnded) switch
        {
            (true, _) => Successful(turnTook, new GameEnded(takeTurn.Player)),
            (false, true) => Successful(turnTook, new GameEnded(Player.None)),
            _ => Successful(turnTook)
        };
    }

    private static Valid<ImmutableArray<IGameEvent>> Successful(params IGameEvent[] events) => new([..events]);
    private static Invalid<ImmutableArray<IGameEvent>> Failure(params string[] errors) => new([..errors]);
}