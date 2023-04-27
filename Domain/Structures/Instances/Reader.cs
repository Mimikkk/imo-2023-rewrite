using Domain.Shareable;

namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public static Instance Read(string name) => new(
    Node.From(File
      .ReadLines(Path.Combine(Shared.Directories.Instances, $"{name}.tsp"))
      .Skip(6)
      .SkipLast(1)),
    name
  );
}
