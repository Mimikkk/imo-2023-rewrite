namespace Domain.Methods;

public static class DomainCalculations {
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Distance(int x, int y) => (int)MathF.Round(Magnitude(x, y));

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int Magnitude(int x, int y) => (int)MathF.Sqrt(x * x + y * y);
}
