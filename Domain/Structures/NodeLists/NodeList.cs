namespace Domain.Structures.NodeLists;

public sealed partial class NodeList {
  public NodeList(int capacity, Action<IList<Node>>? action = null) {
    _capacity = capacity;
    _list = new List<Node>(_capacity);
    _used = new bool[_capacity];
    if (action is not null) Changed += (_, _) => action.Invoke(this.ToArray());
  }

  public static NodeList Create(int capacity, Action<IList<Node>> action) => new(capacity, action);
  public static NodeList Create(int capacity) => new(capacity);

  public void Notify() => Changed?.Invoke(this.ToArray(), EventArgs.Empty);
  public event EventHandler? Changed;
}
