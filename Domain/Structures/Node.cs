namespace Domain.Structures;

public sealed partial record Node(int Index, int X, int Y) {
  public override int GetHashCode() => Index;
}
