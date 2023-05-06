using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public readonly record struct ExchangeInternalEdgeMove(NodeList Cycle, int From, int To, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(Cycle, From, To);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList cycle, int from, int to) {
    var ia = from;
    var ib = to;

    if (ia > ib) (ia, ib) = (ib, ia);

    if (ia == 0 && ib == cycle.Count - 1) cycle.Swap(ia, ib);

    var elementCount = ib - ia + 1;
    for (var i = 0; i < elementCount / 2; ++i) {
      cycle.Swap((ia + cycle.Count + i) % cycle.Count, (ib + cycle.Count - i) % cycle.Count);
    }

    cycle.Notify();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeInternalEdgeMove> Find(Instance instance, NodeList cycle) {
    var moves = new ExchangeInternalEdgeMove[cycle.Count * (cycle.Count - 1) / 2];
    var k = -1;

    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      moves[++k] = new(cycle, i, j, CalculateGain(instance, cycle, i, j));

    return moves;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, NodeList cycle, int exchange, int with) {
    var D = instance.Distance;
    var i = exchange;
    var j = with;

    if (i > j) (i, j) = (j, i);

    var (a, b, c, d) = i == 0 && j == cycle.Count - 1
      ? (cycle[i], cycle[cycle.Next(i)], cycle[cycle.Previous(j)], cycle[j])
      : (cycle[cycle.Previous(i)], cycle[i], cycle[j], cycle[cycle.Next(j)]);

    return D[a, b] + D[c, d] - D[a, c] - D[b, d];
  }
}
