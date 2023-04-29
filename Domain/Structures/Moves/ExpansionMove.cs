using Domain.Calculations;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExpansionMove(NodeList To, Node Node, int At, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(To, Node, At);
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList to, Node node, int at) => InsertMove.Apply(to, node, at);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExpansionMove Find(Instance instance, NodeList cycle, ICollection<Node> nodes) =>
    From(
      cycle,
      NodesCalculations.Edges(cycle)
        .SelectMany(e => nodes.Select(n =>
          (node: n, at: e.indices.j, gain: CalculateGain(instance, e.vertices, n)))
        ).MinBy(candidate => candidate.gain)
    );

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ExpansionMove From(NodeList cycle, (Node node, int at, int gain) candidate) =>
    new(cycle, candidate.node, candidate.at, candidate.gain);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, (Node a, Node b) at, Node node) =>
    instance.Distance[(at.a, node, at.b)] - instance.Distance[at];
}
