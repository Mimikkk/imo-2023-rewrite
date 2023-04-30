using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Algorithms.Searches;
using Avalonia.Controls;
using Avalonia.Input;
using Domain.Shareable;
using Domain.Structures;
using static Domain.Extensions.EnumerableExtensions;
using Domain.Structures.NodeLists;
using Interface.Modules;
using Interface.Structures;

namespace Interface;

public sealed partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();

    Mod = new(this);

    InitializeComboBoxes();
    InitializeChart();
    InitializeListeners();
  }

  private void InitializeListeners() {
    HistoryText.Text = $"Krok: 0";
    HistorySlider.Minimum = 0;


    M.Histories.CollectionChanged += (_, _) => C.Notify();

    HistorySlider.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Value") return;

      HistoryText.Text = $"Krok: {I.Step}";
      C.Notify();
    };
    RunButton.Click += (_, _) => HandleRunCommand();

    ClearParameterStartIndexButton.Click += (_, _) => ParameterStartIndex.Value = 0;
    ParameterStartIndex.PropertyChanged += (_, change) => {
      if (change.Property.Name != "Maximum") return;
      M.Invalidate();
    };
    ParameterStartIndex.ValueChanged += (_, _) => { CalculateAverageButton.IsVisible = ParameterStartIndex.Value > 0; };
    ParameterStartIndex.Value = 0;
    CalculateAverageButton.IsVisible = ParameterStartIndex.Value > 0;

    ClearParameterRegretButton.Click += (_, _) => ParameterRegret.Value = 2;
    ParameterRegret.Value = 2;
    ClearParameterPopulationSizeButton.Click += (_, _) => ParameterPopulationSize.Value = 1;
    ParameterPopulationSize.Value = 1;
    FindBestButton.Click += (_, _) => {
      M.Measure(0, (int)ParameterStartIndex.Maximum);
      ParameterStartIndex.Value = M.BestIndex;
      HandleRunCommand();
    };
    FindWorstButton.Click += (_, _) => {
      M.Measure(0, (int)ParameterStartIndex.Maximum);
      ParameterStartIndex.Value = M.WorstIndex;
      HandleRunCommand();
    };
    CalculateAveragesButton.Click += (_, _) => {
      M.Invalidate();
      M.Measure(0, (int)ParameterStartIndex.Maximum);
      C.Notify();
    };
    CalculateAverageButton.Click += (_, _) => {
      M.Invalidate();
      M.Measure((int)ParameterStartIndex.Value - 1, (int)ParameterStartIndex.Value - 1);
      C.Notify();
    };
  }

  private void InitializeComboBoxes() {
    Instances.SelectionChanged += (_, _) => {
      ParameterStartIndex.Maximum = I.Instance.Dimension;
      ParameterPopulationSize.Maximum = I.Hull.Count() - 1;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
      HistorySlider.Value = 0;
      HistorySlider.Maximum = 0;

      Chart.Plot.AutoScale();
      M.Invalidate();
      M.Histories.Clear();
    };
    Instances.Items = new List<Option<string>> {
      new("KroA 100", "kroA100"),
      new("KroA 200", "kroA200"),
      new("KroB 100", "kroB100"),
      new("KroB 200", "kroB200")
    };
    Instances.SelectedIndex = 0;

    ParameterStartIndex.ValueChanged += (_, _) => {
      ParameterPopulationSize.Maximum = I.Parameter.StartIndex > I.Hull.Count()
        ? 2
        : I.Hull.Count() - 1;
      ParameterPopulationSize.Value = Math.Min(ParameterPopulationSize.Maximum, ParameterPopulationSize.Value);
    };
    ParameterPopulationSize.ValueChanged += (_, _) => {
      ParameterStartIndex.Maximum = I.Parameter.PopulationSize > 2
        ? I.Hull.Count() - 1
        : I.Instance.Dimension;
      ParameterStartIndex.Value = Math.Min(ParameterStartIndex.Maximum, ParameterStartIndex.Value);

      if (!I.Algorithm.UsesVariants) return;
      var size = I.Parameter.PopulationSize;

      ParameterVariants.Items = size switch {
        1 => new List<Option<string>> {
          new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
          new("Wewnętrzna wymiana krawędzi", "internal-edges")
        },
        _ => new List<Option<string>> {
          new("Zewnętrzna wymiana wierzchołków", "external-vertices"),
          new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
          new("Wewnętrzna wymiana krawędzi", "internal-edges"),
          new("Wymiana wierzchołków", "vertices"),
          new("Zew. wym. wierzch. wraz z wew. wym. krawędzi", "external-vertices-internal-edges"),
          new("Mieszany", "mixed")
        }
      };
      ParameterVariants.SelectedIndex = 0;
    };

    Algorithms.SelectionChanged += (_, _) => {
      ParameterRegretBox.IsVisible = I.Algorithm.UsesRegret;
      ParameterWeightBox.IsVisible = I.Algorithm.UsesWeight;
      ParameterTimeLimitBox.IsVisible = I.Algorithm.UsesTimeLimit;
      ParameterIterationLimitBox.IsVisible = I.Algorithm.UsesIterationLimit;
      ParameterInitializersBox.IsVisible = I.Algorithm.UsesInitializers;
      ParameterVariantsBox.IsVisible = I.Algorithm.UsesVariants;
      if (I.Algorithm.UsesVariants) {
        var size = I.Parameter.PopulationSize;

        ParameterVariants.Items = size switch {
          1 => new List<Option<string>> {
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges")
          },
          _ => new List<Option<string>> {
            new("Zewnętrzna wymiana wierzchołków", "external-vertices"),
            new("Wewnętrzna wymiana wierzchołków", "internal-vertices"),
            new("Wewnętrzna wymiana krawędzi", "internal-edges"),
            new("Wymiana wierzchołków", "vertices"),
            new("Zew. wym. wierzch. wraz z wew. wym. krawędzi", "external-vertices-internal-edges"),
            new("Mieszany", "mixed")
          }
        };
        ParameterVariants.SelectedIndex = 0;
      }


      ParameterRegret.Value = 2;
      M.Invalidate();
      C.Notify();
    };
    Algorithms.Items = new List<Option<Search>> {
      new("Najbliższy sąsiad", SearchType.NearestNeighbour),
      new("Rozszerzanie cyklu", SearchType.CycleExpansion),
      new("Rozszerzanie cyklu z k-żalem", SearchType.RegretCycleExpansion),
      new("Rozszerzanie cyklu z ważonym k-żalem", SearchType.WeightedRegretCycleExpansion),
      new("Zachłanne sąsiedztwo", SearchType.GreedyLocal),
      new("Strome sąsiedztwo", SearchType.SteepestLocal),
      // new("Strome sąsiedztwo z pamięcią", Algorithm.SteepestMemory),
      new("Strome sąsiedztwo z listą kandydatów", SearchType.CandidateLocal),
      new("Przypadkowe próbkowanie", SearchType.Random),
      // new("GRASP", Algorithm.RandomAdaptive)
    };
    Algorithms.SelectedIndex = 0;

    ParameterVariants.Items = new List<Option<string>> { new("Domyślny", "default") };
    ParameterVariants.SelectedIndex = 0;

    ParameterInitializers.Items = new List<Option<Search>> {
      new("Brak", SearchType.Identity),
      new("Przypadkowe próbkowanie", SearchType.Random),
      new("Rozszerzanie z k-żalem", SearchType.WeightedRegretCycleExpansion)
    };
    ParameterInitializers.SelectedIndex = 0;
  }

  private void InitializeChart() {
    Chart.Plot.AutoScale();
    C.Notify();

    Chart.PointerMoved += (_, _) => {
      Mouse.UpdateClosest();
      C.Notify();
      P.Notify();
    };
    Chart.PointerReleased += (_, e) => {
      if (e.KeyModifiers == KeyModifiers.Control) Mouse.UpdateSelection();
      else Mouse.UpdateSelected();
      C.Notify();
      P.Notify();
    };
  }

  public void HandleRunCommand() {
    M.Histories.Clear();

    var histories =
      Times(I.Parameter.PopulationSize, () => new List<List<Node>> { new(Mod.Interaction.Instance.Dimension) })
        .ToImmutableArray();

    var configuration = I.Parameter.Configuration with {
      Population = histories
        .Select(history => new NodeList(Mod.Interaction.Instance.Dimension, n => history.Add(n.ToList())))
        .ToImmutableArray(),
    };
    Shared.Random = new(configuration.Start ?? 999);
    I.Algorithm.Search(I.Instance, configuration);

    histories.ForEach(h => M.Histories.Add(h));
    HistorySlider.Maximum = histories.MaxBy(x => x.Count)!.Count - 1;
    HistorySlider.Value = HistorySlider.Maximum;
  }

  internal readonly Modules.Modules Mod;
  private MouseModule Mouse => Mod.Mouse;
  private MemoryModule M => Mod.Memory;
  private ChartRendererModule C => Mod.Chart;
  private InteractionModule I => Mod.Interaction;
  private CyclePanelModule P => Mod.Panel;
}
