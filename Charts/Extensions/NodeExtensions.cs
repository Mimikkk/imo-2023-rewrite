using Domain.Structures;
using ScottPlot;

namespace Charts.Structures;

public static class NodeExtensions {
  public static Coordinates Into(this Node node) => new(node.X, node.Y);
}
