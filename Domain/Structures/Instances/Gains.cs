using Domain.Calculations;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public sealed class Gains {
    public Gains(Instance instance) => D = instance.Distance;

    public int Insert((Node a, Node b) edge, Node node) =>
      D[(edge.a, node, edge.b)] - D[edge];

    private readonly Distances D;
  }
}
