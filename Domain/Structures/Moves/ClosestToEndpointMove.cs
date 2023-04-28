using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public class ClosestToEndpointMove : IMove {
  public void Apply() => Apply(_to, Node, ToStart);

  public static void Apply(NodeList to, Node node, bool toStart) {
    if (toStart) InsertMove.Apply(to, node, 0);
    else AppendMove.Apply(to, node);
  }

  public static ClosestToEndpointMove
    Find(Instance instance, NodeList path, IDictionary<Node, int> offsets, ISet<Node> used) {
    var tail = path.First();
    var head = path.Last();

    Node toTail;
    var offsetTail = 0;
    while (true) {
      toTail = instance.Distance.ClosestBy(tail, offsetTail++);
      if (!used.Contains(toTail)) break;
    }

    var offsetHead = 0;
    Node toHead;
    while (true) {
      toHead = instance.Distance.ClosestBy(head, offsetHead++);
      if (!used.Contains(toHead)) break;
    }

    return instance.Distance[tail, toTail] < instance.Distance[head, toHead]
      ? new ClosestToEndpointMove(path, toTail, --offsetTail, true)
      : new ClosestToEndpointMove(path, toHead, --offsetTail, false);
  }


  private ClosestToEndpointMove(NodeList to, Node node, int closestOffset, bool toStart) {
    _to = to;
    Node = node;
    ToStart = toStart;
    ClosestOffset = closestOffset;
  }

  public readonly Node Node;
  public readonly bool ToStart;
  public readonly int ClosestOffset;
  private readonly NodeList _to;
}
