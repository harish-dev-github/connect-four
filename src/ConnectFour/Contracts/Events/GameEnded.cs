namespace ConnectFour;

public record GameEnded(Player Winner) : IGameEvent;