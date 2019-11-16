using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milky.Extensions
{
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Returns a random item from a provided <see cref="ICollection{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The <see cref="ICollection{T}"/> to pick a random item from</param>
        /// <param name="random"></param>
        /// <returns>A random item</returns>
        public static T GetRandomItem<T>(this ICollection<T> source, Random random = null)
        {
            random ??= new Random();

            return source.ElementAt(random.Next(source.Count));
        }

        /// <summary>
        /// Basically <see cref="Parallel.ForEach{TSource}(IEnumerable{TSource}, ParallelOptions, Action{TSource})"/> but asynchronous
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="source"><see cref="ICollection{T}"/> to iterate through</param>
        /// <param name="maxDegreeOfParallelism">Maximum simultaneous running tasks</param>
        /// <param name="body">Task</param>
        public static Task ForEachAsync<T>(this ICollection<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(
                Partitioner.Create(source).GetPartitions(maxDegreeOfParallelism)
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
