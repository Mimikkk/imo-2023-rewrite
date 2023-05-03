using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches;

public abstract class Searchable {
  public ImmutableArray<NodeList> Search(Instance instance, Configuration configuration) {
    configuration = configuration with { Population = configuration.Population };
    Configure(instance, configuration);
    RunInitializers(instance, configuration);
    return Call(instance, configuration);
  }

  protected abstract void Configure(Instance instance, Configuration configuration);

  protected abstract ImmutableArray<NodeList> Call(Instance instance, Configuration configuration);

  protected virtual void RunInitializers(Instance instance, Configuration configuration) {
    var initializers = configuration.Initializers.ToArray();
    configuration.Initializers.Clear();

    foreach (var (initializer, _configuration) in initializers) configuration.Population = initializer(instance, _configuration);
  }

  public delegate ImmutableArray<NodeList> Callback(Instance instance, Configuration configuration);

  public static implicit operator Callback(Searchable value) => value.Search;

  public sealed record Configuration {
    public Configuration(int size, int dimension) {
      Population = Enumerable.Range(0, size).Select(_ => NodeList.Create(dimension)).ToImmutableArray();
    }

    public ImmutableArray<NodeList> Population;
    public List<(Callback, Configuration)> Initializers = new();
    public readonly List<int> Gains = new();
    public int? Start = null;
    public int? Regret = null;
    public float? Weight = null;
    public float? TimeLimit = null;
    public int? IterationLimit = null;
    public string? Variant = null;
  }
}
