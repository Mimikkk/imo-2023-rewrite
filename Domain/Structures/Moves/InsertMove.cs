using Domain.Structures.NodeLists;

namespace Domain.Structures.Moves;

public record InsertMove(NodeList To, Node Node, int At) : IMove {
  public void Apply() => Apply(To, Node, At);

  public static void Apply(NodeList to, Node node, int at) {
    to.Insert(at, node);
    to.Notify();
  }
}
