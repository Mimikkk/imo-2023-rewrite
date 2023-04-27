using System.Collections.Immutable;
using Domain.Structures.Nodes;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public readonly ImmutableArray<Node> Nodes;
  public readonly int Dimension;
  public readonly string Name;

  private Instance(IEnumerable<Node> nodes, string name) {
    Nodes = nodes.ToImmutableArray();
    Name = name;

    Dimension = Nodes.Length;
    Distance = new(this);
  }
}
