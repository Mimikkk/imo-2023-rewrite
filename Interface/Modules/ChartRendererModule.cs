using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Algorithms.Searches;
using Charts.Extensions;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.NodeLists;
using ScottPlot;
using SkiaSharp;

namespace Interface.Modules;

internal sealed record ChartRendererModule {
  public void Notify() {
    Self.Chart.Plot.Clear();
    Updates.ForEach(update => update());
    Self.Chart.Refresh();
  }

  public ChartRendererModule(MainWindow window) {
    Self = window;
    Updates = new() {
      () => Add.Scatter(I.Instance.Nodes),
      () => M.Histories.ForEach(Render),
      () => (P.Cycles.Count > 0).And(() => P.Cycles.ForEach(cycle => Add.Cycle(cycle, I.Instance))),
      () => Mouse.Closest.Let(Add.Point),
      () => {
        if (Mouse.Selection.Count < 1) return;
        Add.Point(Mouse.Selection.First(), SKColors.Coral);

        var color = Self.Chart.Plot.Plottables.Count;
        var plotted = NodeList.Create(I.Instance.Dimension);

        foreach (var history in M.Histories.Where(history => history.Count > I.Step)) {
          var nodes = history[I.Step].Except(new[] { Mouse.Selection.First() }).ToList();

          nodes.ForEach(plotted.Add);
          Add.Distance(Mouse.Selection.First(), nodes, M.Palette.GetColor(++color).ToSKColor());
        }

        Add.Distance(Mouse.Selection.First(),
          I.Instance.Nodes.Except(plotted)
            .Except(Mouse.Selection));
      },
      () => Mouse.Selection.ForEach(n => Add.Point(n)),
      // () => M.AverageScore.Let(value => Add.Label($"Przeciętna długość: {value:F2}")),
      // () => M.WorstScore.Let(value => Add.Label($"Najgorsza długość: {value:F2}")),
      // () => M.BestScore.Let(value => Add.Label($"Najlepsza długość: {value:F2}")),
      // () => M.AverageTime.Let(value => Add.Label($"Przeciętny czas wykonywania: {value:F2}[ms]")),
      // () => M.WorstTime.Let(value => Add.Label($"Najgorszy czas wykonywania: {value:F2}[ms]")),
      // () => M.BestTime.Let(value => Add.Label($"Najlepszy czas wykonywania: {value:F2}[ms]")),
      // () => M.AverageGain.Let(value => Add.Label($"Przeciętny zysk w długości: {value:F2}")),
      // () => M.WorstGain.Let(value => Add.Label($"Najgorszy zysk w długości: {value:F2}")),
      // () => M.BestGain.Let(value => Add.Label($"Najlepszy zysk w długości: {value:F2}")),
      () => (I.Parameter.PopulationSize > 1).And(() =>
        Add.Label($"Łączna długość: {
          M.Histories.Sum(history =>
            I.Instance.Distance[history.ElementAtOrDefault(I.Step) ?? history[^1]])
        }"))
    };
  }

  private void Render(IEnumerable<List<Node>> history) {
    var enumerable = history as List<Node>[] ?? history.ToArray();
    var step = enumerable.ElementAtOrDefault(I.Step) ?? enumerable[^1];
    Render(step);
  }

  private void Render(IEnumerable<Node> cycle) {
    switch (I.Algorithm.DisplayAs) {
      case Search.DisplayType.Cycle:
        Add.Cycle(cycle, I.Instance);
        return;
      case Search.DisplayType.Path:
        if (I.Step == (int)Self.HistorySlider.Maximum) Add.Cycle(cycle, I.Instance);
        else Add.Path(cycle, I.Instance);
        return;
    }
  }

  private MainWindow Self { get; }
  private AddPlottable Add => Self.Chart.Plot.Add;
  private readonly List<Action> Updates;
  private InteractionModule I => Self.Mod.Interaction;
  private MemoryModule M => Self.Mod.Memory;
  private MouseModule Mouse => Self.Mod.Mouse;
  private CyclePanelModule P => Self.Mod.Panel;
}
