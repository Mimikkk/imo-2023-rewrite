namespace Domain.Calculations;

public static class DomainCalculations {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Distance(int x, int y) => Magnitude(x, y);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Magnitude(int x, int y) => (int)MathF.Round(MathF.Sqrt(SquareMagnitude(x, y)));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int SquareMagnitude(int x, int y) => x * x + y * y;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Regret(int[] values, int k) {
    var regret = Math.Max(k, values.Length) * values[0];
    for (var i = 1; i < k && i < values.Length; i++) regret -= values[i];
    return regret;
  }
}
