using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record ExchangeExternalVerticesMove(NodeList First, NodeList Second, int From, int To) : IMove {
  public void Apply() => Apply(First, Second, From, To);

  public static void Apply(NodeList first, NodeList second, int from, int to) {
    first.RemoveAt(from);
    first.Insert(from, second[to]);
    second.RemoveAt(to);
    second.Insert(to, first[from]);

    first.Notify();
    second.Notify();
  }

  public static IEnumerable<ExchangeExternalVerticesMove> Find(IList<NodeList> cycles) {
    IEnumerable<IEnumerable<ExchangeExternalVerticesMove>> NestedEnumerable() {
      for (var i = 0; i < cycles.Count; ++i)
      for (var j = i + 1; j < cycles.Count; ++j)
        yield return Find(cycles[i], cycles[j]);
    }

    return NestedEnumerable().Flatten();
  }

  public static IEnumerable<ExchangeExternalVerticesMove> Find(NodeList a, NodeList b) {
    for (var i = 0; i < a.Count; ++i)
    for (var j = 0; j < b.Count; ++j)
      yield return new(a, b, i, j);
  }
}
