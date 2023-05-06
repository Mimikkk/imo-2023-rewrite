using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public readonly record struct ExchangeExternalVerticesMove(NodeList First, NodeList Second, int From, int To, int Gain) : IMove {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Apply() => Apply(First, Second, From, To);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Apply(NodeList first, NodeList second, int from, int to) {
    var a = first.Pop(from);
    var b = second.Pop(to);
    InsertMove.Apply(first, b, from);
    InsertMove.Apply(second, a, to);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeExternalVerticesMove[] AssignSpace(Instance instance, IList<NodeList> cycles)
    => new ExchangeExternalVerticesMove[cycles.Count * (cycles.Count - 1) * instance.Dimension * instance.Dimension];


  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeExternalVerticesMove> Find(Instance instance, IList<NodeList> cycles) {
    var size = 0;
    for (var i = 0; i < cycles.Count; ++i)
    for (var j = i + 1; j < cycles.Count; ++j)
      size += cycles[i].Count * cycles[j].Count;
    if (size is 0) return Enumerable.Empty<ExchangeExternalVerticesMove>();

    var moves = new ExchangeExternalVerticesMove[size];
    Fill(instance, cycles, ref moves);

    return moves;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Fill(Instance instance, IList<NodeList> cycles, ref ExchangeExternalVerticesMove[] moves) {
    var k = -1;

    for (var i = 0; i < cycles.Count; ++i)
    for (var j = i + 1; j < cycles.Count; ++j) {
      var first = cycles[i];
      var second = cycles[j];

      for (var m = 0; m < first.Count; ++m)
      for (var n = 0; n < second.Count; ++n)
        moves[++k] = new(first, second, m, n, CalculateGain(instance, first, second, m, n));

    }
  }


  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeExternalVerticesMove FirstWithGain(Instance instance, IList<NodeList> cycles) {
    for (var i = 0; i < cycles.Count; ++i)
    for (var j = i + 1; j < cycles.Count; ++j) {
      var first = cycles[i];
      var second = cycles[j];

      for (var m = 0; m < first.Count; ++m)
      for (var n = 0; n < second.Count; ++n) {
        var gain = CalculateGain(instance, first, second, m, n);
        if (gain > 0) return new(first, second, m, n, gain);
      }
    }

    return default;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static ExchangeExternalVerticesMove BestByGain(Instance instance, IList<NodeList> cycles) {
    ExchangeExternalVerticesMove best = default;

    for (var i = 0; i < cycles.Count; ++i)
    for (var j = i + 1; j < cycles.Count; ++j) {
      var first = cycles[i];
      var second = cycles[j];

      for (var m = 0; m < first.Count; ++m)
      for (var n = 0; n < second.Count; ++n) {
        var gain = CalculateGain(instance, first, second, m, n);
        if (gain > best.Gain) best = new(first, second, m, n, gain);
      }
    }

    return best;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, NodeList first, NodeList second, int exchange, int with) =>
    ReplaceVertex(instance, first, exchange, second[with]) + ReplaceVertex(instance, second, with, first[exchange]);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int ReplaceVertex(Instance instance, NodeList cycle, int i, Node b) {
    var va = cycle.NeighNodes(i);

    return instance.Distance[va] - instance.Distance[va with { b = b }];
  }
}
