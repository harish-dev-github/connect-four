namespace ConnectFour;

internal static class Win
{
    internal static bool Check(GameState currentState, Grid.Position turnPosition, Player player)
    {
        var positions = currentState.Grid.PlayerPositions;
        var r = turnPosition.Row;
        var c = turnPosition.Column;

        return CheckHorizontal() ||
               CheckVertical() ||
               CheckDiagonalLeft() ||
               CheckDiagonalRight();

        bool CheckHorizontal()
        {
            if (c == 0)
            {
                return positions[r][c + 1].Player == player && positions[r][c + 1].Horizontal == 3;
            }

            if (c == currentState.Grid.Columns - 1)
            {
                return positions[r][c - 1].Player == player && positions[r][c - 1].Horizontal == 3;
            }

            if (positions[r][c - 1].Player == positions[r][c + 1].Player && positions[r][c + 1].Player == player)
            {
                return positions[r][c - 1].Horizontal + positions[r][c + 1].Horizontal >= 3;
            }

            if (positions[r][c - 1].Player == player)
            {
                return positions[r][c - 1].Player == player && positions[r][c - 1].Horizontal == 3;
            }

            return positions[r][c - 1].Horizontal + positions[r][c + 1].Horizontal >= 3;
        }
        
        bool CheckVertical()
        {
            if (r == 0)
            {
                return positions[r + 1][c].Player == player && positions[r + 1][c].Vertical == 3;
            }

            if (r == currentState.Grid.Rows - 1)
            {
                return positions[r - 1][c].Player == player && positions[r - 1][c].Vertical == 3;
            }

            if (positions[r + 1][c].Player == positions[r - 1][c].Player && positions[r + 1][c].Player == player)
            {
                return positions[r + 1][c].Vertical + positions[r + 1][c].Vertical >= 3;
            }

            if (positions[r - 1][c].Player == player)
            {
                return positions[r - 1][c].Player == player && positions[r - 1][c].Vertical == 3;
            }

            return positions[r - 1][c].Vertical + positions[r + 1][c].Vertical >= 3;
        }
        
        bool CheckDiagonalLeft()
        {
            if (r == 0 || c == 0)
            {
                return r + 1 < currentState.Grid.Rows && c + 1 < currentState.Grid.Columns &&
                       positions[r + 1][c + 1].Player == player && positions[r + 1][c + 1].DiagonalLeft == 3;
            }

            if (r == currentState.Grid.Rows - 1 || c == currentState.Grid.Columns - 1)
            {
                return positions[r - 1][c - 1].Player == player && positions[r - 1][c - 1].DiagonalLeft == 3;
            }

            if (positions[r - 1][c - 1].Player == positions[r + 1][c + 1].Player && positions[r - 1][c - 1].Player == player)
            {
                return positions[r - 1][c - 1].DiagonalLeft + positions[r + 1][c + 1].DiagonalLeft >= 3;
            }

            if (positions[r - 1][c - 1].Player == player)
            {
                return positions[r - 1][c - 1].DiagonalLeft == 3;
            }

            return positions[r - 1][c - 1].DiagonalLeft + positions[r + 1][c + 1].DiagonalLeft >= 3;
        }

        bool CheckDiagonalRight()
        {
            if (r == 0 || c == currentState.Grid.Columns - 1)
            {
                return r + 1 < currentState.Grid.Rows && c - 1 >= 0 &&
                       positions[r + 1][c - 1].Player == player && positions[r + 1][c - 1].DiagonalRight == 3;
            }

            if (r == currentState.Grid.Rows - 1 || c == 0)
            {
                return positions[r - 1][c + 1].Player == player && positions[r - 1][c + 1].DiagonalRight == 3;
            }

            if (positions[r - 1][c + 1].Player == positions[r + 1][c - 1].Player && positions[r - 1][c + 1].Player == player)
            {
                return positions[r - 1][c + 1].DiagonalRight + positions[r + 1][c - 1].DiagonalRight >= 3;
            }

            if (positions[r - 1][c + 1].Player == player)
            {
                return positions[r - 1][c + 1].DiagonalRight == 3;
            }

            return positions[r - 1][c + 1].DiagonalRight + positions[r + 1][c - 1].DiagonalRight >= 3;
        }
    }
}