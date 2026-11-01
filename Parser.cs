using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TuiView;

class Program
{
    static void Main()
    {
        // =========================================================
        // ДАННЫЕ В ФОРМАТЕ CSV (зашиты в код)
        // =========================================================
        string csvData =
@"id,user,email,age,role,status,comment
1,Ivan Ivanov,ivan@example.com,20,admin,active,Обычный клиент
2,Petr Petrov,petr@example.com,28,user,active,Клиент просил перезвонить
3,Nikita Nikitov,nikita@example.com,18,user,closed,Отказ передумал
4,Anna Smirnova,anna@example.com,35,manager,active,Постоянный клиент
5,Oleg Kuznetsov,oleg@example.com,41,user,blocked,Нарушение правил
6,Elena Popova,elena@example.com,22,user,active,Ожидает подтверждения
7,Dmitry Volkov,dmitry@example.com,19,user,closed,Сам отказался
8,Olga Sokolova,olga@example.com,31,admin,active,Техническая поддержка
9,Sergey Morozov,sergey@example.com,45,user,active,Повторный заказ
10,Tatiana Novikova,tatiana@example.com,26,manager,active,Новый сотрудник";

        // Парсим CSV
        string[] lines = csvData.Replace("\r\n", "\n").Split('\n');
        var rows = new List<string[]>();
        string[] header = null;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] cells = line.Split(',');
            if (header == null) header = cells;
            else rows.Add(cells);
        }

        if (header == null)
        {
            Console.WriteLine("Нет данных для вывода.");
            return;
        }

        // =========================================================
        // МЕНЮ ВЫБОРА
        // =========================================================
        Console.WriteLine("Выберите действие:");
        Console.WriteLine("1 - Вывод всей таблицы");
        Console.WriteLine("2 - Вывод структуры (колонки, типы, макс. длина, ключи)");
        Console.Write("Ваш выбор: ");

        string choice = Console.ReadLine();

        if (choice == "1")
        {
            string renderedTable = RenderTable(header, rows);
            DisplayWithPager(renderedTable);
        }
        else if (choice == "2")
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- СТРУКТУРА ТАБЛИЦЫ ---");
            sb.AppendLine($"Всего колонок: {header.Length} | Всего строк: {rows.Count}");
            sb.AppendLine(new string('-', 90));

            for (int i = 0; i < header.Length; i++)
            {
                string colName = header[i];
                string colType = DetermineType(rows, i);

                // Считаем максимальную длину значения в колонке
                int maxLen = 0;
                foreach (var row in rows)
                {
                    if (i < row.Length && row[i].Length > maxLen)
                        maxLen = row[i].Length;
                }

                // Определяем ключи по названию колонки
                string keyInfo = "";
                if (colName.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    keyInfo = " [PK] Первичный ключ";
                }
                else if (colName.EndsWith("_id", StringComparison.OrdinalIgnoreCase))
                {
                    string refTable = colName.Substring(0, colName.Length - 3) + "s";
                    keyInfo = $" [FK] Ссылается на таблицу {refTable}";
                }

                // Формируем строку (используем PadRight для ровных столбцов)
                string lineInfo = $"- {colName.PadRight(12)} | Тип: {colType.PadRight(6)} | Макс. длина: {maxLen.ToString().PadRight(3)} |{keyInfo}";
                sb.AppendLine(lineInfo);
            }

            DisplayWithPager(sb.ToString());
        }
        else
        {
            Console.WriteLine("Неверный выбор. Доступно только 1 или 2.");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    // ==========================================
    // ЛОГИКА ОПРЕДЕЛЕНИЯ ТИПА
    // ==========================================
    static string DetermineType(List<string[]> rows, int colIndex)
    {
        bool isInt = true;
        bool isFloat = true;
        bool hasData = false;

        foreach (var row in rows)
        {
            if (colIndex >= row.Length || string.IsNullOrWhiteSpace(row[colIndex]))
                continue;

            hasData = true;
            string val = row[colIndex];

            if (!int.TryParse(val, out _)) isInt = false;
            if (!double.TryParse(val.Replace('.', ','), out _)) isFloat = false;
        }

        if (!hasData) return "string";
        if (isInt) return "int";
        if (isFloat) return "float";
        return "string";
    }

    // ==========================================
    // ЛОГИКА РИСОВАНИЯ ТАБЛИЦЫ
    // ==========================================
    static string RenderTable(string[] headers, List<string[]> rows)
    {
        int colCount = headers.Length;
        int[] widths = new int[colCount];

        for (int i = 0; i < colCount; i++) widths[i] = headers[i].Length;
        foreach (var row in rows)
            for (int i = 0; i < colCount; i++)
            {
                string val = i < row.Length ? row[i] : "";
                if (val.Length > widths[i]) widths[i] = val.Length;
            }

        var sb = new StringBuilder();

        sb.AppendLine(Separator(widths));
        sb.AppendLine(RowLine(headers, widths));
        sb.AppendLine(Separator(widths));

        for (int i = 0; i < rows.Count; i++)
        {
            sb.AppendLine(RowLine(rows[i], widths));
            if (i < rows.Count - 1) sb.AppendLine(Separator(widths));
        }

        sb.AppendLine(Separator(widths));
        return sb.ToString();
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

    // ==========================================
    // PAGER (вывод по страницам)
    // ==========================================
    static void DisplayWithPager(string content)
    {
        int height = 0;
        try { height = Console.WindowHeight; } catch { }

        bool needsPager = height > 0 && content.Split('\n').Length > height - 4;

        if (!needsPager)
        {
            Console.WriteLine(content);
            return;
        }

        string pagerProgram = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "more" : "less";

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = pagerProgram,
                RedirectStandardInput = true,
                UseShellExecute = false,
            };
            using var process = Process.Start(psi);
            if (process != null)
            {
                using (var writer = process.StandardInput) writer.Write(content);
                process.WaitForExit();
                return;
            }
        }
        catch { /* Если пайпер не запустился - просто выводим */ }

        Console.WriteLine(content);
    }
}
