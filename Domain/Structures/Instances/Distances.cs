using Domain.Calculations;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public sealed record Distances {
    public int this[int a, int b] => _distances[a, b];
    public int this[Node a, Node b] => _distances[a.Index, b.Index];
    public int this[(Node a, Node b) edge] => this[edge.a, edge.b];
    public int this[(Node a, Node b, Node c) vertex] => this[vertex.a, vertex.b] + this[vertex.b, vertex.c];
    public int this[IList<Node> cycle] => NodesCalculations.Edges(cycle).Sum(edge => this[edge.vertices]);
    public int this[IEnumerable<IList<Node>> cycles] => cycles.Sum(cycle => this[cycle]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Node Furthest(Node node) => FurthestBy(node);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Node FurthestBy(Node node, int offset = 0) => I.Nodes[_furthest[node.Index, offset]];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Node Closest(Node node) => ClosestBy(node);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Node ClosestBy(Node node, int offset = 0) => I.Nodes[_furthest[node.Index, I.Dimension - 2 - offset]];

    private readonly int[,] _distances;
    private readonly int[,] _furthest;

    private int[,] PrecalculateDistances() {
      var distances = new int[I.Dimension, I.Dimension];

      for (var i = 0; i < I.Dimension; ++i)
      for (var j = i + 1; j < I.Dimension; ++j) {
        distances[i, j] = distances[j, i] = NodeCalculations.Distance(I.Nodes[i], I.Nodes[j]);
      }


      return distances;
    }

    private int[,] PrecalculateFurthest() {
      var furthest = new int[I.Dimension, I.Dimension];
      _distances.Rows().ForEach((row, i) => {
        var j = 0;
        foreach (var (index, _) in row.Enumerate().OrderByDescending(x => x.value)) furthest[i, j++] = index;
      });

      return furthest;
    }

    private Instance I { get; }

    public Distances(Instance instance) {
      I = instance;
      _distances = PrecalculateDistances();
      _furthest = PrecalculateFurthest();
    }
  }

  public readonly Distances Distance;
}
