using System;

namespace Milky.Models
{
    public class OutputSettings
    {
        public string OutputDirectory { get; set; } = "Results";

        public bool OutputInvalids { get; set; } = false;

        public string CaptureSeparator { get; set; } = " | ";

        public ConsoleColor HitColor { get; set; } = ConsoleColor.Green;

        public ConsoleColor FreeColor { get; set; } = ConsoleColor.Cyan;

        public ConsoleColor InvalidColor { get; set; } = ConsoleColor.Red;
    }
}
