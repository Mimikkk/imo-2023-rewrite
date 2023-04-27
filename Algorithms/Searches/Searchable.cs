using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

public abstract class Searchable {
  protected abstract ImmutableArray<List<Node>> Call(Instance instance, Configuration configuration);

  public delegate ImmutableArray<List<Node>> Callback(Instance instance, Configuration configuration);

  public static implicit operator Callback(Searchable value) => value.Call;

  public record struct Configuration {
    public Configuration(int size, int dimension) {
      Population = Enumerable.Range(0, size).Select(_ => new List<Node>(dimension)).ToImmutableArray();
    }

    public ImmutableArray<List<Node>> Population;
    public Searchable? Initializer = null;
    public int Regret = 0;
    public float Weight = 0;
    public float TimeLimit = 0;
    public int IterationLimit = 0;
    public string? Variant = null;
  }
}
