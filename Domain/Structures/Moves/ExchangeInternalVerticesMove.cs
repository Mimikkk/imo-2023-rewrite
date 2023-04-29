using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExchangeInternalVerticesMove(NodeList Cycle, int From, int To) : IMove {
  public void Apply() => Apply(Cycle, From, To);

  public static void Apply(NodeList cycle, int from, int to) {
    cycle.Swap(from, to);
    cycle.Notify();
  }

  public static IEnumerable<ExchangeInternalVerticesMove> Find(NodeList cycle) {
    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      yield return new(cycle, i, j);
  }
}
