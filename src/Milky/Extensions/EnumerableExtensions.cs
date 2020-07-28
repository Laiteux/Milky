using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milky.Extensions
{
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Basically <see cref="Parallel.ForEach"/> but async
        /// </summary>
        internal static Task ForEachAsync<TSource>(this IEnumerable<TSource> source, int partitionCount, Func<TSource, Task> body)
        {
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(partitionCount)
                .Select(partitions => Task.Run(async () =>
                {
                    while (partitions.MoveNext())
                    {
                        await body(partitions.Current).ConfigureAwait(false);
                    }
                }))
            );
        }
    }
}
