using System.Collections;

namespace Domain.Structures.NodeLists;

public sealed partial class NodeList : IList<Node> {
  public int Count => _list.Count;
  public bool IsReadOnly => false;

  public IEnumerator<Node> GetEnumerator() => _list.GetEnumerator();
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public void Add(Node item) {
    _list.Add(item);
    _used[item.Index] = true;
  }

  public void Clear() {
    _list.Clear();
    _used.AsSpan().Clear();
  }

  public bool Contains(Node item) => _used[item.Index];
  public void CopyTo(Node[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

  public bool Remove(Node item) {
    _used[item.Index] = false;
    return _list.Remove(item);
  }

  public int IndexOf(Node item) => _list.IndexOf(item);

  public void Insert(int index, Node item) {
    _list.Insert(index, item);
    _used[item.Index] = true;
  }

  public void RemoveAt(int index) {
    _list.RemoveAt(index);
    _used[_list[index].Index] = false;
  }

  public Node this[int index] {
    get => _list[index];
    set {
      _list[index] = value;
      _used[value.Index] = true;
    }
  }

  private readonly List<Node> _list;
  private readonly bool[] _used;
  private readonly int _capacity;
}
