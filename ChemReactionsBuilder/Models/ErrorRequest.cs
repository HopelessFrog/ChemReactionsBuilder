using CommunityToolkit.Mvvm.ComponentModel;

namespace ChemReactionsBuilder.Models;

public partial class ErrorRequest : ObservableObject
{
    [ObservableProperty] private double _initialStep;
    [ObservableProperty] private double _maxError;
    [ObservableProperty] private double _maxSteps;
}