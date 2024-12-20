using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChemReactionsBuilder.Models;

public partial class Reaction : ObservableObject
{
    protected bool Equals(Reaction other)
    {
        return _leftFirst == other._leftFirst && _leftSecond == other._leftSecond && _leftThird == other._leftThird &&
               _rightFirst == other._rightFirst && _rightSecond == other._rightSecond &&
               _rightThird == other._rightThird && Equals(LeftFirstComp, other.LeftFirstComp) &&
               Equals(LeftSecondComp, other.LeftSecondComp) && Equals(LeftThirdComp, other.LeftThirdComp) &&
               Equals(RightFirstComp, other.RightFirstComp) && Equals(RightSecondComp, other.RightSecondComp) &&
               Equals(RightThirdComp, other.RightThirdComp);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Reaction)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_leftFirst);
        hashCode.Add(_leftSecond);
        hashCode.Add(_leftThird);
        hashCode.Add(_rightFirst);
        hashCode.Add(_rightSecond);
        hashCode.Add(_rightThird);
        hashCode.Add(LeftFirstComp);
        hashCode.Add(LeftSecondComp);
        hashCode.Add(LeftThirdComp);
        hashCode.Add(RightFirstComp);
        hashCode.Add(RightSecondComp);
        hashCode.Add(RightThirdComp);
        return hashCode.ToHashCode();
    }

    [ObservableProperty] private int _leftFirst;
    public Component? LeftFirstComp { get; set; }
    [ObservableProperty] private int _leftSecond;
    public Component? LeftSecondComp { get; set; }
    [ObservableProperty] private int _leftThird;
    public Component? LeftThirdComp { get; set; }
    [ObservableProperty] private int _rightFirst;
    public Component? RightFirstComp { get; set; }
    [ObservableProperty] private int _rightSecond;
    public Component? RightSecondComp { get; set; }
    [ObservableProperty] private int _rightThird;
    public Component? RightThirdComp { get; set; }
    public double ActivationEnergy { get; set; }
    public double Multiplier { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        bool appendPlus = false;
        if (LeftFirstComp is not null && LeftFirst != 0)
        {
            builder.Append(LeftFirst == 1 ? "" : LeftFirst);
            builder.Append(LeftFirstComp.Name);
            appendPlus = true;
        }

        if (LeftSecondComp is not null && LeftSecond != 0)
        {
            if (appendPlus) builder.Append(" + ");
            builder.Append(LeftSecond == 1 ? "" : LeftSecond);
            builder.Append(LeftSecondComp.Name);
            appendPlus = true;
        }

        if (LeftThirdComp is not null && LeftThird != 0)
        {
            if (appendPlus) builder.Append(" + ");
            builder.Append(LeftThird == 1 ? "" : LeftThird);
            builder.Append(LeftThirdComp.Name);
        }

        builder.Append(" -> ");
        appendPlus = false;
        if (RightFirstComp is not null && RightFirst != 0)
        {
            builder.Append(RightFirst == 1 ? "" : RightFirst);
            builder.Append(RightFirstComp.Name);
            appendPlus = true;
        }

        if (RightSecondComp is not null && RightSecond != 0)
        {
            if (appendPlus) builder.Append(" + ");
            builder.Append(RightSecond == 1 ? "" : RightSecond);
            builder.Append(RightSecondComp.Name);
            appendPlus = true;
        }

        if (RightThirdComp is not null && RightThird != 0)
        {
            if (appendPlus) builder.Append(" + ");
            builder.Append(RightThird == 1 ? "" : RightThird);
            builder.Append(RightThirdComp.Name);
        }

        return builder.ToString();
    }

    public List<(int, Component, bool)> GetUsedComponents()
    {
        var result = new List<(int, Component, bool)>();
        if (LeftFirstComp is not null && LeftFirst != 0)
        {
            result.Add((LeftFirst, LeftFirstComp, true));
        }

        if (LeftSecondComp is not null && LeftSecond != 0)
        {
            result.Add((LeftSecond, LeftSecondComp, true));
        }

        if (LeftThirdComp is not null && LeftThird != 0)
        {
            result.Add((LeftThird, LeftThirdComp, true));
        }

        if (RightFirstComp is not null && RightFirst != 0)
        {
            result.Add((RightFirst, RightFirstComp, false));
        }

        if (RightSecondComp is not null && RightSecond != 0)
        {
            result.Add((RightSecond, RightSecondComp, false));
        }

        if (RightThirdComp is not null && RightThird != 0)
        {
            result.Add((RightThird, RightThirdComp, false));
        }

        return result;
    }

    public List<(int, Component)> GetAllComponents()
    {
        var result = new List<(int, Component)>();
        if (LeftFirstComp is not null && LeftFirst != 0)
        {
            result.Add((-LeftFirst, LeftFirstComp));
        }

        if (LeftSecondComp is not null && LeftSecond != 0)
        {
            result.Add((-LeftSecond, LeftSecondComp));
        }

        if (LeftThirdComp is not null && LeftThird != 0)
        {
            result.Add((-LeftThird, LeftThirdComp));
        }

        if (RightFirstComp is not null && RightFirst != 0)
        {
            result.Add((RightFirst, RightFirstComp));
        }

        if (RightSecondComp is not null && RightSecond != 0)
        {
            result.Add((RightSecond, RightSecondComp));
        }

        if (RightThirdComp is not null && RightThird != 0)
        {
            result.Add((RightThird, RightThirdComp));
        }

        return result;
    }

    public Reaction ShallowCopy()
    {
        return (Reaction)this.MemberwiseClone();
    }

    public Reaction DeepCopy()
    {
        Reaction other = (Reaction)this.MemberwiseClone();
        other.LeftFirstComp = this.LeftFirstComp;
        other.LeftSecondComp = this.LeftSecondComp;
        other.LeftThirdComp = this.LeftThirdComp;
        other.RightFirstComp = this.RightFirstComp;
        other.RightSecondComp = this.RightSecondComp;
        other.RightThirdComp = this.RightThirdComp;
        return other;
    }
}