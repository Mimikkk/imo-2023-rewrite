using System.Collections.Immutable;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public ImmutableArray<Node> Nodes;
  public readonly int Dimension;
  public readonly string Name;
  
  private Instance(IEnumerable<Node> nodes, string name) {
    Nodes = nodes.ToImmutableArray();
    Name = name;

    Dimension = Nodes.Length;
    Distance = new(this);
  }
}
