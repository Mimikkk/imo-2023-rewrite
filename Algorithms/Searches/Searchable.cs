using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches;

public abstract class Searchable {
  public ImmutableArray<NodeList> Search(Instance instance, Configuration configuration) {
    var initializers = configuration.Initializers.ToArray();
    configuration.Initializers.Clear();
    foreach (var initializer in initializers) configuration.Population = initializer(instance, configuration);

    return Call(instance, configuration);
  }

  protected abstract ImmutableArray<NodeList> Call(Instance instance, Configuration configuration);

  public delegate ImmutableArray<NodeList> Callback(Instance instance, Configuration configuration);

  public static implicit operator Callback(Searchable value) => value.Search;

  public record struct Configuration {
    public Configuration(int size, int dimension) {
      Population = Enumerable.Range(0, size).Select(_ => NodeList.Create(dimension)).ToImmutableArray();
    }

    public ImmutableArray<NodeList> Population;
    public List<Callback> Initializers = new();
    public readonly List<int> Gains = new();
    public int? Start = 0;
    public int Regret = 0;
    public float Weight = 0;
    public float TimeLimit = 0;
    public int IterationLimit = 0;
    public string? Variant = null;
  }
}
