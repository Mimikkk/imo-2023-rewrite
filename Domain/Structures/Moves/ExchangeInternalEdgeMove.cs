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
  public static ExchangeInternalEdgeMove[] AssignSpace(Instance instance, IList<NodeList> cycles)
    => new ExchangeInternalEdgeMove[cycles.Count * instance.Dimension * (instance.Dimension - 1) / 2];


  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeInternalEdgeMove> Find(Instance instance, NodeList cycle) {
    var moves = new ExchangeInternalEdgeMove[cycle.Count * (cycle.Count - 1) / 2];
    Fill(instance, cycle, ref moves);
    return moves;
  }
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Fill(Instance instance, NodeList cycle, ref ExchangeInternalEdgeMove[] moves, int offset = 0) {
    var k = offset - 1;

    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      moves[++k] = new(cycle, i, j, CalculateGain(instance, cycle, i, j));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Fill(Instance instance, IList<NodeList> cycles, ref ExchangeInternalEdgeMove[] moves) {

    var offset = 0;
    foreach (var cycle in cycles) {
      Fill(instance, cycle, ref moves, offset);
      offset += cycle.Count * (cycle.Count - 1) / 2;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeInternalEdgeMove FirstWithGain(Instance instance, IEnumerable<NodeList> cycles) {
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
  public static ExchangeInternalEdgeMove BestByGain(Instance instance, IEnumerable<NodeList> cycles) {
    ExchangeInternalEdgeMove best = default;
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
