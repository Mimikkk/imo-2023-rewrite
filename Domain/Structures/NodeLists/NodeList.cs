namespace Domain.Structures.NodeLists;

public sealed partial class NodeList {
  public NodeList(int capacity, Action<NodeList>? action = null) {
    _capacity = capacity;
    _list = new List<Node>(_capacity);
    _used = new bool[_capacity];
    if (action is not null) Changed += (_, _) => action.Invoke(this);
  }

  public static NodeList Create(int capacity, Action<NodeList> action) => new(capacity, action);
  public static NodeList Create(int capacity) => new(capacity);

  public void Notify() => Changed?.Invoke(this, EventArgs.Empty);
  public event EventHandler? Changed;
}
