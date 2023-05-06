using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public readonly record struct ExchangeInternalVerticesMove(NodeList Cycle, int From, int To, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(Cycle, From, To);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList cycle, int from, int to) {
    cycle.Swap(from, to);
    cycle.Notify();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeInternalVerticesMove[] AssignSpace(Instance instance, IList<NodeList> cycles)
    => new ExchangeInternalVerticesMove[cycles.Count * instance.Dimension * (instance.Dimension - 1) / 2];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeInternalVerticesMove> Find(Instance instance, NodeList cycle) {
    var moves = new ExchangeInternalVerticesMove[cycle.Count * (cycle.Count - 1) / 2];
    Fill(instance, cycle, ref moves);
    return moves;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Fill(Instance instance, NodeList cycle, ref ExchangeInternalVerticesMove[] moves, int offset = 0) {
    var k = offset - 1;

    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      moves[++k] = new(cycle, i, j, CalculateGain(instance, cycle, i, j));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Fill(Instance instance, IList<NodeList> cycles, ref ExchangeInternalVerticesMove[] moves) {

    var offset = 0;
    foreach (var cycle in cycles) {
      Fill(instance, cycle, ref moves, offset);
      offset += cycle.Count * (cycle.Count - 1) / 2;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeInternalVerticesMove FirstWithGain(Instance instance, IEnumerable<NodeList> cycles) {
    foreach (var cycle in cycles.Shuffle()) {
      for (var i = 0; i < cycle.Count; ++i)
      for (var j = i + 1; j < cycle.Count; ++j) {
        var gain = CalculateGain(instance, cycle, i, j);
        if (gain > 0) return new(cycle, i, j, gain);
      }
    }

    return default;
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeInternalVerticesMove BestByGain(Instance instance, IEnumerable<NodeList> cycles) {
    ExchangeInternalVerticesMove best = default;
    foreach (var cycle in cycles) {
      for (var i = 0; i < cycle.Count; ++i)
      for (var j = i + 1; j < cycle.Count; ++j) {
        var gain = CalculateGain(instance, cycle, i, j);
        if (gain > best.Gain) best = new(cycle, i, j, gain);
      }
    }

    return best;
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
