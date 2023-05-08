using System.Collections.Immutable;
using Domain.Shareable;

namespace Domain.Extensions;

public static class RandomExtensions {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, IEnumerable<T> list) => random.Choose(list.ToArray());

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, IList<T> list) => list[random.Next(list.Count)];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, ImmutableArray<T> list) => list[random.Next(list.Length)];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static (T, T) Choose2<T>(this Random random, IList<T> list) {
    var a = Shared.Random.Next(list.Count);
    var b = Shared.Random.Next(list.Count);
    while (a == b) b = Shared.Random.Next(list.Count);

    return (list[a], list[b]);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) =>
    items.OrderBy(_ => Shared.Random.Next()).ToArray();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IList<T> Shuffle<T>(this List<T> items) {
    items.Sort((_, _) => Shared.Random.Next());
    return items;
  }
}
