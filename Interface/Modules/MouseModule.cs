using System.Linq;
using Charts.Extensions;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.NodeLists;
using ScottPlot.Control;

namespace Interface.Modules;

internal sealed record MouseModule(MainWindow Self) {
  public void UpdateClosest() {
    var mouse = CI.GetMouseCoordinates();
    Closest = I.Instance.Nodes.MinBy(node => mouse.Distance(node))!;
  }

  public void UpdateSelected() {
    if (NotCloseEnough || Closest is null) return;

    if (Selection.Contains(Closest)) {
      if (Selection.First() == Closest) Selection.Remove(Closest);
      else Selection.Swap(Selection[0], Closest);
    }
    else {
      Selection.Clear();
      Selection.Add(Closest);
    }
  }

  public void UpdateSelection() {
    if (NotCloseEnough || Closest is null) return;

    if (Selection.Contains(Closest)) Selection.Remove(Closest);
    else Selection.Add(Closest);
  }

  public Node? Closest { get; private set; }
  public readonly NodeList Selection = new(200);

  private InteractionModule I => Self.Mod.Interaction;
  private Interaction CI => Self.Chart.Interaction;
  private bool NotCloseEnough => Closest is null || CI.GetMouseCoordinates().Distance(Closest) >= 125;
}
