namespace TimesheetsParser
{
    public record TimeSheet(DateTime Date, int SpentSeconds, string Issue, string Message, int Line);
}
