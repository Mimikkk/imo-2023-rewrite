namespace Interface.Modules;

internal sealed record Modules {
  public Modules(MainWindow self) {
    Interaction = new(self);
    Memory = new(self);
    Chart = new(self);
    Mouse = new(self);
    Panel = new(self);
  }

  public readonly MouseModule Mouse;
  public readonly MemoryModule Memory;
  public readonly ChartRendererModule Chart;
  public readonly InteractionModule Interaction;
  public readonly CyclePanelModule Panel;
}
