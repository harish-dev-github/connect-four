namespace ConnectFour;

public record GameInitialized(string Id, Grid Grid) : IGameEvent;