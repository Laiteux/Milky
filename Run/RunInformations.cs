using Milky.Utils;
using System;
using System.Globalization;

namespace Milky.Run
{
    public class RunInformations
    {
        private DateTimeUtils _dateTimeUtils;

        public int
            ran,
            hits,
            free;

        public long runStartUnixTimeSeconds;
        public string runStartFormattedDate;

        public RunStatus runStatus = RunStatus.Idle;

        public void SetRunStartDate()
        {
            _dateTimeUtils = DateTimeUtils.GetOrNewInstance();

            runStartUnixTimeSeconds = _dateTimeUtils.GetCurrentUnixTimeSeconds();

            string
                second = DateTime.Now.Second < 10 ? $"0{DateTime.Now.Second}" : DateTime.Now.Second.ToString(),
                minute = DateTime.Now.Minute < 10 ? $"0{DateTime.Now.Minute}" : DateTime.Now.Minute.ToString(),
                hour = DateTime.Now.Hour < 10 ? $"0{DateTime.Now.Hour}" : DateTime.Now.Hour.ToString(),
                day = DateTime.Now.Day < 10 ? $"0{DateTime.Now.Day}" : DateTime.Now.Day.ToString(),
                month = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM")),
                year = DateTime.Now.Year.ToString();

            runStartFormattedDate = $"{month} {day}, {year} - {hour}.{minute}.{second}";
        }

        public enum RunStatus
        {
            Idle,
            Running,
            Finished
        }

        private static RunInformations _classInstance;
        public static RunInformations GetOrNewInstance()
        {
            return _classInstance ?? (_classInstance = new RunInformations());
        }
    }
}