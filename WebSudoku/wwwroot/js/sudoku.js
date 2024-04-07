var gridData; // Declare gridData as a global variable
var mistakeCount = 0; // Initialize mistake counter
var startTime; // Variable to hold start time
var timerInterval; // Interval variable for the timer

$(document).ready(function () {
  // Handle click event for the "Easy" button
  $("#easyButton").click(function () {
    fetchGeneratedSudoku("easy");
    updateMistakeCounter();
  });

  // Handle click event for the "Medium" button
  $("#mediumButton").click(function () {
    fetchGeneratedSudoku("medium");
    updateMistakeCounter();
  });

  // Handle click event for the "Difficult" button
  $("#difficultButton").click(function () {
    fetchGeneratedSudoku("difficult");
    updateMistakeCounter();
  });
  // Solve Solution Button
  $("#solveButton").click(function () {
    solveSudoku();
    clearInterval(timerInterval);
  });

  // Attach event listener to all sudoku input fields
  $(document).on("input", ".sudoku-input", function () {
    validateInput($(this));
  });

  // Add focus and blur event handlers to input fields
  $(document).on("focus", ".sudoku-input", function () {
    $(this).addClass("active-input");
    highlightRowAndColumn($(this).data("row"), $(this).data("col"));
  });

  $(document).on("blur", ".sudoku-input", function () {
    $(this).removeClass("active-input");
    unhighlightRowAndColumn($(this).data("row"), $(this).data("col"));
  });
});

function updateMistakeCounter() {
  mistakeCount = 0;
  $("#mistakes").text("Die Fehler: " + mistakeCount);
}

// Function to fetch the generated Sudoku puzzle from the server
function fetchGeneratedSudoku(difficulty) {
  $.ajax({
    url: "/Home/GenerateSudoku?difficulty=" + difficulty,
    type: "GET",
    dataType: "json",
    success: function (data) {
      console.log("Generated Sudoku:", data);
      gridData = data; // Assign fetched Sudoku data to gridData
      displaySudoku(gridData);
      startTime = new Date();
      startTimer();
    },
    error: function (xhr, status, error) {
      console.error("Error fetching Sudoku puzzle:", error);
    },
  });
}

// Function to display the Sudoku puzzle in the table
function displaySudoku(gridData) {
  var table = $("#generatedSudokuTable");
  table.empty(); // Clear previous puzzle
  for (var i = 0; i < 9; i++) {
    var row = $("<tr>");
    for (var j = 0; j < 9; j++) {
      var cellValue = gridData[i][j];
      var cell = $("<td>");
      if (cellValue !== 0) {
        cell.text(cellValue);
      } else {
        var input = $("<input>", {
          type: "text",
          class: "sudoku-input",
          "data-row": i,
          "data-col": j,
          maxlength: 1,
        });
        cell.append(input);
      }
      // Add classes for sub-grids
      if ((Math.floor(i / 3) + Math.floor(j / 3)) % 2 === 0) {
        cell.addClass("sub-grid-light");
      } else {
        cell.addClass("sub-grid-dark");
      }
      row.append(cell);
    }
    table.append(row);
  }
}

// Function to validate user input
function validateInput(input) {
  var inputValue = input.val();
  input.val(inputValue.replace(/[^1-9]/g, "")); // Only allow digits 1-9

  var row = input.data("row");
  var col = input.data("col");

  // Update gridData with the new input value
  gridData[row][col] = parseInt(inputValue);

  if (inputValue === "") {
    input.removeClass("incorrect-input");
    return;
  }

  if (!isMoveValid(row, col, parseInt(inputValue))) {
    input.addClass("incorrect-input");
    mistakeCount++; // Increment mistake counter
    $("#mistakes").text("Die Fehler: " + mistakeCount); // Update mistake count display
  } else {
    input.removeClass("incorrect-input");
  }

  // Check if all inputs are correct
  if (checkAllInputsCorrect()) {
    clearInterval(timerInterval); // Stop the timer
    showWinNotification();
  }
}

// Function to check if all inputs are correct
function checkAllInputsCorrect() {
  var inputs = $(".sudoku-input");
  for (var i = 0; i < inputs.length; i++) {
    if ($(inputs[i]).hasClass("incorrect-input") || $(inputs[i]).val() === "") {
      return false;
    }
  }
  return true;
}

// Function to show win notification
function showWinNotification() {
  alert(
    `Herzlichen Glückwunsch! Sie haben das Sudoku-Rätsel gelöst!\nSie haben ${
      mistakeCount === 0 ? "keine" : mistakeCount
    } Fehler gemacht.`
  );
}

// Function to check if the user input is valid
function isMoveValid(row, col, num) {
  var inputValue = num;

  // Check if the input conflicts with the same row
  for (var i = 0; i < 9; i++) {
    if (i !== col && gridData[row][i] === inputValue) {
      return false;
    }
  }

  // Check if the input conflicts with the same column
  for (var i = 0; i < 9; i++) {
    if (i !== row && gridData[i][col] === inputValue) {
      return false;
    }
  }

  // Check if the input conflicts with the same 3x3 subgrid
  var startRow = Math.floor(row / 3) * 3;
  var startCol = Math.floor(col / 3) * 3;
  for (var i = startRow; i < startRow + 3; i++) {
    for (var j = startCol; j < startCol + 3; j++) {
      if (i !== row && j !== col && gridData[i][j] === inputValue) {
        return false;
      }
    }
  }

  return true; // If no conflicts found
}

// Function to solve the Sudoku puzzle
function solveSudoku() {
  if (gridData) {
    $.ajax({
      url: "/Home/SolveSudoku",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(gridData),
      success: function (data) {
        console.log("Solved Sudoku:", data);
        gridData = data; // Update gridData with solved puzzle
        displaySudoku(gridData);
      },
      error: function (xhr, status, error) {
        console.error("Error solving Sudoku puzzle:", error);
      },
    });
  } else {
    alert("Du hast das Spiel nicht begonnen!");
  }
}

// Function to start the timer
function startTimer() {
  timerInterval = setInterval(updateTimer, 1000);
}

// Function to update the timer display
function updateTimer() {
  var currentTime = new Date();
  var elapsedTime = currentTime - startTime;
  var minutes = Math.floor(elapsedTime / (1000 * 60));
  var seconds = Math.floor((elapsedTime / 1000) % 60);
  $("#timer").text("Zeit: " + formatTime(minutes) + ":" + formatTime(seconds));
}

// Function to format time to display as mm:ss
function formatTime(time) {
  return time < 10 ? "0" + time : time;
}

// Function to highlight the row and column of the current cell
function highlightRowAndColumn(row, col) {
  $(".active-row").removeClass("active-row");
  $(".active-column").removeClass("active-column");
  $("tr:eq(" + row + ")").addClass("active-row");
  $("td:nth-child(" + (col + 1) + ")").addClass("active-column");
}

// Function to remove highlight from the row and column
function unhighlightRowAndColumn() {
  $(".active-row").removeClass("active-row");
  $(".active-column").removeClass("active-column");
}
