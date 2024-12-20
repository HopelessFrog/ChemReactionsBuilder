using System.Text;

namespace ChemReactionsBuilder.Models;

public class RModel
{
    public RModel(int reactionNumber, double reactionRate, List<(int, Component, bool)> components)
    {
        ReactionNumber = reactionNumber;
        ReactionRate = reactionRate;
        Components = components;
    }
    public int ReactionNumber { get; init; }
    public double ReactionRate { get; init; }
    public List<(int, Component, bool)> Components { get; init; }
    
    public double GetR()
    {
        double result = 1;
        var validComponents = Components.Where(c => c.Item3).ToList();
        foreach (var component in validComponents)
        {
            result *= Math.Pow(component.Item2.Concentration, component.Item1);
        }

        return result * ReactionRate;
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append($"{ReactionNumber}: {ReactionRate}*");
        var validComponents = Components.Where(c => c.Item3).ToList();
        foreach (var component in validComponents)
        {
            for (int i = 0; i < component.Item1; i++)
            {
                builder.Append($"{component.Item2.Name}*");
            }
        }
        return builder.ToString();
    }
}