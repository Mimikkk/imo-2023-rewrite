using System.Collections.Immutable;
using System.Reflection.Metadata;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

public interface ISearch {
  public IEnumerable<IEnumerable<Node>> Call(Instance instance, Configuration configuration);

  public record Configuration {
    public ISearch? Initializer = null;

    public float Weight;
    public int? Start;
    public float TimeLimit;
    public int IterationLimit;
    public int Regret;
    public string Variant;

    public readonly List<int> Gains = new();
    public readonly ImmutableList<List<Node>> Population = null!;
  }
}
