using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Hypercastle.Collections
{
    /// <summary>
    /// Provides a fixed size implementation for runtime. This is to reduce the total number of 
    /// reallocations we need to do when parsing the svg.
    ///
    /// Methods such as TrimExcess will exist, in order to shrink the collection, but you can 
    /// never reallocate and expand the Array. You must precalculate the total maximum capacity 
    /// before using the FixedList.
    /// </summary>
    public struct FixedList<T>
    {
        public T[] Collection;
        public int Length;
        public int Capacity => Collection.Length;

        public FixedList(int size)
        {
            Collection = new T[size];
            Length = 0;
        }

        public static implicit operator T[](FixedList<T> value)
        {
            return value.Collection;
        }
    }

    /// <summary>
    /// NativeFixedLists are like FixedLists, except use unmanaged memory and Unity's internal 
    /// NativeArray as its primary structure. This means allows us to allocate managed memory at 
    /// the correct size, avoiding garbage collection.
    /// </summary>
    public struct NativeFixedList<T> : IDisposable where T : unmanaged
    {
        public NativeArray<T> Collection;
        public int Length;
        public int Capacity => Collection.Length;

        internal readonly Allocator AllocationHandle;

        public NativeFixedList(int size, Allocator allocator)
        {
            Collection = new NativeArray<T>(size, allocator);
            Length = 0;
            AllocationHandle = allocator;
        }

        public void Dispose()
        {
            if (Collection.IsCreated)
            {
                Collection.Dispose();
            }
        }
    }

    // TODO: Add some basic documentation around this.
    public static class FixedListExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(this ref FixedList<T> fixedList, T item)
        {
            fixedList.Collection[fixedList.Length] = item;
            fixedList.Length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ref FixedList<T> fixedList, IEnumerable<T> collection)
        {
            foreach (var element in collection)
            {
                fixedList.Add(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimExcess<T>(this ref FixedList<T> fixedList)
        {
            if (fixedList.Length < fixedList.Capacity)
            {
                Array.Resize<T>(ref fixedList.Collection, fixedList.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this ref FixedList<T> fixedList)
        {
            var end = fixedList.Length - 1;
            var start = 0;
            while (end > start)
            {
                T temp = fixedList.Collection[end];
                fixedList.Collection[end] = fixedList.Collection[start];
                fixedList.Collection[start] = temp;
                end--;
                start++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(this ref NativeFixedList<T> fixedList, T item) where T : unmanaged
        {
            fixedList.Collection[fixedList.Length] = item;
            fixedList.Length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddRange<T>(this ref NativeFixedList<T> fixedList, 
            IEnumerable<T> collection) where T : unmanaged
        {
            foreach (var element in collection)
            {
                fixedList.Add(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimExcess<T>(this ref NativeFixedList<T> fixedList) where T : unmanaged
        {
            if (fixedList.Length < fixedList.Capacity)
            {
                var newArray = new NativeArray<T>(fixedList.Length, fixedList.AllocationHandle);

                unsafe
                {
                    UnsafeUtility.MemCpy(
                        newArray.GetUnsafePtr(),
                        fixedList.Collection.GetUnsafePtr(),
                        UnsafeUtility.SizeOf<T>() * fixedList.Length);
                }
                fixedList.Collection.Dispose();
                fixedList.Collection = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reverse<T>(this ref NativeFixedList<T> fixedList) where T : unmanaged
        {
            var end = fixedList.Length - 1;
            var start = 0;
            while (end > start)
            {
                T temp = fixedList.Collection[end];
                fixedList.Collection[end] = fixedList.Collection[start];
                fixedList.Collection[start] = temp;
                end--;
                start++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ToArray<T>(this ref NativeFixedList<T> fixedList) where T : unmanaged
        {
            return fixedList.Collection.ToArray();
        }
    }
}
