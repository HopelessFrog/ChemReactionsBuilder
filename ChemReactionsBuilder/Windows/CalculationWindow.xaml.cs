using ChemReactionsBuilder.Models;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;

namespace ChemReactionsBuilder.Windows;

public partial class CalculationWindow : Window
{
    public CalculationWindow(Export export)
    {
        InitializeComponent();
        DataTable dt = new();
        List<string> cols = ["¬рем€, мин"];
        foreach (var comp in export.Components)
        {
            cols.Add($"C{comp.Name}, моль/л");
        }

        for (int i = 0; i < cols.Count; i++)
        {
            DataGridTextColumn column = new();
            column.Header = cols[i];
            if (i == 0) column.Binding = new Binding(cols[i].Replace(' ', '_'));
            else column.Binding = new Binding(export.Components[i - 1].Name);
            Data.Columns.Add(column);
        }

        for (int i = 0; i < (int)(export.Time / export.StepTime); i++)
        {
            dynamic row = new ExpandoObject();
            for (int j = 0; j < cols.Count; j++)
            {
                if (j == 0) ((IDictionary<string, object>)row)[cols[j].Replace(' ', '_')] = export.Values[j][i].ToString("F3", CultureInfo.InvariantCulture);
                else ((IDictionary<string, object>)row)[export.Components[j - 1].Name] = export.Values[j][i].ToString("F3", CultureInfo.InvariantCulture);
            }
            Data.Items.Add(row);
        }
    }
}