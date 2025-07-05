using FluentAssertions;

namespace TimesheetsParser.Tests
{
    public class ParserTests
    {
        [Test]
        public async Task Parse_SuccessFlow()
        {
            var parser = await Parser.CreateAsync(@"Data\WL.txt");

            var timeSheets = parser.Parse();

            var expected = new[] 
            { 
                new TimeSheet(new DateTime(2025, 5, 5), 1800, "PLAT-60", "Дейли.", 24),
                new TimeSheet(new DateTime(2025, 5, 5), 1800, "AIHD-100", "Обсуждение НТ бота.", 25),
                new TimeSheet(new DateTime(2025, 5, 5), 5400, "AIHD-100", "Изучение документации библиотеки TelegramBot.", 26),
                new TimeSheet(new DateTime(2025, 5, 5), 7200, "PLAT-200", "Доработка юнит-тестов.", 27),
                new TimeSheet(new DateTime(2025, 5, 5), 12600, "PLAT-210", "Реализация пересоздания секрета с connection string с сохранением пароля.", 28),

                new TimeSheet(new DateTime(2025, 5, 8), 1800, "PLAT-60", "Дейли.", 32),
                new TimeSheet(new DateTime(2025, 5, 8), 5400, "PLAT-300", "Обсуждение проблемы с переопределением постфикса БД для МР, поиск обходных путей. cs-non-auto-service.", 33),
                new TimeSheet(new DateTime(2025, 5, 8), 1800, "PLAT-400", "Консультирование по работе с версиями нугет-пакетов от БП, использующих Платформу. post-processing.", 34),
                new TimeSheet(new DateTime(2025, 5, 8), 19800, "PLAT-50", "Реализация пересоздания секрета с connection string с сохранением пароля.", 35),
            };
            timeSheets.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Parse_ExactDate()
        {
            var parser = await Parser.CreateAsync(@"Data\WL.ExactDate.txt");

            var timeSheets = parser.Parse();

            var expected = new[]
            {
                new TimeSheet(new DateTime(2025, 6, 2), 1800, "PLAT-60", "Дейли.", 4)
            };
            timeSheets.Should().BeEquivalentTo(expected);
        }
    }
}
