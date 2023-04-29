using Domain.Structures.NodeLists;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public sealed class Gains {
    public Gains(Instance instance) => D = instance.Distance;

    public int Insert((Node a, Node b) edge, Node node) =>
      D[(edge.a, node, edge.b)] - D[edge];


    public int ExchangeEdge(NodeList cycle, int exchange, int with) {
      var i = exchange;
      var j = with;

      if (i > j) (i, j) = (j, i);

      var (a, b, c, d) = i == 0 && j == cycle.Count - 1
        ? (cycle[i], cycle[cycle.Next(i)], cycle[cycle.Previous(j)], cycle[j])
        : (cycle[cycle.Previous(i)], cycle[i], cycle[j], cycle[cycle.Next(j)]);

      return D[a, b] + D[c, d] - D[a, c] - D[b, d];
    }

    public int ExchangeVertex(NodeList first, NodeList second, int exchange, int with) =>
      ReplaceVertex(first, exchange, with) + ReplaceVertex(second, with, exchange);

    public int ReplaceVertex(NodeList cycle, int a, int b) {
      var va = cycle.NeighNodes(a);

      return D[va] - D[va with { b = cycle[b] }];
    }

    private readonly Distances D;
  }
}
