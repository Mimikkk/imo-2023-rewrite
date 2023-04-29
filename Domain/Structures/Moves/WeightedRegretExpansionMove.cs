using Domain.Calculations;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record WeightedRegretExpansionMove(NodeList To, Node Node, int At, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(To, Node, At);
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList to, Node node, int at) => InsertMove.Apply(to, node, at);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static WeightedRegretExpansionMove Find(Instance instance, NodeList cycle, IEnumerable<Node> nodes, int regret,
    float weight) =>
    From(
      cycle,
      nodes.Select(node =>
        NodesCalculations.Edges(cycle)
          .Select(edge => (node, at: edge.indices.j, gain: ExpansionMove.CalculateGain(instance, edge.vertices, node)))
          .OrderBy(n => n.gain)
      ).MinBy(match => {
        var enumerable = match.ToArray();

        return DomainCalculations.Regret(enumerable.Select(x => x.gain).ToArray(), regret)
               + weight * enumerable.Min(x => x.gain);
      })!.MinBy(match => match.gain)
    );

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static WeightedRegretExpansionMove From(NodeList cycle, (Node node, int at, int gain) candidate) =>
    new(cycle, candidate.node, candidate.at, candidate.gain);
}
