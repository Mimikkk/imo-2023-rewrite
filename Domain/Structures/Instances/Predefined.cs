namespace Domain.Structures.Instances;

public sealed partial class Instance {
  public static class Predefined {
    public static readonly Instance KroA100 = Read("kroA100");
    public static readonly Instance KroA200 = Read("kroA200");

    public static readonly Instance KroB100 = Read("kroB100");
    public static readonly Instance KroB200 = Read("kroB200");
  }
}
