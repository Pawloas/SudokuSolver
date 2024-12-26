namespace SudokuSolver
{
	internal class Cell
	{
		// 0 - none value filled
		// 1 to 9 - filled value
		public byte value;
		public CellType type { get; }


		internal Cell()
		{
			this.value = 0;
			this.type = CellType.Constant;
		}
		internal Cell(byte value, CellType type)
		{
			this.value = value;
			this.type = type;
		}

		internal bool HasValue()
		{
			return value != 0;
		}
		internal bool HasNotValue()
		{
			return HasValue() == false;
		}
	}
}
