using Algorithms.Searches;
using Domain.Calculations;
using Domain.Extensions;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;
using Interface.Structures;

namespace Interface.Modules;

internal sealed record InteractionModule(MainWindow Self) {
  public Instance Instance {
    get {
      if (_instance is not null && _instance.Name == InstanceSelection) return _instance;
      _instance = Instance.Read(InstanceSelection);
      _hull = null;
      return _instance;
    }
  }

  public NodeList Hull => _hull ??= NodesCalculations.Hull(Instance.Nodes);

  public Search Algorithm => Self.Algorithms.SelectedItem.As<Option<Search>>().Value;
  public int Step => (int)Self.HistorySlider.Value;

  internal sealed record Parameters(MainWindow Self) {
    public int Regret => (int)Self.ParameterRegret.Value;
    public int? StartIndex => Self.ParameterStartIndex.Value is 0 ? null : (int)Self.ParameterStartIndex.Value;
    public int PopulationSize => (int)Self.ParameterPopulationSize.Value;
    public float Weight => (float)Self.ParameterWeight.Value;
    public int IterationLimit => (int)Self.ParameterIterationLimit.Value;
    public float TimeLimit => (float)Self.ParameterTimeLimit.Value;
    public string Variant => Self.ParameterVariants.SelectedItem.As<Option<string>>().Value;

    public Search Initializer =>
      Self.ParameterInitializers.SelectedItem.As<Option<Search>>().Value;

    public Searchable.Configuration Configuration => new(PopulationSize, _instance!.Dimension) {
      Regret = Regret,
      Weight = Weight,
      TimeLimit = TimeLimit,
      IterationLimit = IterationLimit,
      Initializers = new() { Initializer },
      Variant = Variant,
      Start = StartIndex
    };
  }

  public readonly Parameters Parameter = new(Self);

  private static Instance? _instance;
  private NodeList? _hull;
  private string InstanceSelection => Self.Instances.SelectedItem.As<Option<string>>().Value;
}
