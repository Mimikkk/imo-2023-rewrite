namespace Domain.Calculations;

public static class DomainCalculations {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Distance(int x, int y) => (int)MathF.Round(Magnitude(x, y));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Magnitude(int x, int y) => (int)MathF.Sqrt(SquareMagnitude(x, y));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int SquareMagnitude(int x, int y) => x * x + y * y;
}
