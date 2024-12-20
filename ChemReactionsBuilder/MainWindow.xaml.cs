using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChemReactionsBuilder.Messages;
using ChemReactionsBuilder.Models;
using ChemReactionsBuilder.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace ChemReactionsBuilder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        
        WeakReferenceMessenger.Default.Register<DataMessage>(this, (r, m) =>
        {
            InicializeTable(m.Value);
        });

    }
    
    private static readonly Regex _reg = new Regex("[^A-Z,0-9,-,>,+]+");
    private static readonly Regex _posRegDouble = new Regex("[^0-9,]+");
    private static readonly Regex _posNegDouble = new Regex("[^0-9,-]+");
    private static readonly Regex _posRegInt = new Regex("[^0-9]+");
    private static readonly Regex _posReg = new Regex("[^0-3]+");
    private static bool IsTextAllowed(string text)
    {
        return !_reg.IsMatch(text);
    }

    private static bool IsTextAllowedDouble(string text)
    {
        return !_posRegDouble.IsMatch(text);
    }
    
    private static bool IsTextAllowedNegDouble(string text)
    {
        return !_posNegDouble.IsMatch(text);
    }
    
    private static bool IsTextAllowedPosInt(string text)
    {
        return !_posRegInt.IsMatch(text);
    }
    
    private static bool IsTextAllowedInt(string text)
    {
        return !_posReg.IsMatch(text);
    }
    
    private void TextModel_Preview(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowed(e.Text);
    }

    private void CompValue_Preview(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowedDouble(e.Text);
    }
    
    private void TempValue_Preview(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowedNegDouble(e.Text);
    }
    
    private void ReactValue_Preview(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowedInt(e.Text);
    }
    
    private void PointCount_Preview(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowedPosInt(e.Text);
    }

    private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = e.Key == Key.Space;
    }
    

    private void FirstLeftCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.LeftFirst = 1;
    }

    private void SecondLeftCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.LeftSecond = 1;
    }

    private void ThirdLeftCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.LeftThird = 1;
    }

    private void FirstRightCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.RightFirst = 1;
    }

    private void SecondRightCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.RightSecond = 1;
    }

    private void ThirdRightCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reaction react = (Reaction)(sender as ComboBox)!.DataContext;
        react.RightThird = 1;
    }

    private void InicializeTable(Export export)
    {
        DataTable dt = new();
        List<string> cols = ["Время, мин"];
        foreach (var comp in export.Components)
        {
            cols.Add($"{comp.Name}, моль/л");
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