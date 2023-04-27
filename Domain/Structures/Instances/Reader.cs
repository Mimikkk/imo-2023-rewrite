using System.Collections.Immutable;
using Domain.Shareable;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public static Instance Read(string name) => new(
    File
      .ReadLines(Path.Combine(Shared.Directories.Instances, $"{name}.tsp"))
      .Skip(6)
      .SkipLast(1)
      .Select(descriptor => descriptor.Split(" ").Select(int.Parse).ToImmutableArray())
      .Select(coords => new Node(coords[0] - 1, coords[1], coords[2])
      ),
    name
  );
}
