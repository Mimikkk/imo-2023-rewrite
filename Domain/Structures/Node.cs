namespace Domain.Structures;

public sealed partial record Node(int Index, int X, int Y) {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override int GetHashCode() => Index;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Deconstruct(out int x, out int y) {
    x = X;
    y = Y;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Deconstruct(out int index, out int x, out int y) {
    index = Index;
    (x, y) = this;
  }
}
