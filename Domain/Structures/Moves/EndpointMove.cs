using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record EndpointMove(NodeList To, Node Node, bool ToStart, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(To, Node, ToStart);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList to, Node node, bool toStart) {
    if (toStart) InsertMove.Apply(to, node, 0);
    else AppendMove.Apply(to, node);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    var tailGain = CalculateGain(instance, tail, toTail);
    var headGain = CalculateGain(instance, head, toHead);

    return tailGain < headGain
      ? new EndpointMove(path, toTail, true, tailGain)
      : new EndpointMove(path, toHead, false, headGain);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, Node from, Node to) => -instance.Distance[from, to];
}
