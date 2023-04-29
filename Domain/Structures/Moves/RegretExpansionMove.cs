using Domain.Calculations;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record RegretExpansionMove(NodeList To, Node Node, int At, int Gain) : IMove {
  public void Apply() => Apply(To, Node, At);
  public static void Apply(NodeList to, Node node, int at) => InsertMove.Apply(to, node, at);

  public static RegretExpansionMove Find(Instance instance, NodeList cycle, IEnumerable<Node> nodes, int regret) =>
    From(
      cycle,
      nodes.Select(node =>
          NodesCalculations.Edges(cycle)
            .Select(edge => (node: node, at: edge.indices.j,
              gain: ExpansionMove.CalculateGain(instance, edge.vertices, node)))
            .OrderBy(n => n.gain)
        )
        .OrderBy(match => DomainCalculations.Regret(match.Select(x => x.gain).ToArray(), regret))
        .First()
        .MinBy(match => match.gain)
    );

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static RegretExpansionMove From(NodeList cycle, (Node node, int at, int gain) candidate) =>
    new(cycle, candidate.node, candidate.at, candidate.gain);
}
