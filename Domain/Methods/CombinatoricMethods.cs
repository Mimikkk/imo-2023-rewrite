namespace Domain.Methods;

public static class Combinatorics {
  public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items, int k) =>
    Combinations(items.ToArray(), k);

  public static IEnumerable<IList<T>> Combinations<T>(this IList<T> items, int k) {
    if (!items.Any()) return Enumerable.Empty<IList<T>>();

    var head = items.First();
    var tail = items.Skip(1).ToArray();
    return tail.Combinations(k - 1).Select(x => new[] { head }.Concat(x).ToArray()).Concat(tail.Combinations(k));
  }
}
