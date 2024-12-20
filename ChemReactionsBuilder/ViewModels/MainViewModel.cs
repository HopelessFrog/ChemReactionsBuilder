using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using ChemReactionsBuilder.Extensions;
using ChemReactionsBuilder.Models;
using ChemReactionsBuilder.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Win32;
using System.Dynamic;
using System.Windows.Documents;
using ChemReactionsBuilder.Messages;
using ChemReactionsBuilder.Parsers;
using CommunityToolkit.Mvvm.Messaging;

namespace ChemReactionsBuilder.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Components = new()
        {
            new Component() { Name = "A", StartConcentration = 0.5 },
            new Component() { Name = "B" },
            new Component() { Name = "C" },
            new Component() { Name = "D" },
            new Component() { Name = "E" },
            new Component() { Name = "F" },
        };
        Reactions = new()
        {
            new Reaction()
            {
                LeftFirstComp = Components[0], LeftFirst = 1, RightFirst = 3, RightFirstComp = Components[1],
                RightSecond = 1, RightSecondComp = Components[2], Multiplier = 70000000000000000,
                ActivationEnergy = 91930
            },
            new Reaction()
            {
                LeftFirstComp = Components[1], LeftFirst = 1, RightFirst = 3, RightFirstComp = Components[3],
                RightSecond = 1, RightSecondComp = Components[4], Multiplier = 9000000000000, ActivationEnergy = 92000
            },
            new Reaction()
            {
                LeftFirstComp = Components[4], LeftFirst = 1, LeftSecondComp = Components[2], LeftSecond = 1,
                RightFirst = 1, RightFirstComp = Components[5], Multiplier = 50000000000000, ActivationEnergy = 85000
            },
        };
        
    }

    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public ErrorRequest ErrorRequest { get; set; } = new();
    public ErrorResult ErrorResult { get; set; } = new();

    [ObservableProperty] private string _mem = "";
    private DispatcherTimer _timer;
    [ObservableProperty] private string _time = "-";

    [ObservableProperty] private string _textModel = "A->3B+C\nB->3D+E\nE+C->F";

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddReactionCommand))]
    private ObservableCollection<Component> _components = new();

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(RemoveComponentCommand))]
    private Component? _selectedComponent;

    [ObservableProperty] private ObservableCollection<Reaction> _reactions = new();

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(RemoveReactionCommand))]
    private Reaction? _selectedReaction;

    [ObservableProperty] private ObservableCollection<ISeries> _series = new();

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GetSeriesCommand))]
    private double _temperature = -10;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GetSeriesCommand))]
    private double _stepTime = 1;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GetSeriesCommand))]
    private double _quantity = 100;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(GetSeriesCommand))]
    private double _allTime = 60;

    [ObservableProperty] private string _reactionString = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExportToExcelCommand))]
    [NotifyCanExecuteChangedFor(nameof(ViewDataCommand))]
    private Export? _export = null;

    public Axis[] YAxes => [new() { Name = "Концентрация компонента, моль/л" }];
    public Axis[] XAxes => [new() { Name = "Время, мин" }];

    public void UpdateReactionString()
    {
        ReactionString = string.Join('\n', Reactions.Select(r => r.ToString()));
    }

    [RelayCommand]
    private void ParseTextModel()
    {
        List<ParsedReaction> data;
        try
        {
            data = ReactionParser.ParseReactions(TextModel);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var components = new List<string>();
        var tempReact = new List<Reaction>();
        foreach (var dat in data)
        {
            foreach (var react in dat.LeftSide)
            {
                if (react.Component == "")
                    continue;
                if (Components.All(c => c.Name != react.Component))
                {
                    Components.Add(new Component() { Name = react.Component });
                }

                components.Add(react.Component);
            }

            foreach (var react in dat.RightSide)
            {
                if (react.Component == "")
                    continue;
                if (Components.All(c => c.Name != react.Component))
                {
                    Components.Add(new Component() { Name = react.Component });
                }

                components.Add(react.Component);
            }


            foreach (var reaction in data)
            {
                var value = new Reaction();
                if (reaction.LeftSide[0].Component != "")
                {
                    value.LeftFirstComp = Components.FirstOrDefault(c => c.Name == reaction.LeftSide[0].Component);
                    value.LeftFirst = reaction.LeftSide[0].Coefficient;
                }

                if (reaction.LeftSide[1].Component != "")
                {
                    value.LeftSecondComp = Components.FirstOrDefault(c => c.Name == reaction.LeftSide[1].Component);
                    value.LeftSecond = reaction.LeftSide[1].Coefficient;
                }

                if (reaction.LeftSide[2].Component != "")
                {
                    value.LeftThirdComp = Components.FirstOrDefault(c => c.Name == reaction.LeftSide[2].Component);
                    value.LeftThird = reaction.LeftSide[2].Coefficient;
                }

                if (reaction.RightSide[0].Component != "")
                {
                    value.RightFirstComp = Components.FirstOrDefault(c => c.Name == reaction.RightSide[0].Component);
                    value.RightFirst = reaction.RightSide[0].Coefficient;
                }

                if (reaction.RightSide[1].Component != "")
                {
                    value.RightSecondComp = Components.FirstOrDefault(c => c.Name == reaction.RightSide[1].Component);
                    value.RightSecond = reaction.RightSide[1].Coefficient;
                }

                if (reaction.RightSide[2].Component != "")
                {
                    value.RightThirdComp = Components.FirstOrDefault(c => c.Name == reaction.RightSide[2].Component);
                    value.RightThird = reaction.RightSide[2].Coefficient;
                }

                if (!Reactions.Contains(value))
                {
                    Reactions.Add(value);
                }

                tempReact.Add(value);
            }
        }

        var componentsToRemove = Components.Where(c => !components.Contains(c.Name)).ToList();

        foreach (var component in componentsToRemove)
        {
            Components.Remove(component);
        }

        var itemsToRemove = Reactions.Where(item => !tempReact.Contains(item)).ToList();

        foreach (var item in itemsToRemove)
        {
            Reactions.Remove(item);
        }
    }

    private bool CanRemoveComponent() => SelectedComponent is not null;

    [RelayCommand(CanExecute = nameof(CanRemoveComponent))]
    private void RemoveComponent()
    {
        if (SelectedComponent is null) return;
        Components.Remove(SelectedComponent);
        SelectedComponent = null;
        AddReactionCommand.NotifyCanExecuteChanged();
        GetSeriesCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddComponent()
    {
        int index = AllowedChars.Length > Components.Count() ? Components.Count() : AllowedChars.Length - 1;
        Components.Add(new Component() { Name = AllowedChars[index].ToString() });
        AddReactionCommand.NotifyCanExecuteChanged();
        GetSeriesCommand.NotifyCanExecuteChanged();
    }

    private bool CanAddReaction()
    {
        var can = Components.Any();
        return can;
    }

    [RelayCommand(CanExecute = nameof(CanAddReaction))]
    private void AddReaction()
    {
        Reactions.Add(new Reaction());
        GetSeriesCommand.NotifyCanExecuteChanged();
    }

    private bool CanRemoveReaction() => SelectedReaction is not null;

    [RelayCommand(CanExecute = nameof(CanRemoveReaction))]
    private void RemoveReaction()
    {
        if (SelectedReaction is null) return;
        Reactions.Remove(SelectedReaction);
        SelectedReaction = null;
        GetSeriesCommand.NotifyCanExecuteChanged();
    }

    private bool CanGetSeries =>
        Reactions.Count != 0 && Components.Count != 0 && AllTime > 1 && StepTime > 0 && Quantity > 0;

    [RelayCommand(CanExecute = nameof(CanGetSeries))]
    private void GetSeries()
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        foreach (var component in Components)
        {
            component.Concentration = 0;
        }

        var cluster = new ReactionCluster(Reactions.ToList(), Components.ToList())
        {
            Temperature = this.Temperature,
            Time = this.AllTime,
            Quantity = this.Quantity,
            TimeStep = this.StepTime,
        };
        try
        {
            (var series, Export) = cluster.GetSeries(ErrorRequest, ErrorResult);
            OnPropertyChanged(nameof(CanExport));
            WeakReferenceMessenger.Default.Send(new DataMessage(Export));
            Series = new(series);
            stopwatch.Stop();
            using Process proc = Process.GetCurrentProcess();
            MessageBox.Show(
                $"Время = {stopwatch.ElapsedMilliseconds} мс \n" +
                $"Погрешность {ErrorResult.Error} %\n" +
                $"Шаг = {ErrorResult.Step} мин\n" +
                $"RAM = {Math.Round((double)(proc.PrivateMemorySize64 / (1024 * 1024)), 2)} МБ\n" +
                $"Делений = {ErrorResult.StepsCount}",
                "",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message
                ,
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            stopwatch.Stop();
        }


        Time = stopwatch.ElapsedMilliseconds + " мс";
    }

    public bool CanExport => Export is not null;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void ExportToExcel()
    {
        if (Export is null) return;
        SaveFileDialog dlg = new SaveFileDialog();
        dlg.FileName = "Data";
        dlg.DefaultExt = ".xlsx";
        dlg.Filter = "(.xlsx)|*.xlsx";
        bool? result = dlg.ShowDialog();
        if (result == true)
        {
            try
            {
                Export.ExportToXlsx(dlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void ViewData()
    {
        if (Export is null) return;
        new CalculationWindow(Export).ShowDialog();
    }
}