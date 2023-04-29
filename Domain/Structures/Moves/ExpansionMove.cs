using Domain.Calculations;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExpansionMove(NodeList To, Node Node, int At) : IMove {
  public void Apply() => Apply(To, Node, At);
  public static void Apply(NodeList to, Node node, int at) => InsertMove.Apply(to, node, at);

  public static ExpansionMove Find(Instance instance, NodeList cycle, ICollection<Node> nodes) =>
    From(
      cycle,
      NodesCalculations.Edges(cycle)
        .SelectMany(e => nodes.Select(n =>
          (node: n, at: e.indices.j, gain: instance.Gain.Insert(e.vertices, n)))
        ).MinBy(candidate => candidate.gain)
    );

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static ExpansionMove From(NodeList cycle, (Node node, int at, int gain) candidate) =>
    new(cycle, candidate.node, candidate.at);
}
