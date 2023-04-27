using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

public abstract class Searchable {
  private ImmutableArray<List<Node>> Search(Instance instance, Configuration configuration) {
    var initializers = configuration.Initializers.ToArray();
    configuration.Initializers.Clear();
    foreach (var initializer in initializers) configuration.Population = initializer(instance, configuration);

    return Call(instance, configuration);
  }

  protected abstract ImmutableArray<List<Node>> Call(Instance instance, Configuration configuration);

  public delegate ImmutableArray<List<Node>> Callback(Instance instance, Configuration configuration);

  public static implicit operator Callback(Searchable value) => value.Search;

  public record struct Configuration {
    public Configuration(int size, int dimension) {
      Population = Enumerable.Range(0, size).Select(_ => new List<Node>(dimension)).ToImmutableArray();
    }

    public ImmutableArray<List<Node>> Population;
    public List<Callback> Initializers = new();
    public int Regret = 0;
    public float Weight = 0;
    public float TimeLimit = 0;
    public int IterationLimit = 0;
    public string? Variant = null;
  }
}
