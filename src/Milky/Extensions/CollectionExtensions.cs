using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milky.Extensions
{
    internal static class CollectionExtensions
    {
        public static T GetRandomItem<T>(this ICollection<T> source, Random random = null)
        {
            random ??= new Random();

            return source.ElementAt(random.Next(source.Count));
        }

        public static Task ForEachAsync<T>(this ICollection<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(Partitioner.Create(source).GetPartitions(maxDegreeOfParallelism)
                .Select(partition => Task.Run(async () =>
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                }))
            );
        }
    }
}
