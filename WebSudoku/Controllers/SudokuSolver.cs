using System;

public class SudokuSolver
{
    public bool Solve(int[,] puzzle)
    {
        return SolveHelper(puzzle, 0, 0);
    }

    private bool SolveHelper(int[,] puzzle, int row, int col)
    {
        if (row == 9)
        {
            row = 0;
            if (++col == 9)
                return true;
        }

        if (puzzle[row, col] != 0)
            return SolveHelper(puzzle, row + 1, col);

        for (int num = 1; num <= 9; num++)
        {
            if (IsValidMove(row, col, num, puzzle))
            {
                puzzle[row, col] = num;
                if (SolveHelper(puzzle, row + 1, col))
                    return true;
                puzzle[row, col] = 0; 
            }
        }

        return false;
    }

    private bool IsValidMove(int row, int col, int num, int[,] puzzle)
    {
        return !UsedInRow(row, num, puzzle) &&
               !UsedInColumn(col, num, puzzle) &&
               !UsedInSubgrid(row - row % 3, col - col % 3, num, puzzle);
    }

    private bool UsedInRow(int row, int num, int[,] puzzle)
    {
        for (int col = 0; col < 9; col++)
        {
            if (puzzle[row, col] == num)
                return true;
        }
        return false;
    }

    private bool UsedInColumn(int col, int num, int[,] puzzle)
    {
        for (int row = 0; row < 9; row++)
        {
            if (puzzle[row, col] == num)
                return true;
        }
        return false;
    }

    private bool UsedInSubgrid(int startRow, int startCol, int num, int[,] puzzle)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (puzzle[row + startRow, col + startCol] == num)
                    return true;
            }
        }
        return false;
    }
}
