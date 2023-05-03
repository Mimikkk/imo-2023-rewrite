namespace Domain.Structures.NodeLists;

public sealed partial class NodeList {
  public NodeList Clone() {
    var clone = new NodeList(_capacity);
    clone._list.AddRange(_list);
    return clone;
  }
}
