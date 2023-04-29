using System.Collections.Immutable;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Cli.Benchmarks;

public static class Memory {
  public static readonly Instance Instance = Instance.Predefined.KroA100;
  public static readonly List<ImmutableArray<NodeList>> Results = new();
  public const int Iterations = 100;

  public static void Save() {
    var distances = Results.TakeLast(Iterations).Select(x => (double)RandomSearchTest.Instance.Distance[x]).ToList();

    var min = distances.Min();
    var max = distances.Max();
    var average = distances.Average();

    var file = new FileInfo($"{Shared.Directories.Memory}/memory.txt");
    if (!file.Exists) file.Create().Close();

    File.WriteAllText(file.FullName, $"{min}{Environment.NewLine}");
    File.AppendAllText(file.FullName, $"{max}{Environment.NewLine}");
    File.AppendAllText(file.FullName, $"{average}{Environment.NewLine}");
    Results.Clear();
  }

  public static (double min, double max, double average) Load() {
    var file = new FileInfo($"{Shared.Directories.Memory}/memory.txt");

    var lines = File.ReadAllLines(file.FullName);
    var min = double.Parse(lines[0]);
    var max = double.Parse(lines[1]);
    var average = double.Parse(lines[2]);

    return (min, max, average);
  }
}
