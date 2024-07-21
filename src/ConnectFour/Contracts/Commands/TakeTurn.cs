namespace ConnectFour;

public record TakeTurn(Player Player, Grid.Position Position) : IGameCommand;