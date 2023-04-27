namespace Domain.Structures;

public sealed partial record Node(int Index, int X, int Y) {
  public override int GetHashCode() => Index;

  public void Deconstruct(out int x, out int y) {
    x = X;
    y = Y;
  }

  public void Deconstruct(out int index, out int x, out int y) {
    index = Index;
    (x, y) = this;
  }
}
