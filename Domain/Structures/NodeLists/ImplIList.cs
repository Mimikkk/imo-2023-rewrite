using System.Collections;

namespace Domain.Structures.NodeLists;

public sealed partial class NodeList : IList<Node> {
  public int Count => _list.Count;
  public bool IsReadOnly => false;


  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public IEnumerator<Node> GetEnumerator() => _list.GetEnumerator();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Add(Node item) {
    _list.Add(item);
    _used[item.Index] = true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Clear() {
    _list.Clear();
    _used.AsSpan().Clear();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Contains(Node item) => _list.Contains(item);

  // [MethodImpl(MethodImplOptions.AggressiveInlining)]
  // public bool Contains(int index) => _list.Contains();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void CopyTo(Node[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool Remove(Node item) {
    _used[item.Index] = false;
    return _list.Remove(item);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public int IndexOf(Node item) => _list.IndexOf(item);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Insert(int index, Node item) {
    _used[item.Index] = true;
    _list.Insert(index, item);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void RemoveAt(int index) {
    _used[_list[index].Index] = false;
    _list.RemoveAt(index);
  }

  public Node this[int index] {
    get => _list[index];
    set => _list[index] = value;
  }

  private readonly List<Node> _list;
  private readonly bool[] _used;
  private readonly int _capacity;
}
