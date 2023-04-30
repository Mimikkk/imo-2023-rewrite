using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExchangeExternalVerticesMove(NodeList First, NodeList Second, int From, int To, int Gain) : IMove {
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
  public static IEnumerable<ExchangeExternalVerticesMove> Find(Instance instance, IList<NodeList> cycles) {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerable<IEnumerable<ExchangeExternalVerticesMove>> NestedEnumerable() {
      for (var i = 0; i < cycles.Count; ++i)
      for (var j = i + 1; j < cycles.Count; ++j)
        yield return Find(instance, cycles[i], cycles[j]);
    }

    return NestedEnumerable().Flatten();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<ExchangeExternalVerticesMove> Find(Instance instance, NodeList a, NodeList b) {
    for (var i = 0; i < a.Count; ++i)
    for (var j = 0; j < b.Count; ++j)
      yield return new(a, b, i, j, CalculateGain(instance, a, b, i, j));
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
