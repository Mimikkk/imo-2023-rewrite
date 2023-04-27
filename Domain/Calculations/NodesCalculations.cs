using System.Collections.Immutable;
using Domain.Structures;

namespace Domain.Calculations;

public static class NodesCalculations {
  public static IEnumerable<(Node a, Node b)> Edges(IList<Node> nodes) =>
    nodes.Select((_, i) => (
      nodes[i % nodes.Count],
      nodes[(i + 1) % nodes.Count]
    ));

  public static List<Node> Hull(ImmutableArray<Node> points) {
    var nodes = points.ToList();
    var hull = new List<Node> { nodes.MinBy(p => p.X)! };
    nodes.Remove(hull.First());

    var (collinear, counter) = (new List<Node>(), 0);
    while (hull.Count < 2 || hull.First() != hull.Last()) {
      if (++counter == 3) nodes.Add(hull.First());

      var next = nodes.First();
      foreach (var node in nodes.Skip(1))
        switch (CalculateRelation(hull.Last(), next, node)) {
          case GeometricRelation.Collinear:
            collinear.Add(node);
            break;
          case GeometricRelation.RightOf:
            next = node;
            collinear.Clear();
            break;
        }


      if (collinear.Count > 0) {
        collinear.Add(next);

        var last = hull.Last();
        collinear = collinear
          .OrderBy(n => DomainCalculations.SquareMagnitude(n.X - last.X, n.Y - last.Y))
          .ToList();

        hull.AddRange(collinear);
        nodes.RemoveAll(collinear.Contains);
        collinear.Clear();
        continue;
      }

      hull.Add(next);
      nodes.Remove(next);
    }

    return hull;
  }

  private enum GeometricRelation : byte {
    LeftOf,
    RightOf,
    Collinear
  }

  private static GeometricRelation CalculateRelation(Node a, Node b, Node c) =>
    ((float)(a.X - c.X) * (b.Y - c.Y) - (a.Y - c.Y) * (b.X - c.X)) switch {
      < 0.00001f and > -0.00001f => GeometricRelation.Collinear,
      < 0 => GeometricRelation.RightOf,
      _ => GeometricRelation.LeftOf
    };
}
