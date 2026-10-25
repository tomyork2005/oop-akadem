using System.Text;

namespace LogiCore.Domain.Common;

public static class ReportTableExtensions
{
    public const string ColumnSeparator = " | ";
    public const char LineChar = '-';
    public const string EmptyTableText = "(нет данных)";

    public static string ToReportTable<T>(this IEnumerable<T> rows, string[] headers, Func<T, string[]> toCells)
    {
        if (rows == null)
        {
            throw new ArgumentNullException(nameof(rows));
        }
        if (headers == null || headers.Length == 0)
        {
            throw new ArgumentException("нужен хотя бы один заголовок", nameof(headers));
        }
        if (toCells == null)
        {
            throw new ArgumentNullException(nameof(toCells));
        }

        var lines = new List<string[]>();
        foreach (var row in rows)
        {
            var cells = toCells(row);
            if (cells.Length != headers.Length)
            {
                throw new ArgumentException($"в строке {cells.Length} ячеек, а колонок {headers.Length}");
            }
            lines.Add(cells);
        }

        var widths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++)
        {
            widths[i] = headers[i].Length;
        }
        foreach (var line in lines)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Length > widths[i])
                {
                    widths[i] = line[i].Length;
                }
            }
        }

        var sb = new StringBuilder();
        AppendRow(sb, headers, widths);
        AppendSeparator(sb, widths);
        foreach (var line in lines)
        {
            AppendRow(sb, line, widths);
        }
        if (lines.Count == 0)
        {
            sb.AppendLine(EmptyTableText);
        }
        return sb.ToString();
    }

    private static void AppendRow(StringBuilder sb, string[] cells, int[] widths)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(ColumnSeparator);
            }
            sb.Append(cells[i].PadRight(widths[i]));
        }
        sb.AppendLine();
    }

    private static void AppendSeparator(StringBuilder sb, int[] widths)
    {
        int total = ColumnSeparator.Length * (widths.Length - 1);
        for (int i = 0; i < widths.Length; i++)
        {
            total += widths[i];
        }
        sb.AppendLine(new string(LineChar, total));
    }
}
