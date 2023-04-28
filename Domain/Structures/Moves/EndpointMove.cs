using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed class EndpointMove : IMove {
  public void Apply() => Apply(_to, Node, ToStart);

  public static void Apply(NodeList to, Node node, bool toStart) {
    if (toStart) InsertMove.Apply(to, node, 0);
    else AppendMove.Apply(to, node);
  }

  public static EndpointMove
    Find(Instance instance, NodeList path, ISet<Node> used) {
    var tail = path.First();
    var head = path.Last();

    var toTail = instance.Distance.Closest(tail);
    for (var offset = 1; used.Contains(toTail); ++offset)
      toTail = instance.Distance.ClosestBy(tail, offset);

    var toHead = instance.Distance.Closest(head);
    for (var offset = 1; used.Contains(toHead); ++offset)
      toHead = instance.Distance.ClosestBy(head, offset);

    return instance.Distance[tail, toTail] < instance.Distance[head, toHead]
      ? new EndpointMove(path, toTail, true)
      : new EndpointMove(path, toHead, false);
  }


  private EndpointMove(NodeList to, Node node, bool toStart) {
    _to = to;
    Node = node;
    ToStart = toStart;
  }

  public readonly Node Node;
  public readonly bool ToStart;
  private readonly NodeList _to;
}
