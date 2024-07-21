namespace ConnectFour;

internal static class Turn
{
    internal static GameState When(GameState currentState, TurnTook turnTook)
    {
        var positions = currentState.Grid.PlayerPositions;
        var r = turnTook.Position.Row;
        var c = turnTook.Position.Column;

        int verticalCount = 1;
        if (r > 0 && turnTook.Player == positions[r - 1][c].Player)
        {
            verticalCount = positions[r - 1][c].Vertical + 1;
        }

        int horizontalCount = 1;
        if (c > 0 && turnTook.Player == positions[r][c - 1].Player)
        {
            horizontalCount = positions[r][c - 1].Horizontal + 1;
        }

        int diagonalLeftCount = 1;
        if (r > 0 && c > 0 && turnTook.Player == positions[r - 1][c - 1].Player)
        {
            diagonalLeftCount = positions[r - 1][c - 1].DiagonalLeft + 1;
        }

        int diagonalRightCount = 1;
        if (r > 0 && c < currentState.Grid.Columns - 1 && turnTook.Player == positions[r - 1][c + 1].Player)
        {
            diagonalRightCount = positions[r - 1][c + 1].DiagonalRight + 1;
        }

        var newPositionInfo = new Grid.PositionInfo(turnTook.Player, verticalCount, horizontalCount, diagonalLeftCount, diagonalRightCount);

        var updatedRow = currentState.Grid.PlayerPositions[r].SetItem(c, newPositionInfo);

        return currentState with
        {
            NextPlayer = turnTook.Player == Player.One ? Player.Two : Player.One,
            NumberOfTurns = currentState.NumberOfTurns + 1,
            Grid = currentState.Grid with
            {
                PlayerPositions = currentState.Grid.PlayerPositions.SetItem(r, updatedRow)
            }
        };
    }
}