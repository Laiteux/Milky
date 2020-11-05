using Milky.Enums;
using System.Collections.Generic;

namespace Milky.Models
{
    public class CheckResult
    {
        public CheckResult(ComboResult comboResult)
        {
            ComboResult = comboResult;
        }

        public CheckResult(ComboResult comboResult, IDictionary<string, object> captures) : this(comboResult)
        {
            Captures = captures;
        }

        public ComboResult ComboResult { get; }

        public IDictionary<string, object> Captures { get; }

        /// <summary>
        /// Files to output the result to in the <see cref="OutputSettings.OutputDirectory"/>, ".txt" will automatically be appended to them
        /// </summary>
        public IEnumerable<string> OutputFiles { get; set; }
    }
}
