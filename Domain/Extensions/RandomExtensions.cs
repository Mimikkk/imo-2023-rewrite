using System.Collections.Immutable;

namespace Domain.Extensions;

public static class RandomExtensions {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, IEnumerable<T> list) => random.Choose(list.ToArray());
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, IList<T> list) => list[random.Next(list.Count)];
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Choose<T>(this Random random, ImmutableArray<T> list) => list[random.Next(list.Length)];
}
