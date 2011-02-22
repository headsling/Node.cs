using System;
using System.Collections.Generic;
using System.Collections;
using NodeCS.Exceptions;

namespace NodeCS
{
    public class Heap<T, P> 
    {
        private HeapItem[] heapEntries;
        private T[] items;
        private Queue<int> freeList;

        private int capacity;
        private int count;
        
        private const int minCapacity = 16;
        
        private Comparison<P> compareFunc;
        

        public Heap( ) : this( Comparer<P>.Default )
        {
        }

        public Heap( IComparer<P> comparer )
        {
            Init(minCapacity, new Comparison<P>(comparer.Compare));
        }

        public Heap( Comparison<P> comparison ): this( minCapacity, comparison)
        {
        }

        public Heap(int initialCapacity, Comparison<P> comparison)
        {
            Init(initialCapacity, comparison);
        }

        private void Init(int initialCapacity, Comparison<P> comparison)
        {
            count = 0;
            compareFunc = comparison;
            setCapacity(initialCapacity);

        }

        public int Count
        {
            get { return count; }
        }

        public int Capacity
        {
            get { return heapEntries.Length; }
            set { setCapacity(value); }
        }

        private void setCapacity(int capacity )
        {
            int originalCap = this.capacity;

            if (capacity < minCapacity)
                capacity = minCapacity;
            
            Preconditions.IsTrue<ArgumentOutOfRangeException>( capacity > count, "New capacity is less than Count" );

            this.capacity = capacity;
            if (heapEntries == null)
            {
                freeList = new Queue<int>( capacity );
                heapEntries = new HeapItem[capacity];
                items = new T[capacity];

                for (int i = 0; i < capacity; i++) freeList.Enqueue(i);
                
                return;
            }

            Array.Resize<HeapItem>(ref heapEntries, capacity);
            Array.Resize<T>( ref items, capacity );

            for( int i = originalCap; i < capacity; i++ ) freeList.Enqueue( i );
        }

        /// <summary>
        /// Clears the item at the given entry (as returned by <seealso cref="Push"/>)
        /// without causing a reheap, the slot for this item will still exist in the heap
        /// and will be returned at the appropriate Pop
        /// </summary>
        /// <param name="entry">
        /// A <see cref="System.Int32"/>
        /// </param>
        public void ClearItem( int entry )
        {
            items[entry] = default(T);
        }

        private void reheap ( int index )
        {
            HeapItem tmp = heapEntries[count];
            heapEntries[count] = default(HeapItem);

            if (count == 0 || index == count ) return;

            int i = index;
            int parent = (i - 1) / 2;
            while (compareFunc(tmp.Priority, heapEntries[parent].Priority) > 0)
            {
                heapEntries[i] = heapEntries[parent];
                i = parent;
                parent = (i - 1) / 2;
            }
            
            if( i == index )
            {
                while (i < (count) / 2)
                {
                    int j = (2 * i) + 1;
                    if ((j < count - 1) &&
                        (compareFunc(heapEntries[j].Priority, heapEntries[j + 1].Priority) < 0)) ++j;

                    if (compareFunc(heapEntries[j].Priority, tmp.Priority) <= 0) break;

                    heapEntries[i] = heapEntries[j];
                    i = j;
                }
            }
            heapEntries[i] = tmp;
        }

        private T removeAt(int index)
        {
            HeapItem item = heapEntries[index];
            --count;

            reheap( index );
            T value = items[item.Index];

            items[item.Index] = default(T);

            freeList.Enqueue( item.Index );

            return value;
        }

        public bool Verify()
        {
            int i = 0;
            while (i < count / 2)
            {
                int leftChild = (2 * i) + 1;
                int rightChild = leftChild + 1;
                if (compareFunc(heapEntries[i].Priority, heapEntries[leftChild].Priority) < 0)
                {
                    return false;
                }
                if (rightChild < count && compareFunc(heapEntries[i].Priority, heapEntries[rightChild].Priority) < 0)
                {
                    return false;
                }
                ++i;
            }
            return true;
        }

        public int Push( T item, P priority )
        {
            if (count == capacity) setCapacity((3 * Capacity) / 2);

            int i = count++;

            while ((i > 0) && (compareFunc(heapEntries[(i - 1) / 2].Priority, priority) < 0))
            {
                heapEntries[i] = heapEntries[(i - 1) / 2];
                i = (i - 1) / 2;
            }

            int entry = freeList.Dequeue();

            var heapItem = new HeapItem( entry, priority );

            heapEntries[i] = heapItem;

            items[entry] = item;

            return i;
        }

        public T Pop()
        {
            Preconditions.IsTrue<InvalidOperationException>( Count > 0, "Nothing to pop" );
            
            return removeAt(0);
        }

        /// <summary>
        ///returns the priority at the head of the heap
        /// </summary>
        /// <returns>
        /// A <see cref="P"/>
        /// </returns>
        public P PeekPriority()
        {
            Preconditions.IsTrue<InvalidOperationException>( Count > 0, "Heap is empty" );

            return heapEntries[0].Priority;
        }

        /// <summary>
        ///returns the value at the head of the heap
        /// </summary>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        public T Peek()
        {
            Preconditions.IsTrue<InvalidOperationException>( Count > 0, "Heap is empty" );

            return items[heapEntries[0].Index];
        }

        public void Clear()
        {
            for (int i = 0; i < count; ++i)
            {
                heapEntries[i] = default(HeapItem);
                items[i] = default(T);
            }
            count = 0;
        }

        struct HeapItem
        {
            public readonly int Index;
            public readonly P Priority; 

            public HeapItem( int index, P priority )
            {
                Index = index;
                Priority = priority;
            }
        }  }
}

