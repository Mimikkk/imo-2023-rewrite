namespace Domain.Structures.Moves;

public interface IMove {
  public void Apply();

  public int Gain { get; }
}
