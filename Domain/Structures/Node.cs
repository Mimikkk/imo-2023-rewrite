namespace Domain.Structures;

public readonly partial record struct Node(int Index, int X, int Y) {
  public bool Equals(Node other) => other.Index == Index;
  public override int GetHashCode() => Index;
}
