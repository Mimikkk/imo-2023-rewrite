using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public sealed class Gains {
    public Gains(Instance instance) => D = instance.Distance;

    public int Insert((Node a, Node b) edge, Node node) =>
      D[(edge.a, node, edge.b)] - D[edge];



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

    private readonly Distances D;
  }
}
