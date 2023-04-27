using Domain.Methods;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public record Distances {
    public int this[int a, int b] => _distances[a, b];
    public int this[Node a, Node b] => _distances[a.Index, b.Index];
    public int this[(Node a, Node b) edge] => this[edge.a, edge.b];
    public int this[(Node a, Node b, Node c) vertex] => this[vertex.a, vertex.b] + this[vertex.b, vertex.c];

    public Distances(Instance instance) => _distances = Create(instance);

    private readonly int[,] _distances;

    private static int[,] Create(Instance instance) {
      var distances = new int[instance.Dimension, instance.Dimension];

      for (var i = 0; i < instance.Dimension; ++i)
      for (var j = i + 1; j < instance.Dimension; ++j) {
        distances[i, j] = distances[i, j] = NodeCalculations.Distance(instance.Nodes[i], instance.Nodes[j]);
      }


      return distances;
    }
  }

  public readonly Distances Distance;
}
