namespace ChemReactionsBuilder.Models;

public class Component
{
    protected bool Equals(Component other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Component)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static bool operator ==(Component? left, Component? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Component? left, Component? right)
    {
        return !Equals(left, right);
    }

    public string Name { get; set; }
    public double Concentration { get; set; }
    public double StartConcentration { get; set; }

    public Component ShallowCopy()
    {
        return (Component)this.MemberwiseClone();
    }
}