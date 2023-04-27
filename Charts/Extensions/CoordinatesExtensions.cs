using Domain.Structures;
using ScottPlot;

namespace Charts.Extensions;

public static class CoordinatesExtensions {
  public static void Deconstruct(this Coordinates coord, out double x, out double y) {
    x = coord.X;
    y = coord.Y;
  }

  public static double Distance(this Coordinates from, Coordinates to) {
    var (x, y) = from;
    var (ox, oy) = to;
    return Math.Sqrt(Math.Pow(x - ox, 2) + Math.Pow(y - oy, 2));
  }

  public static double Distance(this Coordinates from, Node to) {
    var (x, y) = from;
    var (ox, oy) = to;
    return Math.Sqrt(Math.Pow(x - ox, 2) + Math.Pow(y - oy, 2));
  }
}
