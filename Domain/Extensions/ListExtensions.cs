namespace Domain.Extensions;

public static class ListExtensions {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this IList<T> list, int i, int j) => (list[i], list[j]) = (list[j], list[i]);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Swap<T>(this IList<T> list, T a, T b) => list.Swap(list.IndexOf(a), list.IndexOf(b));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Contains<T>(this IList<T> list, IEnumerable<T> items) => items.All(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Contains<T>(this IList<T> list, IEnumerable<IEnumerable<T>> items) => items.All(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Contains<T>(this IList<T> list, params T[] items) => items.All(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool Contains<T>(this IList<T> list, params T[][] items) => items.All(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ContainsAny<T>(this IList<T> list, IEnumerable<T> items) => items.Any(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ContainsAny<T>(this IList<T> list, IEnumerable<IEnumerable<T>> items) => items.Any(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ContainsAny<T>(this IList<T> list, params T[] items) => items.Any(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ContainsAny<T>(this IList<T> list, params T[][] items) => items.Any(list.Contains);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IList<T> AddWhen<T>(this IList<T> list, T item, bool predicate = false) =>
    list.Also(_ => list.Add(item), predicate);
}
