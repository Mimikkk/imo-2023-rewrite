using Domain.Structures;
using Domain.Structures.NodeLists;

namespace Domain.Methods;

public static class NodesCombinatorics {
  public static IEnumerable<(NodeList cycle, (Node a, Node b) vertices, (int i, int j) indices)>
    Combinations(this NodeList cycle) {
    for (var i = 0; i < cycle.Count; ++i)
    for (var j = i + 1; j < cycle.Count; ++j)
      yield return (cycle, (cycle[i], cycle[j]), (i, j));
  }
}
