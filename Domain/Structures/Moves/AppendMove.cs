using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record AppendMove(NodeList To, Node Node, int Gain) : IMove {
  public void Apply() => Apply(To, Node);

  public static void Apply(NodeList to, Node node) {
    to.Add(node);
    to.Notify();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, Node from, Node to) => -instance.Distance[from, to];
}
