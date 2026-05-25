namespace PipsSolver.Model;

class Domino(int left, int right)
{
  public int Left { get; private set; } = left;
  public int Right { get; private set; } = right;

  public DominoOrientation Orientation { get; private set; } = DominoOrientation.Right;
  public bool Flipped { get; private set; } = false;
  public (Cell Anchor, Cell Neighbor)? Cells { get; set; } = null;

  public void Flip()
  {
    Flipped = !Flipped;
    (Left, Right) = (Right, Left);
  }

  public void Rotate()
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

  public void Reset()
  {
    if (Flipped) Flip();

    Orientation = DominoOrientation.Right;
  }

  public bool Placed()
  {
    return Cells is { };
  }

  public override string ToString()
  {
    return $"Domino({Left},{Right},{Orientation})";
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
