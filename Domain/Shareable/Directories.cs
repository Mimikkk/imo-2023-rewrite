namespace Domain.Shareable;

using static Path;

public static partial class Shared {
  public static class Directories {
    public static readonly string Project =
      Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName!;

    public static readonly string Resources = Combine(Project, "Resources");

    public static readonly string Instances = Combine(Resources, "Instances");

    public static readonly string Figures = Combine(Resources, "Figures");
  }
}
