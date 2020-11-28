using System;
using System.Collections.Generic;
using System.Linq;

namespace Milky.Utilities
{
    // Don't worry about this, I became creative
    internal class Library<T>
    {
        internal readonly List<KeyValuePair<int, T>> Items = new List<KeyValuePair<int, T>>();
        internal readonly List<int> Borrowed = new List<int>();

        private readonly object _locker = new object();
        private readonly Random _random = new Random();

        internal void Add(T item)
        {
            lock (_locker)
            {
                Items.Add(new KeyValuePair<int, T>(Items.Count, item));
            }
        }

        internal void Fill(int till)
        {
            lock (_locker)
            {
                if (Items.Count == 0)
                {
                    throw new Exception("Library contains 0 items.");
                }

                int beforeItemsCount = Items.Count;

                for (int i = 0; i < till - beforeItemsCount; i++)
                {
                    Add(Items[i].Value);
                }
            }
        }

        internal void RandomlyFill(int till)
        {
            lock (_locker)
            {
                if (Items.Count == 0)
                {
                    throw new Exception("Library contains 0 items.");
                }

                int beforeItemsCount = Items.Count;

                while (Items.Count < till)
                {
                    foreach (var item in Items
                        .GetRange(0, beforeItemsCount)
                        .OrderBy(i => _random.Next())
                        .Take(till - Items.Count))
                    {
                        Add(item.Value);
                    }
                }
            }
        }

        internal void Remove(KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                Borrowed.Remove(item.Key);
                Items.Remove(item);
            }
        }

        internal void ReplaceAll(List<T> items)
        {
            lock (_locker)
            {
                Items.Clear();
                Borrowed.Clear();

                items.ForEach(Add);
            }
        }

        internal bool TryBorrowFirst(out KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                if (Borrowed.Count == Items.Count)
                {
                    item = default;

                    return false;
                }

                item = Items.First(i => !Borrowed.Contains(i.Key));

                Borrowed.Add(item.Key);

                return true;
            }
        }

        internal bool TryBorrowRandom(out KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                if (Borrowed.Count == Items.Count)
                {
                    item = default;

                    return false;
                }

                item = Items
                    .Where(i => !Borrowed.Contains(i.Key))
                    .OrderBy(i => _random.Next())
                    .First();

                Borrowed.Add(item.Key);

                return true;
            }
        }

        internal void Return(KeyValuePair<int, T> item)
        {
            lock (_locker)
            {
                Borrowed.Remove(item.Key);
            }
        }
    }
}
