using ChemReactionsBuilder.Models;

namespace ChemReactionsBuilder.Parsers;

public static class ReactionParser
{
     public static List<ParsedReaction> ParseReactions(string reactionsString)
    {
        if (string.IsNullOrWhiteSpace(reactionsString))
            throw new ArgumentException("Input string is empty or null.");

        var reactions = new List<ParsedReaction>();
        var lines = reactionsString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;

            try
            {
                var reaction = ParseReaction(trimmedLine);
                reactions.Add(reaction);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing reaction: '{trimmedLine}'. {ex.Message}");
            }
        }

        return reactions;
    }

    private static ParsedReaction ParseReaction(string reaction)
    {
        var parts = reaction.Replace(" ", "").Split("->");
        if (parts.Length != 2)
            throw new ArgumentException("Invalid reaction format. Expected 'A->B+C'.");

        var leftSide = ParseSide(parts[0]);
        var rightSide = ParseSide(parts[1]);


        var temp = 0;
        foreach (var comp in leftSide)
        {
            temp += comp.Coefficient;
        }

        if (temp > 3)
            throw new ArgumentException("Sum of the coefficients on the left side cannot be greater than 3");
            

        return new ParsedReaction()
        {
            LeftSide = leftSide,
            RightSide = rightSide
        };
    }

    private static (string Component, int Coefficient)[] ParseSide(string side)
    {
        var components = side.Split('+')
            .Select(ParseComponent)
            .ToArray();

        if (components.Length > 3)
            throw new ArgumentException("Each side of the reaction can have at most 3 components.");

        var temp = 0;

      

        // Заполнение пустых слотов, если компонентов меньше 3
        var paddedComponents = components
            .Concat(Enumerable.Repeat(("", 0), 3 - components.Length))
            .Take(3)
            .ToArray();

        return paddedComponents;
    }

    private static (string Component, int Coefficient) ParseComponent(string component)
    {
        if (string.IsNullOrWhiteSpace(component))
            throw new ArgumentException("Component cannot be empty.");

        component = component.Trim();
        int coefficient = 1;

        // Проверка на коэффициент перед компонентом
        var index = 0;
        while (index < component.Length && char.IsDigit(component[index]))
            index++;

        if (index > 0)
        {
            coefficient = int.Parse(component.Substring(0, index));
            component = component.Substring(index).Trim();
        }

        if (string.IsNullOrWhiteSpace(component))
            throw new ArgumentException("Component name is missing after coefficient.");

        return (component, coefficient);
    }
}