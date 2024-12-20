namespace ChemReactionsBuilder.Models;

public class ParsedReaction
{
    public (string Component, int Coefficient)[] LeftSide { get; set; } = new (string, int)[3];
    public (string Component, int Coefficient)[] RightSide { get; set; } = new (string, int)[3];
}