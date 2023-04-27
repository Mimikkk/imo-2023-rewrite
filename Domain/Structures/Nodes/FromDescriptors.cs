namespace Domain.Structures.Nodes;

public readonly partial record struct Node {
  public static IEnumerable<Node> From(IEnumerable<string> descriptors) =>
    descriptors
      .Select(descriptor => descriptor.Split(" ").Select(int.Parse).ToArray())
      .Select(coords => new Node(coords[0] - 1, coords[1], coords[2]));
}
