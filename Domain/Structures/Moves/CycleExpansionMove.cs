using Domain.Calculations;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record CycleExpansionMove(NodeList To, Node Node, int At) : IMove {
  public void Apply() => Apply(To, Node, At);
  public static void Apply(NodeList to, Node node, int at) => InsertMove.Apply(to, node, at);

  public CycleExpansionMove
    FindBestFitByLowestGain(Instance instance, NodeList cycle, IEnumerable<Node> except) {
    var nodes = instance.Nodes.Except(except).ToArray();

    var (node, at, _) =
      NodesCalculations.Edges(cycle)
        .SelectMany(edge => nodes.Select(node =>
          (node: edge.vertices.b, at: edge.indices.j, gain: instance.Gain.Insert(edge.vertices, node)))
        ).MinBy(candidate => candidate.gain);

    return new CycleExpansionMove(cycle, node, at);
  }
}
