using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milky.Extensions
{
    internal static class EnumerableExtensions
    {
        public static Task ForEachAsync<TSource>(this IEnumerable<TSource> source, int partitionCount, Func<TSource, Task> body)
        {
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(partitionCount)
                .Select(partition => Task.Run(async () =>
                {
                    while (partition.MoveNext())
                    {
                        await body(partition.Current).ConfigureAwait(false);
                    }
                }))
            );
        }
    }
}
