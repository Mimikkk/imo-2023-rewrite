using System.Collections.Immutable;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public ImmutableArray<Node> Nodes;
  public readonly int Dimension;
  public readonly string Name;

  // public unsafe ref Node T() {
    // var x = Unsafe.Add(ref Nodes, 0);
  // }

  private Instance(IEnumerable<Node> nodes, string name) {
    Nodes = nodes.ToImmutableArray();
    Name = name;

    Dimension = Nodes.Length;
    Distance = new(this);
  }
}
