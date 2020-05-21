using Milky.Enums;
using System.Collections.Generic;

namespace Milky.Models
{
    public class CheckResult
    {
        public CheckResult()
        {
        }

        public CheckResult(ComboResult comboResult)
        {
            ComboResult = comboResult;
        }

        public CheckResult(ComboResult comboResult, IDictionary<string, object> captures)
        {
            ComboResult = comboResult;
            Captures = captures;
        }

        public ComboResult ComboResult { get; set; } = ComboResult.Invalid;

        public IDictionary<string, object> Captures { get; set; } = new Dictionary<string, object>();

        public string OutputFile { get; set; }
    }
}
