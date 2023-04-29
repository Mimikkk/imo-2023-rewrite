namespace Domain.Shareable;

using static Path;

public static partial class Shared {
  public static class Directories {
    public static readonly string Project = K();

    public static string K() {
      while (true) {
        var directory = Directory.GetCurrentDirectory();
        if (directory.EndsWith("imo-2023-rewrite")) return directory;
        Directory.SetCurrentDirectory(Directory.GetParent(directory)?.FullName!);
      }
    }

    public static readonly string Resources = Combine(Project, "Resources");

    public static readonly string Instances = Combine(Resources, "Instances");

    public static readonly string Figures = Combine(Resources, "Figures");

    public static readonly string Memory = Combine(Resources, "Memory");
  }
}
