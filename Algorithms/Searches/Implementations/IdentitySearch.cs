using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches.Implementations;

public class IdentitySearch : Search {
  protected override IEnumerable<IEnumerable<Node>> Call(Instance instance, Configuration configuration) =>
    configuration.Population;
}
