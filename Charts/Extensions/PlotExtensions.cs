using Domain.Shareable;
using ScottPlot;

namespace Charts.Extensions;

public static class PlotExtensions {
  public static void Save(this Plot chart, string filename) {
    if (!Directory.Exists($"{Shared.Directories.Figures}")) Directory.CreateDirectory($"{Shared.Directories.Figures}");

    chart.SavePng($"{Shared.Directories.Figures}/{filename}.png", 1200, 800);
  }
}
