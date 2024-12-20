namespace ChemReactionsBuilder.Models;


public class ReactionCluster(List<Reaction> reactions, List<Component> components)
{
    public List<Reaction> Reactions { get; } = reactions;
    public List<Component> Components { get; } = components;
    public double Temperature { get; init; }
    public double Time { get; init; }
    public double TimeStep { get; init; }
    public double Quantity { get; init; }
}