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
  public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) =>
    items.OrderBy(_ => Shared.Random.Next()).ToList();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IList<T> Shuffle<T>(this List<T> items) {
    items.Sort((_, _) => Shared.Random.Next());
    return items;
  }
}
