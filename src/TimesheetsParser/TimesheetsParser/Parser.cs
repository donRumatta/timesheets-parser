using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TimesheetsParser
{
    public class Parser
    {
        private string _filePath;
        private string[]? _contents;
        private DateTime _currentDate;
        private bool _weekend = false;
        private string _processedTag = "[x]";

        private Parser(string filePath)
        {
            _filePath = filePath;
        }

        public static async Task<Parser> CreateAsync(string filePath)
        {
            var parser = new Parser(filePath);

            await parser.InitAsync();

            return parser;
        }

        private async Task InitAsync()
        {
            _contents = await File.ReadAllLinesAsync(_filePath);

            var ruCulture = new CultureInfo("ru-RU");
            _currentDate = DateTime.Parse(_contents.First(), ruCulture).AddDays(-1);

            AnsiConsole.MarkupLine($"[olive]Start date: {_currentDate:d}[/]");
        }

        public async Task SetProcessed(int lineNumber)
        {
            var line = _contents[lineNumber];
            line = $"{_processedTag} {line}";
            _contents[lineNumber] = line;

            await File.WriteAllLinesAsync(_filePath, _contents);
        }

        public List<TimeSheet> Parse()
        {
            var result = new List<TimeSheet>();

            var line = 0;

            // skip month string
            foreach (var item in _contents.Skip(1))
            {
                line += 1;

                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                if (item.StartsWith(_processedTag))
                {
                    AnsiConsole.MarkupLineInterpolated($"[orangered1]SKIPPED {item}[/]");

                    continue;
                }

                if (item.StartsWith("--"))
                {
                    ProcessDayChange(item);

                    continue;
                }

                var timeSheet = ParseTimeSheetLine(item, line);

                result.Add(timeSheet);
            }

            return result;
        }

        private void ProcessDayChange(string item)
        {
            int daysToAdd = _weekend ? 3 : 1;
            _currentDate = _currentDate.AddDays(daysToAdd);

            _weekend = false;

            if (item.Contains("Пт"))
            {
                _weekend = true;
            }
        }

        private TimeSheet ParseTimeSheetLine(string item, int line)
        {
            var regex = new Regex(@"(\d+|\d+\s\d+)\s([A-Z]+-\d+)\s(.+)", RegexOptions.Compiled);
            var match = regex.Match(item);
            if (match.Success)
            {
                var time = match.Groups[1].Value;
                var issue = match.Groups[2].Value;
                var message = match.Groups[3].Value;

                var seconds = GetSecondsFromTime(time);

                return new TimeSheet(_currentDate, seconds, issue, message, line);
            }
            else
            {
                throw new FormatException($"Failed to parse string '{item}'");
            }
        }

        private int GetSecondsFromTime(string time)
        {
            // 1, 2, 3, etc
            if (time.Length == 1)
            {
                return CalculateSeconds(time, "0");
            }

            // 15, 30, 45
            if (time.Length == 2)
            {
                return CalculateSeconds("0", time);
            }

            // 1 30, 3 15, 2 45
            var timeParts = time.Split(' ');

            return CalculateSeconds(timeParts[0], timeParts[1]);
        }

        private int CalculateSeconds(string hoursString, string minutesString)
        {
            var hours = int.Parse(hoursString);
            var minutes = int.Parse(minutesString);

            return hours * 60 * 60 + minutes * 60;
        }
    }
}
