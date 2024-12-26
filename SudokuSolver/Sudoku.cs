using System.Runtime.CompilerServices;

namespace SudokuSolver
{
	internal class Sudoku
	{
		private List<Box> _sudoku;
		private Dictionary<byte, byte[]> _getRowBoxes;
		private Dictionary<byte, byte[]> _getColumnBoxes;

		internal Sudoku(List<Box> sudoku)
		{
			if (sudoku.Count != 9)
				throw new ArgumentException($"Each sudoku must have 9 boxes !!");

			this._sudoku = sudoku;
			_getRowBoxes = InitializeRowBoxes();
			_getColumnBoxes = InitializeColumnBoxes();
		}

		internal Sudoku(List<List<char>> sudoku)
		{
			if (sudoku.Count != 9)
				throw new ArgumentException($"Each sudoku must have 9 boxes !!");

			foreach (List<char> box in sudoku)
			{
				if (box.Count != 9)
					throw new ArgumentException($"Each box must have 9 cells !!");
			}

			this._sudoku = InitSudoku();
			this._getRowBoxes = InitializeRowBoxes();
			this._getColumnBoxes = InitializeColumnBoxes();


			for (byte i = 0; i < 9; i += 1)
			{
				for (byte j = 0; j < 9; j += 1)
				{
					try
					{
						byte value = (byte)sudoku[i][j];

						if ((value >= 48) && (value <= 57))
							this._sudoku[BoxIndex(i, j)].cells[CellIndex(i, j)] = new Cell((byte)(value - 48), CellType.Constant);
						else
							throw new ArgumentException($"'value' is not a number from <1, 9>");

					}
					catch(Exception)
					{
						this._sudoku[BoxIndex(i, j)].cells[CellIndex(i, j)] = new Cell(0, CellType.Variable);
					}
				}
			}

			// Check if input does not contains duplicit values
			Tuple<byte, byte> newCell = GetNextCellPosition(CellType.Constant, previous_i: 0, previous_j: 0);
			CheckInput(sudoku, i: newCell.Item1, j: newCell.Item2);
		}

		internal void Solve()
		{ 
			Tuple<byte, byte> newCell = GetNextCellPosition(CellType.Variable);
			byte i = newCell.Item1;
			byte j = newCell.Item2;

			if ((i == 100) || (j == 100))
			{
				Print();
				return;
			}
				

			Cell cell = this._sudoku[BoxIndex(i, j)].cells[CellIndex(i, j)];
			for (byte value = 1; value <= 9; value += 1)
			{
				cell.value = value;

				if ((this._sudoku[BoxIndex(i, j)].DuplicityCheck() == false) && 
					(DuplicityCheckRow(i) == false) && 
					(DuplicityCheckColumn(j) == false))
				{
					Solve();
				}
			}

			cell.value = 0;
		}

		internal void Print()
		{
			ConsoleColor constantColor = ConsoleColor.Red;
			ConsoleColor variableColor = ConsoleColor.White;
			

			for (byte i = 0; i < 9; i += 1)
			{
				if ((i != 0) && (i % 3 == 0))
				{
					Console.WriteLine($"------+-------+------");
				}

				for (byte j = 0; j < 9; j += 1)
				{
					Cell cell = Cell(i, j);

					if ((j != 0) && (j % 3 == 0))
						Console.Write(" ");

					if (cell.value == 0)
						Console.Write(" ");
					else
					{
						if (cell.type == CellType.Constant)
							Console.ForegroundColor = constantColor;
						else if (cell.type == CellType.Variable)
							Console.ForegroundColor = variableColor;

						Console.Write(cell.value);
						Console.ResetColor();	
					}
					
					if (j < 8)
						Console.Write(" ");

					if ((j != 0) && (j != 8) && ((j + 1) % 3 == 0))
						Console.Write($"|");
				}
				Console.WriteLine();
			}

			Console.WriteLine();
		}

		private List<Box> InitSudoku()
		{
			List<Box> sudoku = new List<Box>();

			for (byte i = 0; i < 9; i += 1)
				sudoku.Add(new Box());

			return sudoku;
		}
		private static Dictionary<byte, byte[]> InitializeRowBoxes()
		{
			Dictionary<byte, byte[]> dict = new Dictionary<byte, byte[]>();
			dict.Add(0, new byte[] { 0, 1, 2 });
			dict.Add(1, new byte[] { 0, 1, 2 });
			dict.Add(2, new byte[] { 0, 1, 2 });
			dict.Add(3, new byte[] { 3, 4, 5 });
			dict.Add(4, new byte[] { 3, 4, 5 });
			dict.Add(5, new byte[] { 3, 4, 5 });
			dict.Add(6, new byte[] { 6, 7, 8 });
			dict.Add(7, new byte[] { 6, 7, 8 });
			dict.Add(8, new byte[] { 6, 7, 8 });

			return dict;
		}
		private static Dictionary<byte, byte[]> InitializeColumnBoxes()
		{
			Dictionary<byte, byte[]> dict = new Dictionary<byte, byte[]>();
			dict.Add(0, new byte[] { 0, 3, 6 });
			dict.Add(1, new byte[] { 0, 3, 6 });
			dict.Add(2, new byte[] { 0, 3, 6 });
			dict.Add(3, new byte[] { 1, 4, 7 });
			dict.Add(4, new byte[] { 1, 4, 7 });
			dict.Add(5, new byte[] { 1, 4, 7 });
			dict.Add(6, new byte[] { 2, 5, 8 });
			dict.Add(7, new byte[] { 2, 5, 8 });
			dict.Add(8, new byte[] { 2, 5, 8 });

			return dict;
		}

