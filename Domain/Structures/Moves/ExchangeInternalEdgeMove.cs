using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExchangeInternalEdgeMove(NodeList Cycle, int From, int To) : IMove {
  public void Apply() => Apply(Cycle, From, To);

  public static void Apply(NodeList cycle, int from, int to) {
    var ia = from;
    var ib = to;

    if (ia > ib) (ia, ib) = (ib, ia);

    if (ia == 0 && ib == cycle.Count - 1) cycle.Swap(ia, ib);

    var elementCount = ib - ia + 1;
    for (var i = 0; i < elementCount / 2; ++i) {
      cycle.Swap((ia + cycle.Count + i) % cycle.Count, (ib + cycle.Count - i) % cycle.Count);
    }
  }

  public static IEnumerable<ExchangeInternalEdgeMove> Find(NodeList cycle) {
    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      yield return new(cycle, i, j);
  }
}
