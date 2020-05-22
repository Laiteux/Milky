using Milky.Enums;
using System.Collections.Generic;

namespace Milky.Models
{
    public class CheckResult
    {
        public CheckResult()
        {
        }

        public CheckResult(ComboResult comboResult) : this()
        {
            ComboResult = comboResult;
        }

        public CheckResult(ComboResult comboResult, IDictionary<string, object> captures) : this(comboResult)
        {
            Captures = captures;
        }

        public ComboResult ComboResult { get; private set; } = ComboResult.Invalid;

        public IDictionary<string, object> Captures { get; private set; }

        public string OutputFile { get; set; }
    }
}
