namespace ConnectFour;

public record InitializeGame(int Rows, int Columns) : IGameCommand;