namespace Domain.Extensions;

public static class EnumerableExtensions {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static IEnumerable<(int Index, T value)> Enumerate<T>(this IEnumerable<T> enumerable) =>
    enumerable.Select((value, index) => (index, value));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
    foreach (var item in enumerable) action(item);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action) {
    var i = 0;
    foreach (var item in enumerable) action(item, i++);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ForEach<T>(this IEnumerable<T> enumerable, Action action) {
    foreach (var _ in enumerable) action();
  }
}
