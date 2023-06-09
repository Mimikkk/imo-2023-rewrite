using Domain.Structures;

namespace Domain.Calculations;

public static class NodeCalculations {
  [MethodImpl(MethodImplOptions.AggressiveInlining)] 
  public static int Distance(Node from, Node to) => DomainCalculations.Distance(from.X - to.X, from.Y - to.Y);
}
