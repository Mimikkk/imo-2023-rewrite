using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public record AppendMove(NodeList To, Node Node) : IMove {
  public void Apply() => Apply(To, Node);

  public static void Apply(NodeList to, Node node) {
    to.Add(node);
    to.Notify();
  }
}
