namespace ChemReactionsBuilder.Models;

public class Balance
{
    public Balance(int reactionNumber, List<(int, Component)> components)
    {
        ReactionNumber = reactionNumber;
        Components = components;
    }
    public int ReactionNumber { get; init; }
    public List<(int, Component)> Components { get; init; }
}