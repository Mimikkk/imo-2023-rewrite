using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public sealed record InsertMove(NodeList To, Node Node, int At, int Gain) : IMove {
  public void Apply() => Apply(To, Node, At);

  public static void Apply(NodeList to, Node node, int at) {
    to.Insert(at, node);
    to.Notify();
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int CalculateGain(Instance instance, Node from, Node to) => -instance.Distance[from, to];
}
