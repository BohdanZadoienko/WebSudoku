using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private SudokuGenerator generator;
    private SudokuSolver solver;

    public HomeController()
    {
        generator = new SudokuGenerator();
        solver = new SudokuSolver();
    }

    // GET: Home
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GenerateSudoku(string difficulty)
    {
        try
        {
            // Check if difficulty is provided and valid
            if (string.IsNullOrEmpty(difficulty) ||
                (difficulty != "easy" && difficulty != "medium" && difficulty != "difficult"))
            {
                return BadRequest("Invalid difficulty level.");
            }

            // Generate Sudoku puzzle based on difficulty
            int[,] puzzle;
            switch (difficulty)
            {
                case "easy":
                    puzzle = generator.Generate(15); // Adjust cell removal range for easy difficulty
                    break;
                case "medium":
                    puzzle = generator.Generate(35); // Adjust cell removal range for medium difficulty
                    break;
                case "difficult":
                    puzzle = generator.Generate(50); // Adjust cell removal range for difficult difficulty
                    break;
                default:
                    return BadRequest("Invalid difficulty level.");
            }

            // Convert puzzle to JSON and return
            int[][] puzzleArray = ConvertToJaggedArray(puzzle);
            return Json(puzzleArray);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while generating Sudoku puzzle: {ex.Message}");
        }
    }


    // Endpoint to solve Sudoku puzzle
    [HttpPost]
    public IActionResult SolveSudoku([FromBody] int[][] puzzle)
    {
        if (puzzle == null)
        {
            return BadRequest("Invalid Sudoku puzzle data.");
        }

        int[,] puzzleMatrix = ConvertTo2DArray(puzzle);
        if (solver.Solve(puzzleMatrix))
        {
            int[][] solvedPuzzle = ConvertToJaggedArray(puzzleMatrix);
            return Json(solvedPuzzle);
        }
        else
        {
            return BadRequest("Unable to solve the Sudoku puzzle.");
        }
    }


    // Helper method to convert a 2D array to a jagged array
    private int[][] ConvertToJaggedArray(int[,] array)
    {
        int[][] jaggedArray = new int[array.GetLength(0)][];
        for (int i = 0; i < array.GetLength(0); i++)
        {
            jaggedArray[i] = new int[array.GetLength(1)];
            for (int j = 0; j < array.GetLength(1); j++)
            {
                jaggedArray[i][j] = array[i, j];
            }
        }
        return jaggedArray;
    }

    // Helper method to convert a jagged array to a 2D array
    private int[,] ConvertTo2DArray(int[][] jaggedArray)
    {
        int[,] array = new int[jaggedArray.Length, jaggedArray[0].Length];
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                array[i, j] = jaggedArray[i][j];
            }
        }
        return array;
    }
}