		private void CheckInput(List<List<char>> sudoku, byte i = 0, byte j = 0)
		{
			if ((i == 100) || (j == 100))
				return;

			if ((this._sudoku[BoxIndex(i, j)].DuplicityCheck()))
				throw new ArgumentException($"In box #{this._sudoku[BoxIndex(i, j)]} were found duplicit values");

			if (DuplicityCheckRow(i))
				throw new ArgumentException($"In row were found duplicit values");

			if (DuplicityCheckColumn(j))
				throw new ArgumentException($"In column were found duplicit values");

			Tuple<byte, byte> newCell = GetNextCellPosition(CellType.Constant, previous_i: i, previous_j: j);
			CheckInput(sudoku, i: newCell.Item1, j: newCell.Item2);
		}
		private bool DuplicityCheck(List<List<Cell>> cells)
		{
			HashSet<byte> set = new HashSet<byte>();

			// count the occurrences of each number
			for (byte i = 0; i < cells.Count; i += 1)
			{
				for (byte j = 0; j < cells[i].Count; j += 1)
				{
					Cell cell = cells[i][j];

					if (cell.HasValue() && (set.Add(cell.value) == false))
						return true;
				}
			}

			return false;
		}
		private bool DuplicityCheckRow(byte row)
		{
			// get cells of specified boxes
			byte[] boxes = _getRowBoxes[row];
			List<Cell> box1cells = _sudoku[boxes[0]].CellsByRow(row);
			List<Cell> box2cells = _sudoku[boxes[1]].CellsByRow(row);
			List<Cell> box3cells = _sudoku[boxes[2]].CellsByRow(row);

			List<List<Cell>> cells = new List<List<Cell>>() { box1cells, box2cells, box3cells };

			return DuplicityCheck(cells);
		}
		private bool DuplicityCheckColumn(byte column)
		{
			// get cells of specific boxes
			byte[] boxes = _getColumnBoxes[column];
			List<Cell> box1cells = _sudoku[boxes[0]].CellsByColumn(column);
			List<Cell> box2cells = _sudoku[boxes[1]].CellsByColumn(column);
			List<Cell> box3cells = _sudoku[boxes[2]].CellsByColumn(column);

			List<List<Cell>> cells = new List<List<Cell>>() { box1cells, box2cells, box3cells };

			return DuplicityCheck(cells);
		}

		private Cell Cell(byte row, byte column)
		{
			return Cells(row, column)[CellIndex(row, column)];
		}
		private List<Cell> Cells(byte row, byte column)
		{
			return _sudoku[BoxIndex(row, column)].cells;
		}
		private byte BoxIndex(byte row, byte column)
		{
			if (row >= 9)
				throw new ArgumentException($"arg 'row' must be from range <0, 8>");

			if (column >= 9)
				throw new ArgumentException($"arg 'column' must be from range <0, 8>");

			byte[] rows = _getRowBoxes[row];
			byte[] columns = _getColumnBoxes[column];

			byte boxIndex = 10;
			HashSet<byte> set = new HashSet<byte>(rows);

			foreach (byte value in columns)
			{
				if (set.Contains(value))
				{
					boxIndex = value;
					break;
				}
			}

			if (boxIndex == 10)
				throw new IndexOutOfRangeException($"Code should not run into this part !");

			return boxIndex;
		}
		private byte CellIndex(byte row, byte column)
		{
			if (row >= 9)
				throw new ArgumentException($"arg 'row' must be from range <0, 8>");

			if (column >= 9)
				throw new ArgumentException($"arg 'column' must be from range <0, 8>");

			row %= 3;
			column %= 3;

			switch (row)
			{
				case 0:
					return new List<byte> { 0, 1, 2 }[column];
				case 1:
					return new List<byte> { 3, 4, 5 }[column];
				case 2:
					return new List<byte> { 6, 7, 8 }[column];
				default:
					throw new ArgumentException($"Code should not run into this part !");
			}
				
					
		}

		private Tuple<byte, byte> GetNextCellPosition(CellType type, byte previous_i = 100, byte previous_j = 100)
		{
			bool VariableLogic(Cell cell, byte i, byte j)
			{
				if ((cell.type == CellType.Variable) && (cell.HasNotValue()))
					return true;

				return false;
			}

			bool ConstantLogic(Cell cell, byte i, byte j)
			{
				if (cell.type == CellType.Constant)
				{
					if (previous_i < i)
						return true;

					if ((previous_i == i) && (previous_j < j))
						return true;
				}

				return false;
			}


			for (byte i = 0; i < 9; i += 1)
			{
				for (byte j = 0; j < 9; j += 1)
				{
					Cell cell = this._sudoku[BoxIndex(i, j)].cells[CellIndex(i, j)];

					
					switch (type)
					{
						case CellType.Variable:
							if (VariableLogic(cell, i, j))
								return new Tuple<byte, byte>(i, j);

							break;

						case CellType.Constant:
							if (ConstantLogic(cell, i, j))
								return new Tuple<byte, byte>(i, j);

							break;
					}
				}
			}
			
			return new Tuple<byte, byte>(100, 100);
		}

	}
}
