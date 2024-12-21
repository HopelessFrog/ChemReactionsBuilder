using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Documents;
using ChemReactionsBuilder.Helpers;
using ChemReactionsBuilder.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

namespace ChemReactionsBuilder.Extensions;

public static class Extensions
{
    private const double R = 8.31;
    private const int Diff = 273;

    public static double GetReactionRate(this ReactionCluster reaction, int index)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        return reaction.Reactions[index].Multiplier
               * Math.Pow(Math.E, -reaction.Reactions[index].ActivationEnergy / (R * (reaction.Temperature + Diff)));
    }

    public static List<RModel> GetRModels(this ReactionCluster reaction)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);
        List<RModel> models = new();
        for (int i = 0; i < reaction.Reactions.Count; i++)
        {
            models.Add(new(i, reaction.GetReactionRate(i), reaction.Reactions[i].GetUsedComponents()));
        }

        return models;
    }

    public static List<Balance> GetBalanceModels(this ReactionCluster reaction)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);
        List<Balance> models = new();
        for (int i = 0; i < reaction.Reactions.Count; i++)
        {
            models.Add(new(i, reaction.Reactions[i].GetAllComponents()));
        }

        return models;
    }

    public static double GetMatBalance(this ReactionCluster reaction, int index, List<Balance> balances,
        List<RModel> rModels)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);
        var component = reaction.Components[index];
        var balanceNumber = balances
            .Where(b => b.Components.Any(c => c.Item2 == component)).ToList();
        var models = rModels.Where(r => r.Components.Any(c => c.Item2 == component)).ToList();

        var check = balanceNumber.Join(models,
            b => b.ReactionNumber, r => r.ReactionNumber,
            (b, r) => new
            {
                R = r.GetR(),
                Uses = b.Components
                    .Where(c => c.Item2 == component)
                    .Select(c => c.Item1)
                    .Aggregate(0, (f, s) => f + s),
            }).ToList();

        var check2 = check.Aggregate((double)0, (f, s) => f + (s.Uses * s.R));
        double result = check2;
        return result;
    }

    /*public static double[][] GetPoints(this ReactionCluster reaction)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);
        List<double> y0arr = [0];
        var rModels = reaction.GetRModels();
        var balances = reaction.GetBalanceModels();
        foreach (var component in reaction.Components)
        {
            y0arr.Add(component.StartConcentration);
        }
        Vector<double> y0 = Vector<double>.Build.DenseOfEnumerable(y0arr);
        int N = (int)(reaction.Time / reaction.TimeStep);
        Func<double, Vector<double>, Vector<double>> odeSystem = (t, z) =>
        {
            double[] A = z.ToArray();

            for (int i = 0; i < reaction.Components.Count(); i++)
            {
                reaction.Components[i].Concentration = A[i + 1];
            }

            double[] array = new double[A.Length];
            array[0] = t;
            for (int i = 0; i < reaction.Components.Count; i++)
            {
                array[i + 1] = reaction.GetMatBalance(i, balances, rModels);
            }

            var vector = Vector<double>.Build.Dense(array);
            return vector;
        };
        var res =  RungeKutta.FourthOrder(y0, 0, reaction.Time, N, odeSystem);

        double[][] result = new double[reaction.Components.Count + 1][];
        for (int i = 0; i < result.Length; i++) result[i] = new double[N];
        for (int i = 0; i < N; i++)
        {
            var temp = res[i].ToList();
            for (int j = 0; j < result.Length; j++)
            {
                if (j == 0 && i != 0) result[j][i] = temp[j] / (reaction.TimeStep * i / 2);
                else result[j][i] = temp[j];
            }
        }
        return result;
    }*/
    public static double[][] GetPoints(this ReactionCluster reaction, ErrorRequest request, ErrorResult errorResult)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);

        errorResult.StepsCount = 0;
        errorResult.Error = 0;
        errorResult.Step = 0;
        errorResult.StepsCount = 0;

        errorResult.Step = reaction.TimeStep;

        List<double> y0arr = [0];
        var rModels = reaction.GetRModels();
        var balances = reaction.GetBalanceModels();
        foreach (var component in reaction.Components)
        {
            y0arr.Add(component.StartConcentration);
        }

        Vector<double> y0 = Vector<double>.Build.DenseOfEnumerable(y0arr);
        int N = (int)(reaction.Time / reaction.TimeStep);
        Func<double, Vector<double>, Vector<double>> odeSystem = (t, z) =>
        {
            double[] A = z.ToArray();

            for (int i = 0; i < reaction.Components.Count(); i++)
            {
                reaction.Components[i].Concentration = A[i + 1];
            }

            double[] array = new double[A.Length];
            array[0] = t;
            for (int i = 0; i < reaction.Components.Count; i++)
            {
                array[i + 1] = reaction.GetMatBalance(i, balances, rModels);
            }

            var vector = Vector<double>.Build.Dense(array);
            return vector;
        };
        var res = RungeKutta.FourthOrder(y0, 0, reaction.Time, N / 2, odeSystem);
        res = Calculate(y0, 0, reaction.Time, N, odeSystem, res, request, errorResult);


        double[][] result = new double[reaction.Components.Count + 1][];
        for (int i = 0; i < result.Length; i++) result[i] = new double[N];
        for (int i = 0; i < N; i++)
        {
            var temp = res[i].ToList();
            for (int j = 0; j < result.Length; j++)
            {
                if (j == 0 && i != 0) result[j][i] = temp[j] / (reaction.TimeStep * i / 2);
                else result[j][i] = temp[j];
            }
        }

        return result;
    }

    private static Vector<double>[] Calculate(Vector<double> y0,
        double start,
        double end,
        int N,
        Func<double, Vector<double>, Vector<double>> f, Vector<double>[] previus, ErrorRequest request,
        ErrorResult result)
    {
        var actual = RungeKutta.FourthOrder(y0, 0, end, N, f);



        var prev = previus.Last();
        var act = actual.Last();


        var error = CalculateLocalError(prev, act) * 100;
        result.Error = error;

        

        if (error > request.MaxError)
        {
            if (request.MaxSteps <= result.StepsCount)
            {
                throw new Exception(
                    $"Преавшена допустимое число шагов, точность не достигнута ,погрешность {Math.Round(result.Error,4)}%(цель {request.MaxError}%)");
            }
            
            result.Step = end / N;
            result.StepsCount += 1;
          actual =  Calculate(y0, 0, end, N * 2, f, actual, request, result);
        }

        return actual;
    }

    private static double CalculateLocalError(MathNet.Numerics.LinearAlgebra.Vector<double> previous, MathNet.Numerics.LinearAlgebra.Vector<double> current)
    {
        double error = 0;
        int index = 0;

        for (int i = 1; i < previous.Count; i++)
        {
            double localError = Math.Abs((current[i] - previous[i]) / current[i]);
            error = Math.Max(error, localError);

        }

        return error;
    }

    public static (List<ISeries>, Export) GetSeries(this ReactionCluster reaction, ErrorRequest request,
        ErrorResult errorResult)
    {
        ArgumentNullException.ThrowIfNull(reaction);
        ArgumentNullException.ThrowIfNull(reaction.Reactions);
        ArgumentNullException.ThrowIfNull(reaction.Components);
        var points = reaction.GetPoints(request, errorResult);
        var export = new Export()
        {
            Values = points,
            Components = reaction.Components.Select(c => c.ShallowCopy()).ToArray(),
            Reactions = reaction.Reactions.Select(r => r.DeepCopy()).ToArray(),
            Temperature = reaction.Temperature,
            Time = reaction.Time,
            Quantity = reaction.Quantity,
            StepTime = reaction.TimeStep,
        };
        var result = new List<ISeries>();
        var xs = points[0];
        for (int i = 1; i < points.Length; i++)
        {
            result.Add(new LineSeries<ObservablePoint>
            {
                Values = points[i].Select((p, index) => new ObservablePoint(xs[index], p)).ToArray(),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 0,
                IsVisibleAtLegend = true,
                Name = $"Компонент {reaction.Components[i - 1].Name}",
                Stroke = StrokeHelper.GetRandomDashStroke(3)
            });
        }

        return (result, export);
    }
}