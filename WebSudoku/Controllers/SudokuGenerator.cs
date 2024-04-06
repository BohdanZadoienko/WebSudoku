using System;

public class SudokuGenerator
{
    private int[,] grid;
    private readonly Random random;

    public SudokuGenerator()
    {
        grid = new int[9, 9];
        random = new Random();
    }

public int[,] Generate(int cellsToRemove)
{
    GenerateGrid();

    Console.WriteLine("Grid before removing numbers:");
    PrintGrid("Before:");

    RemoveNumbers(cellsToRemove);

    Console.WriteLine("Grid after removing numbers:");
    PrintGrid("After:");

    return grid;
}


    public void PrintGrid(string message)
    {
        Console.WriteLine(message);
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                Console.Write(grid[row, col] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public bool IsValidMove(int row, int col, int num, int[,] puzzle)
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

    private void GenerateGrid()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Shuffle(numbers);
        GenerateGrid(0, 0, numbers);
    }

    private bool GenerateGrid(int row, int col, int[] numbers)
    {
        if (row == 9)
            return true;

        if (col == 9)
            return GenerateGrid(row + 1, 0, numbers);

        foreach (int num in numbers)
        {
            if (IsValidMove(row, col, num, grid))
            {
                grid[row, col] = num;

                if (GenerateGrid(row, col + 1, numbers))
                    return true;

                grid[row, col] = 0; // Backtrack
            }
        }

        return false;
    }

    private void Shuffle(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int j = random.Next(i, array.Length);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

   private void RemoveNumbers(int cellsToRemove)
{
    int cellsRemoved = 0;

    while (cellsRemoved < cellsToRemove)
    {
        int row = random.Next(9);
        int col = random.Next(9);

        if (grid[row, col] != 0)
        {
            grid[row, col] = 0;
            cellsRemoved++;
        }
    }
}
}

