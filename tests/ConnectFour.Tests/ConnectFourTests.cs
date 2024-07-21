using System.Collections.Immutable;
using FluentAssertions;
using static ConnectFour.Tests.Extensions;

namespace ConnectFour.Tests;

public class ConnectFourTests
{
    [Fact]
    public void Game_Initialization_Sets_Grid()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) = ExecuteMultipleCommandsUntilFailure(new InitializeGame(6, 7));
        
        gameState.Id.Should().NotBeNull();
        gameState.Id.Should().NotBe(string.Empty);
        gameState.Grid.Rows.Should().Be(6);
        gameState.Grid.Columns.Should().Be(7);
        gameState.Grid.PlayerPositions.SelectMany(x => x).Should().AllSatisfy(p => p.Should().BeEquivalentTo(
            new Grid.PositionInfo(Player.None, 0, 0, 0, 0)));
        gameState.NextPlayer.Should().Be(Player.One);
        gameState.Status.Should().Be(GameStatus.Initialized);
        gameState.NumberOfTurns.Should().Be(0);

        events.OfType<GameInitialized>().Single().Grid.Should().BeEquivalentTo(gameState.Grid);
    }

    [Fact]
    public void FirstTakeTurnSucceeds()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) =
            Play(6, 7, (Player.One, 0, 1));
        
        events.Should().NotBeEmpty();
        events.Should().ContainSingle(evt => evt is TurnTook);
        gameState.Grid.PlayerPositions[0][1].Should().BeEquivalentTo(
            new Grid.PositionInfo(
                Player.One, 1, 1, 1, 1));
        errors.Should().BeEmpty();
    }

    [Fact]
    public void When_SamePlayer_Takes2ContinuousTurns_Then_InvalidCommand()
    {
        (_, _, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 1),
                (Player.One, 1, 1));
        
        errors.Should().Contain("Invalid_Player");
    }

    [Fact]
    public void When_TurnTookOn_AlreadyTakenPosition_Then_InvalidCommand()
    {
        (_, _, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 1),
                (Player.Two, 0, 1));
        
        errors.Should().Contain("Invalid_Position");
    }
    
    [Fact]
    public void When_WinningMoveExecuted_Horizontally_Then_GameEndedEventAndState()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 0),
                (Player.Two, 3, 1),
                (Player.One, 0, 1),
                (Player.Two, 5, 2),
                (Player.One, 0, 2),
                (Player.Two, 3, 3),
                (Player.One, 0, 3));
        events.Should().ContainSingle(x => x is GameEnded);
        gameState.Status.Should().Be(GameStatus.Ended);
        gameState.Winner.Should().Be(Player.One);
    }

    [Fact]
    public void When_WinningMoveExecuted_Vertically_Then_GameEndedEventAndState()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 0),
                (Player.Two, 3, 1),
                (Player.One, 1, 0),
                (Player.Two, 5, 2),
                (Player.One, 2, 0),
                (Player.Two, 3, 3),
                (Player.One, 3, 0));
        events.Should().ContainSingle(x => x is GameEnded);
        gameState.Status.Should().Be(GameStatus.Ended);
        gameState.Winner.Should().Be(Player.One);
    }
    
    [Fact]
    public void When_WinningMoveExecuted_DiagonallyRight_Then_GameEndedEventAndState()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 0),
                (Player.Two, 3, 1),
                (Player.One, 1, 1),
                (Player.Two, 5, 2),
                (Player.One, 2, 2),
                (Player.Two, 3, 4),
                (Player.One, 3, 3));
        events.Should().ContainSingle(x => x is GameEnded);
        gameState.Status.Should().Be(GameStatus.Ended);
        gameState.Winner.Should().Be(Player.One);
    }
    
    [Fact]
    public void When_WinningMoveExecuted_DiagonallyLeft_Then_GameEndedEventAndState()
    {
        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) =
            Play(6, 7,
                (Player.One, 0, 6),
                (Player.Two, 3, 1),
                (Player.One, 1, 5),
                (Player.Two, 5, 2),
                (Player.One, 2, 4),
                (Player.Two, 3, 4),
                (Player.One, 3, 3));
        events.Should().ContainSingle(x => x is GameEnded);
        gameState.Status.Should().Be(GameStatus.Ended);
        gameState.Winner.Should().Be(Player.One);
    }

    [Fact]
    public void Given_StartedGame_When_GameFilled_Then_GameEndedEventAndStateWithNoWinner()
    {
        //    | X | O | X | O | X | O | X |
        //    | X | O | X | O | X | O | X |
        //    | O | X | O | X | O | X | O |
        //    | O | X | O | X | O | X | O |
        //    | X | O | X | O | X | O | X |
        //    | O | X | O | X | O | X | O |

        
        (Player, int, int)[] turns = [
            (Player.One, 0, 0), (Player.Two, 0, 1), (Player.One, 0, 2), (Player.Two, 0, 3), (Player.One, 0, 4), (Player.Two, 0, 5), (Player.One, 0, 6),
            (Player.Two, 1, 1), (Player.One, 1, 0), (Player.Two, 1, 3), (Player.One, 1, 2), (Player.Two, 1, 5), (Player.One, 1, 4), (Player.Two, 2, 0),
            (Player.One, 1, 6), (Player.Two, 2, 2), (Player.One, 2, 1), (Player.Two, 2, 4), (Player.One, 2, 3), (Player.Two, 2, 6), (Player.One, 2, 5),
            (Player.Two, 3, 0), (Player.One, 3, 1), (Player.Two, 3, 2), (Player.One, 3, 3), (Player.Two, 3, 4), (Player.One, 3, 5), (Player.Two, 3, 6),
            (Player.One, 4, 0), (Player.Two, 4, 1), (Player.One, 4, 2), (Player.Two, 4, 3), (Player.One, 4, 4), (Player.Two, 4, 5), (Player.One, 4, 6),
            (Player.Two, 5, 0), (Player.One, 5, 1), (Player.Two, 5, 2), (Player.One, 5, 3), (Player.Two, 5, 4), (Player.One, 5, 5), (Player.Two, 5, 6),
        ];

        (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) = Play(6, 7, turns);
        gameState.Status.Should().Be(GameStatus.Ended);
        gameState.Winner.Should().Be(Player.None);
        errors.Should().BeEmpty();
        events.Should().ContainSingle(x => x is GameEnded);
    }
}

