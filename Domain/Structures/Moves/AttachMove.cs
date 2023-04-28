using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public record AttachMove(NodeList To, Node Node) : Move {
  public void Apply() => Apply(To, Node);

  public static void Apply(NodeList to, Node node) {
    to.Add(node);
    to.Notify();
  }
}
