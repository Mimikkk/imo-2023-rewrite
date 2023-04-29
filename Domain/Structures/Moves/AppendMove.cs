using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record AppendMove(NodeList To, Node Node, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(To, Node);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList to, Node node) {
    to.Add(node);
    to.Notify();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, Node from, Node to) => -instance.Distance[from, to];
}