internal static class Extensions
{
    internal static ImmutableArray<IGameEvent> AsEvents(this IValidated<ImmutableArray<IGameEvent>> commandResult) =>
        commandResult.Match(
            success => success.Value,
            errors => throw new NotImplementedException());
    
    internal static TEvent AsEvent<TEvent>(this IValidated<ImmutableArray<IGameEvent>> commandResult)
        where TEvent : IGameEvent =>
        commandResult.Match(
            success => success.Value.OfType<TEvent>().Single(),
            errors => throw new NotImplementedException());
    
    internal static ImmutableArray<string> AsErrors(this IValidated<ImmutableArray<IGameEvent>> commandResult) =>
        commandResult.Match(
            success => throw new NotImplementedException(),
            errors => errors);

    internal static (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors)
        Play(int rows, int cols, params (Player, int, int)[] positions)
    {
        IGameCommand[] initialize = [new InitializeGame(6, 7)];
        IGameCommand[] takeTurns = positions
            .Select((x, i) =>
                new TakeTurn(x.Item1, new Grid.Position(x.Item2, x.Item3)))
            .ToArray();
        return ExecuteMultipleCommandsUntilFailure(initialize.Concat(takeTurns).ToArray());
    }

    internal static (GameState gameState, ImmutableArray<IGameEvent> events, ImmutableArray<string> errors) ExecuteMultipleCommandsUntilFailure
        (params IGameCommand[] commands)
    {
        GameState gameState = GameState.Zero;
        var errors = ImmutableArray<string>.Empty;
        ImmutableArray<IGameEvent> events = ImmutableArray<IGameEvent>.Empty;
        foreach (var command in commands)
        {
            var commandExecutionResult = Game.ExecuteCommand(gameState, command);
            var shouldBreak = commandExecutionResult.Match(
                success =>
                {
                    events = [..success.Value];
                    gameState = success.Value.Aggregate(gameState, Game.ApplyEvent);
                    return false;
                },
                e =>
                {
                    errors = e;
                    return true;
                });

            if (shouldBreak)
            {
                break;
            }
        }

        return (gameState, events, errors);
    }
}