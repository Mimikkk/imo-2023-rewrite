namespace Domain.Structures.NodeLists;

public sealed partial class NodeList {
  public NodeList(int capacity, Action<IList<Node>>? action = null) {
    _capacity = capacity;
    _list = new List<Node>(_capacity);
    _used = new bool[_capacity];
    if (action is not null) Changed += (_, _) => action.Invoke(this.ToArray());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static NodeList Create(int capacity, Action<IList<Node>> action) => new(capacity, action);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static NodeList Create(int capacity) => new(capacity);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int NextBy(int index, int offset = 0) => (index + offset) % Count;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int PreviousBy(int index, int offset = 0) => (index - offset + Count) % Count;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int Next(int index) => NextBy(index, 1);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int Previous(int index) => PreviousBy(index, 1);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (int a, int b, int c) Neigh(int index) => (Previous(index), index, Next(index));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (int a, int b, int c) Neigh(Node node) => Neigh(IndexOf(node));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public (Node a, Node b, Node c) NeighNodes(int index) => (this[Previous(index)], this[index], this[Next(index)]);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public Node Pop(int index) {
    var node = this[index];
    RemoveAt(index);
    return node;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Notify() => Changed?.Invoke(this.ToArray(), EventArgs.Empty);

  public event EventHandler? Changed;
}
