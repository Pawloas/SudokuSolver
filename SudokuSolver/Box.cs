namespace SudokuSolver
{
	internal class Box
	{
		internal List<Cell> cells;


		internal Box()
		{
			List<Cell> cells = new List<Cell>();

			for (byte i = 0; i < 9; i += 1)
				cells.Add(new Cell());

			this.cells = cells;
		}

		internal bool DuplicityCheck()
		{
			HashSet<byte> set = new HashSet<byte>();

			for (byte i = 0; i < cells.Count; i += 1)
			{
				Cell cell = cells[i];

				if (cell.HasNotValue())
					continue;

				if (set.Add(cell.value) == false)
					return true;
			}

			return false;
		}

		internal List<Cell> CellsByRow(byte row)
		{
			if (row >= 9)
				throw new ArgumentException($"arg 'row' must be from range <0, 8>");

			row %= 3;

			switch (row)
			{
				case 0:
					return new List<Cell> { cells[0], cells[1], cells[2] };
				case 1:
					return new List<Cell> { cells[3], cells[4], cells[5] };
				case 2:
					return new List<Cell> { cells[6], cells[7], cells[8] };
				default:
					throw new ArgumentException($"Code should not run into this part !");
			}
		}

		internal List<Cell> CellsByColumn(byte column)
		{
			if (column >= 9)
				throw new ArgumentException($"arg 'column' must be from range <0, 8>");

			column %= 3;

			switch (column)
			{
				case 0:
					return new List<Cell> { cells[0], cells[3], cells[6] };
				case 1:
					return new List<Cell> { cells[1], cells[4], cells[7] };
				case 2:
					return new List<Cell> { cells[2], cells[5], cells[8] };
				default:
					throw new ArgumentException($"Code should not run into this part !");
			}
		}
	}
}
