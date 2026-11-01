using System;
using System.Collections.Generic;
using System.Text;

namespace TuiView;

class Program
{
    static void Main()
    {
        // ===== ДАННЫЕ В МАССИВЕ =====
        // Первая строка — заголовок, остальные — данные.
        // Можно менять содержимое — таблица подстроится.
        string[][] data =
        {
            new[] { "id", "name", "age" },
            new[] { "0", "Ivan Ivanov", "20" },
            new[] { "1", "Petr Petrov", "28" },
            new[] { "2", "Nikita Nikitov", "18" },
        };

        // ===== ПРОВЕРКА =====
        if (data.Length == 0 || data[0].Length == 0)
        {
            Console.Error.WriteLine("[error] данных нет");
            return;
        }

        string[] header = data[0];
        int colCount = header.Length;

        // Данные (без заголовка), нормализованные под число колонок
        var rows = new List<string[]>();
        for (int r = 1; r < data.Length; r++)
        {
            var cells = new string[colCount];
            for (int i = 0; i < colCount; i++)
                cells[i] = i < data[r].Length ? data[r][i] : "";
            rows.Add(cells);
        }

        // ===== ШИРИНА КОЛОНОК =====
        int[] widths = new int[colCount];
        for (int i = 0; i < colCount; i++) widths[i] = header[i].Length;
        foreach (var row in rows)
            for (int i = 0; i < colCount; i++)
                if (row[i].Length > widths[i]) widths[i] = row[i].Length;

        // ===== ПЕЧАТЬ =====
        Console.WriteLine(Separator(widths));
        Console.WriteLine(RowLine(header, widths));
        Console.WriteLine(Separator(widths));

        for (int i = 0; i < rows.Count; i++)
        {
            Console.WriteLine(RowLine(rows[i], widths));
            if (i < rows.Count - 1)
                Console.WriteLine(Separator(widths));
        }

        Console.WriteLine(Separator(widths));
    }

    static string Separator(int[] widths)
    {
        var sb = new StringBuilder();
        sb.Append('|');
        for (int i = 0; i < widths.Length; i++)
        {
            sb.Append(new string('-', widths[i] + 2));
            sb.Append('|');
        }
        return sb.ToString();
    }

    static string RowLine(string[] cells, int[] widths)
    {
        var sb = new StringBuilder();
        sb.Append('|');
        for (int i = 0; i < cells.Length; i++)
        {
            sb.Append(' ');
            sb.Append(cells[i].PadRight(widths[i]));
            sb.Append(' ');
            sb.Append('|');
        }
        return sb.ToString();
    }
}
