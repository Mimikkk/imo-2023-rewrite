using Domain.Structures;
using Domain.Structures.Instances;
using static Algorithms.Searches.ISearch;

namespace Algorithms.Searches.Implementations;

public class IdentitySearch : ISearch {
  public IEnumerable<IEnumerable<Node>> Call(Instance instance, Configuration configuration) =>
    configuration.Population;
}
