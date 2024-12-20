using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChemReactionsBuilder.Models;

public class Export
{
    public double[][] Values { get; init; }
    public Reaction[] Reactions { get; init; }
    public Component[] Components { get; init; }
    public double Temperature { get; init; }
    public double Time { get; init; }
    public double Quantity { get; init; }
    public double StepTime { get; init; }

    public void ExportToXlsx(string filename)
    {
        int col = Components.Length + 3;
        Excel.Application excelAppObj = new Excel.Application();
        excelAppObj.DisplayAlerts = false;
        Excel.Workbook workBook = excelAppObj.Workbooks.Add(Missing.Value);
        Excel.Worksheet worksheet = (Excel.Worksheet)workBook.Worksheets.Add();
        worksheet.Cells[1, col] = "Реакции";
        worksheet.Cells[1, col + 2] = "Расход потока через реактор, л/мин";
        worksheet.Cells[1, col + 3] = this.Quantity.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        worksheet.Cells[2, col + 2] = "Температура смеси в реакторе, С";
        worksheet.Cells[2, col + 3] = this.Temperature.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        worksheet.Cells[3, col + 2] = "Время моделирования, мин";
        worksheet.Cells[3, col + 3] = this.Time.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        worksheet.Cells[4, col + 2] = "Шаг моделирования, мин";
        worksheet.Cells[4, col + 3] = this.StepTime;
        Excel.Range rng = worksheet.Cells[1, col] as Excel.Range;
        rng.Font.Bold = true;
        rng = worksheet.Cells[1, col + 2] as Excel.Range;
        rng.Font.Bold = true;
        rng = worksheet.Cells[2, col + 2] as Excel.Range;
        rng.Font.Bold = true;
        rng = worksheet.Cells[3, col + 2] as Excel.Range;
        rng.Font.Bold = true;
        rng = worksheet.Cells[4, col + 2] as Excel.Range;
        rng.Font.Bold = true;

        int row = 1;
        foreach (Component comp in Components)
        {
            worksheet.Cells[row, col + 5] = $"Начальная концентрация компонента {comp.Name}, моль/л";
            rng = worksheet.Cells[row, col + 5] as Excel.Range;
            rng.Font.Bold = true;
            worksheet.Cells[row, col + 6] = comp.StartConcentration.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            row++;
        }

        int rowReactions = 2;
        for (int i = 0; i < Reactions.Length; i++)
        {
            worksheet.Cells[rowReactions, col] = Reactions[i].ToString();
            rowReactions++;
        }

        col = 2;
        row = 1;
        worksheet.Cells[row, col - 1] = "Время, мин";
        rng = worksheet.Cells[row, col - 1] as Excel.Range;
        rng.Font.Bold = true;
        foreach (Component comp in Components)
        {
            worksheet.Cells[row, col] = $"C{comp.Name}, моль/л";
            rng = worksheet.Cells[row, col] as Excel.Range;
            rng.Font.Bold = true;
            col++;
        }

        row++;
        for (; row < (int)(Time / StepTime) + 1; row++)
        {
            for (col = 1; col < Values.Length + 1; col++)
            {
                worksheet.Cells[row, col] = Values[col - 1][row - 1]
                    .ToString("0.#####", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        worksheet.Columns.AutoFit();

        workBook.SaveAs(filename, Excel.XlFileFormat.xlOpenXMLWorkbook, null, null, false,
        false, Excel.XlSaveAsAccessMode.xlShared, false, false, null, null, null);
        workBook.Close(true, Missing.Value, Missing.Value);
        excelAppObj.Quit();
        Marshal.ReleaseComObject(workBook);
        Marshal.ReleaseComObject(workBook);
        Marshal.ReleaseComObject(excelAppObj);
    }
}