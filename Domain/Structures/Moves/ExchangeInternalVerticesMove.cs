using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExchangeInternalVerticesMove(NodeList Cycle, int From, int To, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(Cycle, From, To);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList cycle, int from, int to) {
    cycle.Swap(from, to);
    cycle.Notify();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeInternalVerticesMove> Find(Instance instance, NodeList cycle) {
    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      yield return new(cycle, i, j, CalculateGain(instance, cycle, i, j));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, NodeList cycle, int i, int j) {
    var D = instance.Distance;
    if (i > j) (i, j) = (j, i);

    var (a, b, c) = cycle.NeighNodes(i);
    var (d, e, f) = cycle.NeighNodes(j);

    if (j - i == 1)
      return D[a, b] + D[e, f] - D[a, e] - D[b, f];

    if ((i, j) == (0, cycle.Count - 1))
      return D[b, c] + D[d, e] - D[e, c] - D[d, b];

    return D[(a, b, c)] + D[(d, e, f)] - D[(a, e, c)] - D[(d, b, f)];
  }
}
