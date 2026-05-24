namespace PipsSolver.Model;

class Domino(int left, int right)
{
  public int Left { get; } = left;
  public int Right { get; } = right;

  public DominoOrientation Orientation { get; private set; } = DominoOrientation.Right;
  public bool Flipped { get; private set; } = false;
  public (Cell, Cell)? Cells { get; set; } = null;

  void Flip()
  {
    Flipped = !Flipped;
  }

  void Rotate()
  {
    Orientation = Orientation switch
    {
      DominoOrientation.Right => DominoOrientation.Down,
      DominoOrientation.Down => DominoOrientation.Left,
      DominoOrientation.Left => DominoOrientation.Up,
      DominoOrientation.Up => DominoOrientation.Right,
      _ => throw new Exception("Unknown Domino Orientation")
    };
  }

  void Reset()
  {
    Flipped = false;
    Orientation = DominoOrientation.Right;
  }
};

enum DominoSide
{
  Left,
  Right,
}

enum DominoOrientation
{
  Right,
  Down,
  Left,
  Up,
}
