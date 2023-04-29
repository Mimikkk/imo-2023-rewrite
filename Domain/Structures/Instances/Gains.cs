using Domain.Structures.Moves;
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

    public int ExchangeVertices(NodeList first, NodeList second, int exchange, int with) =>
      ReplaceVertex(first, exchange, second[with]) + ReplaceVertex(second, with, first[exchange]);

    public int ExchangeVertices(NodeList cycle, int i, int j) {
      if (i > j) (i, j) = (j, i);

      var (a, b, c) = cycle.NeighNodes(i);
      var (d, e, f) = cycle.NeighNodes(j);

      if (j - i == 1)
        return D[a, b] + D[e, f] - D[a, e] - D[b, f];

      if ((i, j) == (0, cycle.Count - 1))
        return D[b, c] + D[d, e] - D[e, c] - D[d, b];

      return D[(a, b, c)] + D[(d, e, f)] - D[(a, e, c)] - D[(d, b, f)];
    }

    public int ReplaceVertex(NodeList cycle, int i, Node b) {
      var va = cycle.NeighNodes(i);

      return D[va] - D[va with { b = b }];
    }

    private readonly Distances D;
  }
}
