namespace ConnectFour;

public record GameInitialized(GameId Id, Grid Grid) : IGameEvent;