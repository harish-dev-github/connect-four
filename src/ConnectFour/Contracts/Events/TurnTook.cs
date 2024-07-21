namespace ConnectFour;

public record TurnTook(Player Player, Grid.Position Position) : IGameEvent;