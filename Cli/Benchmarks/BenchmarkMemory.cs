using System.Collections.Immutable;
using Algorithms.Searches;
using Algorithms.Searches.Implementations;
using Charts.Extensions;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Cli.Benchmarks;

public static class BenchmarkMemory {
  public static readonly Instance Instance = Instance.Predefined.KroB200;

  public static Searchable.Configuration Configuration => new(2, Instance.Dimension) {
    Variant = Variant,
    TimeLimit = 3.5f
  };

  public static Searchable Search => SearchType.IteratedLocal;
  public const string SearchName = nameof(SearchType.IteratedLocal);
  public static readonly int? Variant = (int?)IteratedLocalSearch.Variant.SmallPerturbation;
  public const string VariantName = nameof(IteratedLocalSearch.Variant.SmallPerturbation);

  public const int Iterations = 1;

  public static readonly List<(ImmutableArray<NodeList> cycles, int iterations)> Results = new();

  public static void Save() {
    var results = Results.TakeLast(Iterations)
      .Select(result => (result.cycles, result.iterations, distance: Instance.Distance[result.cycles])).ToArray();
    var best = results.MinBy(x => x.distance);
    var worst = results.MaxBy(x => x.distance);

    var averageDistance = results.Select(x => x.distance).Average();
    File.WriteAllText(Files.Distances.FullName,
      $"{best.distance}{Environment.NewLine}{worst.distance}{Environment.NewLine}{averageDistance}{Environment.NewLine}");

    var averageIterations = results.Select(x => x.iterations).Average();
    File.AppendAllText(Files.Distances.FullName,
      $"{best.iterations}{Environment.NewLine}{worst.iterations}{Environment.NewLine}{averageIterations}{Environment.NewLine}"
    );

    var plot = new ScottPlot.Plot();
    foreach (var cycle in best.cycles) plot.Add.Cycle(cycle, Instance);
    plot.Add.Label($"Łączna długość: {Instance.Distance[best.cycles]}");
    plot.Save($"best-{Instance.Name}-{SearchName}-{VariantName}-cycles");
    plot.Clear();
    // foreach (var cycle in worst.cycles) plot.Add.Cycle(cycle, Instance);
    // plot.Add.Label($"Łączna długość: {Instance.Distance[worst.cycles]}");
    // plot.Save($"worst-{Instance.Name}-{SearchName}-cycles");
    // plot.Clear();

    Results.Clear();
  }

  public static (double min, double max, double average) Load(int offset) {
    var lines = File.ReadAllLines(Files.Distances.FullName);
    var best = double.Parse(lines[offset]);
    var worst = double.Parse(lines[offset + 1]);
    var average = double.Parse(lines[offset + 2]);

    return (best, worst, average);
  }

  private static class Files {
    public static FileInfo Distances {
      get {
        if (!_distances.Exists) _distances.Create().Close();
        return _distances;
      }
    }

    private static readonly FileInfo _distances = new($"{Shared.Directories.Memory}/distances.txt");
  }
}
