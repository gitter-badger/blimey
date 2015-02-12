// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ___________                      .___       __  .__                    │ \\
// │ \_   _____/___  __ __  ____    __| _/____ _/  |_|__| ____   ____       │ \\
// │  |    __)/  _ \|  |  \/    \  / __ |\__  \\   __\  |/  _ \ /    \      │ \\
// │  |     \(  <_> )  |  /   |  \/ /_/ | / __ \|  | |  (  <_> )   |  \     │ \\
// │  \___  / \____/|____/|___|  /\____ |(____  /__| |__|\____/|___|  /     │ \\
// │      \/                   \/      \/     \/                    \/      │ \\
// │                                                                        │ \\
// │ Implementations of various useful features not found in all flavours   │ \\
// │ of .NET, for example System.Collections.Generic.HashSet is not         │ \\
// │ available in the .NET Compact Framework; most implementations are      │ \\
// │ pulled straight from the Mono project.                                 │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\


#if NET_CF || NETFX_CORE || MONOTOUCH

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;

//
// HashSet.cs
//
// Authors:
//  Jb Evain  <jbevain@novell.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// HashSet is basically implemented as a reduction of Dictionary<K, V>

namespace System.Collections.Generic {

    // [Serializable, HostProtection (SecurityAction.LinkDemand, MayLeakOnAbort = true)] AJP
    [DebuggerDisplay ("Count={Count}")]
    //[DebuggerTypeProxy (typeof (CollectionDebuggerView<,>))] AJP
    public class HashSet<T> 
        : ICollection<T>
        //, ISerializable AJP
        //, IDeserializationCallback AJP
#if NET_4_0 || MOONLIGHT || MOBILE
                            , ISet<T>
#endif
    {
        const int INITIAL_SIZE = 10;
        const float DEFAULT_LOAD_FACTOR = (90f / 100);
        const int NO_SLOT = -1;
        const int HASH_FLAG = -2147483648;

        struct Link {
            public int HashCode;
            public int Next;
        }

        // The hash table contains indices into the "links" array
        int [] table;

        Link [] links;
        T [] slots;

        // The number of slots in "links" and "slots" that
        // are in use (i.e. filled with data) or have been used and marked as
        // "empty" later on.
        int touched;

        // The index of the first slot in the "empty slots chain".
        // "Remove ()" prepends the cleared slots to the empty chain.
        // "Add ()" fills the first slot in the empty slots chain with the
        // added item (or increases "touched" if the chain itself is empty).
        int empty_slot;

        // The number of items in this set.
        int count;

        // The number of items the set can hold without
        // resizing the hash table and the slots arrays.
        int threshold;

        IEqualityComparer<T> comparer;
        //SerializationInfo si; AJP

        // The number of changes made to this set. Used by enumerators
        // to detect changes and invalidate themselves.
        int generation;

        public int Count {
            get { return count; }
        }

        public HashSet ()
        {
            Init (INITIAL_SIZE, null);
        }

        public HashSet (IEqualityComparer<T> comparer)
        {
            Init (INITIAL_SIZE, comparer);
        }

        public HashSet (IEnumerable<T> collection) : this (collection, null)
        {
        }

        public HashSet (IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException ("collection");

            int capacity = 0;
            var col = collection as ICollection<T>;
            if (col != null)
                capacity = col.Count;

            Init (capacity, comparer);
            foreach (var item in collection)
                Add (item);
        }

        /* AJP
        protected HashSet (SerializationInfo info, StreamingContext context)
        {
            si = info;
        }
        */

        void Init (int capacity, IEqualityComparer<T> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException ("capacity");

            this.comparer = comparer ?? EqualityComparer<T>.Default;
            if (capacity == 0)
                capacity = INITIAL_SIZE;

            /* Modify capacity so 'capacity' elements can be added without resizing */
            capacity = (int) (capacity / DEFAULT_LOAD_FACTOR) + 1;

            InitArrays (capacity);
            generation = 0;
        }

        void InitArrays (int size)
        {
            table = new int [size];

            links = new Link [size];
            empty_slot = NO_SLOT;

            slots = new T [size];
            touched = 0;

            threshold = (int) (table.Length * DEFAULT_LOAD_FACTOR);
            if (threshold == 0 && table.Length > 0)
                threshold = 1;
        }

        bool SlotsContainsAt (int index, int hash, T item)
        {
            int current = table [index] - 1;
            while (current != NO_SLOT) {
                Link link = links [current];
                if (link.HashCode == hash && ((hash == HASH_FLAG && (item == null || null == slots [current])) ? (item == null && null == slots [current]) : comparer.Equals (item, slots [current])))
                    return true;

                current = link.Next;
            }

            return false;
        }

        public void CopyTo (T [] array)
        {
            CopyTo (array, 0, count);
        }
        
        public void CopyTo (T [] array, int arrayIndex)
        {
            CopyTo (array, arrayIndex, count);
        }

        public void CopyTo (T [] array, int arrayIndex, int count)
        {
            if (array == null)
                throw new ArgumentNullException ("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException ("arrayIndex");
            if (arrayIndex > array.Length)
                throw new ArgumentException ("index larger than largest valid index of array");
            if (array.Length - arrayIndex < count)
                throw new ArgumentException ("Destination array cannot hold the requested elements!");

            for (int i = 0, items = 0; i < touched && items < count; i++) {
                if (GetLinkHashCode (i) != 0)
                    array [arrayIndex++] = slots [i];
            }
        }

        void Resize ()
        {
            int newSize = PrimeHelper.ToPrime ((table.Length << 1) | 1);

            // allocate new hash table and link slots array
            var newTable = new int [newSize];
            var newLinks = new Link [newSize];

            for (int i = 0; i < table.Length; i++) {
                int current = table [i] - 1;
                while (current != NO_SLOT) {
                    int hashCode = newLinks [current].HashCode = GetItemHashCode (slots [current]);
                    int index = (hashCode & int.MaxValue) % newSize;
                    newLinks [current].Next = newTable [index] - 1;
                    newTable [index] = current + 1;
                    current = links [current].Next;
                }
            }

            table = newTable;
            links = newLinks;

            // allocate new data slots, copy data
            var newSlots = new T [newSize];
            Array.Copy (slots, 0, newSlots, 0, touched);
            slots = newSlots;

            threshold = (int) (newSize * DEFAULT_LOAD_FACTOR);
        }

        int GetLinkHashCode (int index)
        {
            return links [index].HashCode & HASH_FLAG;
        }

        int GetItemHashCode (T item)
        {
            if (item == null)
                return HASH_FLAG;
            return comparer.GetHashCode (item) | HASH_FLAG;
        }

        public bool Add (T item)
        {
            int hashCode = GetItemHashCode (item);
            int index = (hashCode & int.MaxValue) % table.Length;

            if (SlotsContainsAt (index, hashCode, item))
                return false;

            if (++count > threshold) {
                Resize ();
                index = (hashCode & int.MaxValue) % table.Length;
            }

            // find an empty slot
            int current = empty_slot;
            if (current == NO_SLOT)
                current = touched++;
            else
                empty_slot = links [current].Next;

            // store the hash code of the added item,
            // prepend the added item to its linked list,
            // update the hash table
            links [current].HashCode = hashCode;
            links [current].Next = table [index] - 1;
            table [index] = current + 1;

            // store item
            slots [current] = item;

            generation++;

            return true;
        }

        public IEqualityComparer<T> Comparer {
            get { return comparer; }
        }

        public void Clear ()
        {
            count = 0;

            Array.Clear (table, 0, table.Length);
            Array.Clear (slots, 0, slots.Length);
            Array.Clear (links, 0, links.Length);

            // empty the "empty slots chain"
            empty_slot = NO_SLOT;

            touched = 0;
            generation++;
        }

        public bool Contains (T item)
        {
            int hashCode = GetItemHashCode (item);
            int index = (hashCode & int.MaxValue) % table.Length;

            return SlotsContainsAt (index, hashCode, item);
        }

        public bool Remove (T item)
        {
            // get first item of linked list corresponding to given key
            int hashCode = GetItemHashCode (item);
            int index = (hashCode & int.MaxValue) % table.Length;
            int current = table [index] - 1;

            // if there is no linked list, return false
            if (current == NO_SLOT)
                return false;

            // walk linked list until right slot (and its predecessor) is
            // found or end is reached
            int prev = NO_SLOT;
            do {
                Link link = links [current];
                if (
                    link.HashCode == hashCode && 
                    ((hashCode == HASH_FLAG && (item == null || null == slots [current])) 
                        ? (item == null && null == slots [current]) 
                        : comparer.Equals (slots [current], item)))
                    break;

                prev = current;
                current = link.Next;
            } while (current != NO_SLOT);

            // if we reached the end of the chain, return false
            if (current == NO_SLOT)
                return false;

            count--;

            // remove slot from linked list
            // is slot at beginning of linked list?
            if (prev == NO_SLOT)
                table [index] = links [current].Next + 1;
            else
                links [prev].Next = links [current].Next;

            // mark slot as empty and prepend it to "empty slots chain"
            links [current].Next = empty_slot;
            empty_slot = current;

            // clear slot
            links [current].HashCode = 0;
            slots [current] = default (T);

            generation++;

            return true;
        }

        public int RemoveWhere (Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException ("match");

            var candidates = new List<T> ();

            foreach (var item in this)
                if (match (item)) 
                    candidates.Add (item);

            foreach (var item in candidates)
                Remove (item);

            return candidates.Count;
        }

        public void TrimExcess ()
        {
            Resize ();
        }

        // set operations

        public void IntersectWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            var other_set = ToSet (other);

            RemoveWhere (item => !other_set.Contains (item));
        }

        public void ExceptWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in other)
                Remove (item);
        }

        public bool Overlaps (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in other)
                if (Contains (item))
                    return true;

            return false;
        }

        public bool SetEquals (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            var other_set = ToSet (other);

            if (count != other_set.Count)
                return false;

            foreach (var item in this)
                if (!other_set.Contains (item))
                    return false;

            return true;
        }

        public void SymmetricExceptWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in ToSet (other))
                if (!Add (item))
                    Remove (item);
        }

        HashSet<T> ToSet (IEnumerable<T> enumerable)
        {
            var set = enumerable as HashSet<T>;
            if (set == null || !Comparer.Equals (set.Comparer))
                set = new HashSet<T> (enumerable);

            return set;
        }

        public void UnionWith (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in other)
                Add (item);
        }

        bool CheckIsSubsetOf (HashSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in this)
                if (!other.Contains (item))
                    return false;

            return true;
        }

        public bool IsSubsetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            if (count == 0)
                return true;

            var other_set = ToSet (other);

            if (count > other_set.Count)
                return false;

            return CheckIsSubsetOf (other_set);
        }

        public bool IsProperSubsetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            if (count == 0)
                return true;

            var other_set = ToSet (other);

            if (count >= other_set.Count)
                return false;

            return CheckIsSubsetOf (other_set);
        }

        bool CheckIsSupersetOf (HashSet<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            foreach (var item in other)
                if (!Contains (item))
                    return false;

            return true;
        }

        public bool IsSupersetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            var other_set = ToSet (other);

            if (count < other_set.Count)
                return false;

            return CheckIsSupersetOf (other_set);
        }

        public bool IsProperSupersetOf (IEnumerable<T> other)
        {
            if (other == null)
                throw new ArgumentNullException ("other");

            var other_set = ToSet (other);

            if (count <= other_set.Count)
                return false;

            return CheckIsSupersetOf (other_set);
        }

        class HashSetEqualityComparer : IEqualityComparer<HashSet<T>>
        {
            public bool Equals (HashSet<T> lhs, HashSet<T> rhs)
            {
                if (lhs == rhs)
                    return true;

                if (lhs == null || rhs == null || lhs.Count != rhs.Count)
                    return false;

                foreach (var item in lhs)
                    if (!rhs.Contains (item))
                        return false;

                return true;
            }

            public int GetHashCode (HashSet<T> hashset)
            {
                if (hashset == null)
                    return 0;

                IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
                int hash = 0;
                foreach (var item in hashset)
                    hash ^= comparer.GetHashCode (item);

                return hash;
            }
        }

        static readonly HashSetEqualityComparer setComparer = new HashSetEqualityComparer ();

        public static IEqualityComparer<HashSet<T>> CreateSetComparer ()
        {
            return setComparer;
        }

        /* AJP
        [SecurityPermission (SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if (info == null) {
                throw new ArgumentNullException("info");
            }
            info.AddValue("Version", generation);
            info.AddValue("Comparer", comparer, typeof(IEqualityComparer<T>));
            info.AddValue("Capacity", (table == null) ? 0 : table.Length);
            if (table != null) {
                T[] tableArray = new T[table.Length];
                CopyTo(tableArray);
                info.AddValue("Elements", tableArray, typeof(T[]));
            }
        }

        public virtual void OnDeserialization (object sender)
        {
            if (si != null)
            {
                generation = (int) si.GetValue("Version", typeof(int));
                comparer = (IEqualityComparer<T>) si.GetValue("Comparer", 
                                          typeof(IEqualityComparer<T>));
                int capacity = (int) si.GetValue("Capacity", typeof(int));

                empty_slot = NO_SLOT;
                if (capacity > 0) {
                    table = new int[capacity];
                    slots = new T[capacity];

                    T[] tableArray = (T[]) si.GetValue("Elements", typeof(T[]));
                    if (tableArray == null) 
                        throw new SerializationException("Missing Elements");

                    for (int iElement = 0; iElement < tableArray.Length; iElement++) {
                        Add(tableArray[iElement]);
                    }
                } else 
                    table = null;

                si = null;
            }
        }
        */

        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return new Enumerator (this);
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        void ICollection<T>.Add (T item)
        {
            Add (item);
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return new Enumerator (this);
        }

        public Enumerator GetEnumerator ()
        {
            return new Enumerator (this);
        }

        //[Serializable] AJP
        public struct Enumerator : IEnumerator<T>, IDisposable {

            HashSet<T> hashset;
            int next;
            int stamp;

            T current;

            internal Enumerator (HashSet<T> hashset)
                : this ()
            {
                this.hashset = hashset;
                this.stamp = hashset.generation;
            }

            public bool MoveNext ()
            {
                CheckState ();

                if (next < 0)
                    return false;

                while (next < hashset.touched) {
                    int cur = next++;
                    if (hashset.GetLinkHashCode (cur) != 0) {
                        current = hashset.slots [cur];
                        return true;
                    }
                }

                next = NO_SLOT;
                return false;
            }

            public T Current {
                get { return current; }
            }

            object IEnumerator.Current {
                get {
                    CheckState ();
                    if (next <= 0)
                        throw new InvalidOperationException ("Current is not valid");
                    return current;
                }
            }

            void IEnumerator.Reset ()
            {
                CheckState ();
                next = 0;
            }

            public void Dispose ()
            {
                hashset = null;
            }

            void CheckState ()
            {
                if (hashset == null)
                    throw new ObjectDisposedException (null);
                if (hashset.generation != stamp)
                    throw new InvalidOperationException ("HashSet have been modified while it was iterated over");
            }
        }

        // borrowed from System.Collections.HashTable
        static class PrimeHelper {

            static readonly int [] primes_table = {
                11,
                19,
                37,
                73,
                109,
                163,
                251,
                367,
                557,
                823,
                1237,
                1861,
                2777,
                4177,
                6247,
                9371,
                14057,
                21089,
                31627,
                47431,
                71143,
                106721,
                160073,
                240101,
                360163,
                540217,
                810343,
                1215497,
                1823231,
                2734867,
                4102283,
                6153409,
                9230113,
                13845163
            };

            static bool TestPrime (int x)
            {
                if ((x & 1) != 0) {
                    int top = (int) Math.Sqrt (x);

                    for (int n = 3; n < top; n += 2) {
                        if ((x % n) == 0)
                            return false;
                    }

                    return true;
                }

                // There is only one even prime - 2.
                return x == 2;
            }

            static int CalcPrime (int x)
            {
                for (int i = (x & (~1)) - 1; i < Int32.MaxValue; i += 2)
                    if (TestPrime (i))
                        return i;

                return x;
            }

            public static int ToPrime (int x)
            {
                for (int i = 0; i < primes_table.Length; i++)
                    if (x <= primes_table [i])
                        return primes_table [i];

                return CalcPrime (x);
            }
        }
    }
}


#endif



// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

#if NET_CF || NETFX_CORE

using System.Runtime.InteropServices;

//
// System.ICloneable.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System {
    [ComVisible(true)]
#if INSIDE_CORLIB
    public
#else
    internal
#endif
    interface ICloneable {
        object Clone ();
    }
}


#endif



// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

#if NET_CF

//
// System.Runtime.Serialization.ISerializable.cs
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
//
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Runtime.Serialization {
        [System.Runtime.InteropServices.ComVisibleAttribute (true)]

    public interface ISerializable {
        void GetObjectData (SerializationInfo info, StreamingContext context);
    }
}


#endif



// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

#if NET_CF || NETFX_CORE || MONOTOUCH

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Diagnostics;

// 
// System.Collections.Generic.SortedList.cs
// 
// Author:
//   Sergey Chaban (serge@wildwestsoftware.com)
//   Duncan Mak (duncan@ximian.com)
//   Herve Poussineau (hpoussineau@fr.st
//   Zoltan Varga (vargaz@gmail.com)
// 

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Collections.Generic
{
    /// <summary>
    ///  Represents a collection of associated keys and values
    ///  that are sorted by the keys and are accessible by key
    ///  and by index.
    /// </summary>
    //[Serializable] AJP
    [ComVisible(false)]
    [DebuggerDisplay ("Count={Count}")]
    //[DebuggerTypeProxy (typeof (CollectionDebuggerView<,>))]
    public class SortedList<TKey, TValue> : IDictionary<TKey, TValue>, 
        IDictionary,
        ICollection,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable {

        readonly static int INITIAL_SIZE = 16;

        enum EnumeratorMode : int { KEY_MODE = 0, VALUE_MODE, ENTRY_MODE }

        int inUse;
        int modificationCount;
        KeyValuePair<TKey, TValue>[] table;
        IComparer<TKey> comparer;
        int defaultCapacity;

        //
        // Constructors
        //
        public SortedList () 
            : this (INITIAL_SIZE, null)
        {
        }

        public SortedList (int capacity)
            : this (capacity, null)
        {
        }

        public SortedList (int capacity, IComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException ("initialCapacity");

            if (capacity == 0)
                defaultCapacity = 0;
            else
                defaultCapacity = INITIAL_SIZE;
            Init (comparer, capacity, true);
        }

        public SortedList (IComparer<TKey> comparer) : this (INITIAL_SIZE, comparer)
        {
        }

        public SortedList (IDictionary<TKey, TValue> dictionary) : this (dictionary, null)
        {
        }

        public SortedList (IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            if (dictionary == null)
                throw new ArgumentNullException ("dictionary");

            Init (comparer, dictionary.Count, true);

            foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
                Add (kvp.Key, kvp.Value);
        }

        //
        // Properties
        //

        // ICollection

        public int Count {
            get {
                return inUse;
            }
        }

        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }

        Object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        // IDictionary

        bool IDictionary.IsFixedSize {
            get {
                return false;
            }
        }

        bool IDictionary.IsReadOnly {
            get {
                return false;
            }
        }

        public TValue this [TKey key] {
            get {
                if (key == null)
                    throw new ArgumentNullException("key");

                int i = Find (key);

                if (i >= 0)
                    return table [i].Value;
                else
                    throw new KeyNotFoundException ();
            }
            set {
                if (key == null)
                    throw new ArgumentNullException("key");

                PutImpl (key, value, true);
            }
        }

        object IDictionary.this [object key] {
            get {
                if (!(key is TKey))
                    return null;
                else
                    return this [(TKey)key];
            }

            set {
                this [ToKey (key)] = ToValue (value);
            }
        }

        public int Capacity {
            get {
                return table.Length;
            }

            set {
                int current = this.table.Length;

                if (inUse > value) {
                    throw new ArgumentOutOfRangeException("capacity too small");
                }
                else if (value == 0) {
                    // return to default size
                    KeyValuePair<TKey, TValue> [] newTable = new KeyValuePair<TKey, TValue> [defaultCapacity];
                    Array.Copy (table, newTable, inUse);
                    this.table = newTable;
                }
#if NET_1_0
                else if (current > defaultCapacity && value < current) {
                    KeyValuePair<TKey, TValue> [] newTable = new KeyValuePair<TKey, TValue> [defaultCapacity];
                    Array.Copy (table, newTable, inUse);
                    this.table = newTable;
                }
#endif
                else if (value > inUse) {
                    KeyValuePair<TKey, TValue> [] newTable = new KeyValuePair<TKey, TValue> [value];
                    Array.Copy (table, newTable, inUse);
                    this.table = newTable;
                }
                else if (value > current) {
                    KeyValuePair<TKey, TValue> [] newTable = new KeyValuePair<TKey, TValue> [value];
                    Array.Copy (table, newTable, current);
                    this.table = newTable;
                }
            }
        }

        public IList<TKey> Keys {
            get { 
                return new ListKeys (this);
            }
        }

        public IList<TValue> Values {
            get {
                return new ListValues (this);
            }
        }

        ICollection IDictionary.Keys {
            get {
                return new ListKeys (this);
            }
        }

        ICollection IDictionary.Values {
            get {
                return new ListValues (this);
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys {
            get { 
                return Keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values {
            get {
                return Values;
            }
        }

        public IComparer<TKey> Comparer {
            get {
                return comparer;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get {
                return false;
            }
        }

        //
        // Public instance methods.
        //

        public void Add (TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException ("key");

            PutImpl (key, value, false);
        }

        public bool ContainsKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException ("key");

            return (Find (key) >= 0);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator ()
        {
            for (int i = 0; i < inUse; i ++) {
                KeyValuePair<TKey, TValue> current = this.table [i];

                yield return new KeyValuePair<TKey, TValue> (current.Key, current.Value);
            }
        }

        public bool Remove (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException ("key");

            int i = IndexOfKey (key);
            if (i >= 0) {
                RemoveAt (i);
                return true;
            }
            else
                return false;
        }

        // ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Clear () 
        {
            defaultCapacity = INITIAL_SIZE;
            this.table = new KeyValuePair<TKey, TValue> [defaultCapacity];
            inUse = 0;
            modificationCount++;
        }

        public void Clear () 
        {
            defaultCapacity = INITIAL_SIZE;
            this.table = new KeyValuePair<TKey, TValue> [defaultCapacity];
            inUse = 0;
            modificationCount++;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (Count == 0)
                return;
            
            if (null == array)
                throw new ArgumentNullException();

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            
            if (arrayIndex >= array.Length)
                throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
            if (Count > (array.Length - arrayIndex))
                throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");

            int i = arrayIndex;
            foreach (KeyValuePair<TKey, TValue> pair in this)
                array [i++] = pair;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> keyValuePair) {
            Add (keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> keyValuePair) {
            int i = Find (keyValuePair.Key);

            if (i >= 0)
                return Comparer<KeyValuePair<TKey, TValue>>.Default.Compare (table [i], keyValuePair) == 0;
            else
                return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> keyValuePair) {
            int i = Find (keyValuePair.Key);

            if (i >= 0 && (Comparer<KeyValuePair<TKey, TValue>>.Default.Compare (table [i], keyValuePair) == 0)) {
                RemoveAt (i);
                return true;
            }
            else
                return false;
        }

        // IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
        {
            for (int i = 0; i < inUse; i ++) {
                KeyValuePair<TKey, TValue> current = this.table [i];

                yield return new KeyValuePair<TKey, TValue> (current.Key, current.Value);
            }
        }

        // IEnumerable

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        // IDictionary

        void IDictionary.Add (object key, object value)
        {
            PutImpl (ToKey (key), ToValue (value), false);
        }

        bool IDictionary.Contains (object key)
        {
            if (null == key)
                throw new ArgumentNullException();
            if (!(key is TKey))
                return false;

            return (Find ((TKey)key) >= 0);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator ()
        {
            return new Enumerator (this, EnumeratorMode.ENTRY_MODE);
        }

        void IDictionary.Remove (object key)
        {
            if (null == key)
                throw new ArgumentNullException ("key");
            if (!(key is TKey))
                return;
            int i = IndexOfKey ((TKey)key);
            if (i >= 0) RemoveAt (i);
        }

        // ICollection

        void ICollection.CopyTo (Array array, int arrayIndex)
        {
            if (Count == 0)
                return;
            
            if (null == array)
                throw new ArgumentNullException();

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            
            if (array.Rank > 1)
                throw new ArgumentException("array is multi-dimensional");
            if (arrayIndex >= array.Length)
                throw new ArgumentNullException("arrayIndex is greater than or equal to array.Length");
            if (Count > (array.Length - arrayIndex))
                throw new ArgumentNullException("Not enough space in array from arrayIndex to end of array");

            IEnumerator<KeyValuePair<TKey,TValue>> it = GetEnumerator ();
            int i = arrayIndex;

            while (it.MoveNext ()) {
                array.SetValue (it.Current, i++);
            }
        }

        //
        // SortedList<TKey, TValue>
        //

        public void RemoveAt (int index)
        {
            KeyValuePair<TKey, TValue> [] table = this.table;
            int cnt = Count;
            if (index >= 0 && index < cnt) {
                if (index != cnt - 1) {
                    Array.Copy (table, index+1, table, index, cnt-1-index);
                } else {
                    table [index] = default (KeyValuePair <TKey, TValue>);
                }
                --inUse;
                ++modificationCount;
            } else {
                throw new ArgumentOutOfRangeException("index out of range");
            }
        }

        public int IndexOfKey (TKey key)
        {
            if (key == null)
                throw new ArgumentNullException ("key");

            int indx = 0;
            try {
                indx = Find (key);
            } catch (Exception) {
                throw new InvalidOperationException();
            }

            return (indx | (indx >> 31));
        }

        public int IndexOfValue (TValue value)
        {
            if (inUse == 0)
                return -1;

            for (int i = 0; i < inUse; i ++) {
                KeyValuePair<TKey, TValue> current = this.table [i];

                if (Equals (value, current.Value))
                    return i;
            }

            return -1;
        }

        public bool ContainsValue (TValue value)
        {
            return IndexOfValue (value) >= 0;
        }

        public void TrimExcess ()
        {
            if (inUse < table.Length * 0.9)
                Capacity = inUse;
        }

        public bool TryGetValue (TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int i = Find (key);

            if (i >= 0) {
                value = table [i].Value;
                return true;
            }
            else {
                value = default (TValue);
                return false;
            }
        }

        //
        // Private methods
        //

        void EnsureCapacity (int n, int free)
        {
            KeyValuePair<TKey, TValue> [] table = this.table;
            KeyValuePair<TKey, TValue> [] newTable = null;
            int cap = Capacity;
            bool gap = (free >=0 && free < Count);

            if (n > cap) {
                newTable = new KeyValuePair<TKey, TValue> [n << 1];
            }

            if (newTable != null) {
                if (gap) {
                    int copyLen = free;
                    if (copyLen > 0) {
                        Array.Copy (table, 0, newTable, 0, copyLen);
                    }
                    copyLen = Count - free;
                    if (copyLen > 0) {
                        Array.Copy (table, free, newTable, free+1, copyLen);
                    }
                } else {
                    // Just a resizing, copy the entire table.
                    Array.Copy (table, newTable, Count);
                }
                this.table = newTable;
            } else if (gap) {
                Array.Copy (table, free, table, free+1, Count - free);
            }
        }

        void PutImpl (TKey key, TValue value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException ("null key");

            KeyValuePair<TKey, TValue> [] table = this.table;

            int freeIndx = -1;

            try {
                freeIndx = Find (key);
            } catch (Exception) {
                throw new InvalidOperationException();
            }

            if (freeIndx >= 0) {
                if (!overwrite)
                    throw new ArgumentException("element already exists");

                table [freeIndx] = new KeyValuePair <TKey, TValue> (key, value);
                ++modificationCount;
                return;
            }

            freeIndx = ~freeIndx;

            if (freeIndx > Capacity + 1)
                throw new Exception ("SortedList::internal error ("+key+", "+value+") at ["+freeIndx+"]");


            EnsureCapacity (Count+1, freeIndx);

            table = this.table;
            table [freeIndx] = new KeyValuePair <TKey, TValue> (key, value);

            ++inUse;
            ++modificationCount;

        }

        void Init (IComparer<TKey> comparer, int capacity, bool forceSize) 
        {
            if (comparer == null)
                comparer = Comparer<TKey>.Default;
            this.comparer = comparer;
            if (!forceSize && (capacity < defaultCapacity))
                capacity = defaultCapacity;
            this.table = new KeyValuePair<TKey, TValue> [capacity];
            this.inUse = 0;
            this.modificationCount = 0;
        }

        void  CopyToArray (Array arr, int i, 
                       EnumeratorMode mode)
        {
            if (arr == null)
                throw new ArgumentNullException ("arr");

            if (i < 0 || i + this.Count > arr.Length)
                throw new ArgumentOutOfRangeException ("i");
            
            IEnumerator it = new Enumerator (this, mode);

            while (it.MoveNext ()) {
                arr.SetValue (it.Current, i++);
            }
        }

        int Find (TKey key)
        {
            KeyValuePair<TKey, TValue> [] table = this.table;
            int len = Count;

            if (len == 0) return ~0;

            int left = 0;
            int right = len-1;

            while (left <= right) {
                int guess = (left + right) >> 1;

                int cmp = comparer.Compare (table[guess].Key, key);
                if (cmp == 0) return guess;

                if (cmp <  0) left = guess+1;
                else right = guess-1;
            }

            return ~left;
        }

        TKey ToKey (object key) {
            if (key == null)
                throw new ArgumentNullException ("key");
            if (!(key is TKey))
                throw new ArgumentException (
                    "The value \"" + key + "\" isn't of type \"" + typeof (TKey) + 
                    "\" and can't be used in this generic collection.", "key");
            return (TKey)key;
        }

        TValue ToValue (object value) {
            if (!(value is TValue))
                throw new ArgumentException (
                    "The value \"" + value + "\" isn't of type \"" + typeof (TValue) + 
                    "\" and can't be used in this generic collection.", "value");
            return (TValue)value;
        }

        internal TKey KeyAt (int index) {
            if (index >= 0 && index < Count)
                return table [index].Key;
            else
                throw new ArgumentOutOfRangeException("Index out of range");
        }

        internal TValue ValueAt (int index) {
            if (index >= 0 && index < Count)
                return table [index].Value;
            else
                throw new ArgumentOutOfRangeException("Index out of range");
        }

        //
        // Inner classes
        //


        sealed class Enumerator : ICloneable, IDictionaryEnumerator, IEnumerator {

            SortedList<TKey, TValue>host;
            int stamp;
            int pos;
            int size;
            EnumeratorMode mode;

            object currentKey;
            object currentValue;

            bool invalid = false;

            readonly static string xstr = "SortedList.Enumerator: snapshot out of sync.";

            public Enumerator (SortedList<TKey, TValue>host, EnumeratorMode mode)
            {
                this.host = host;
                stamp = host.modificationCount;
                size = host.Count;
                this.mode = mode;
                Reset ();
            }

            public Enumerator (SortedList<TKey, TValue>host)
            : this (host, EnumeratorMode.ENTRY_MODE)
            {
            }

            public void Reset ()
            {
                if (host.modificationCount != stamp || invalid)
                    throw new InvalidOperationException (xstr);

                pos = -1;
                currentKey = null;
                currentValue = null;
            }

            public bool MoveNext ()
            {
                if (host.modificationCount != stamp || invalid)
                    throw new InvalidOperationException (xstr);

                KeyValuePair<TKey, TValue> [] table = host.table;

                if (++pos < size) {
                    KeyValuePair<TKey, TValue> entry = table [pos];

                    currentKey = entry.Key;
                    currentValue = entry.Value;
                    return true;
                }

                currentKey = null;
                currentValue = null;
                return false;
            }

            public DictionaryEntry Entry
            {
                get {
                    if (invalid || pos >= size || pos == -1)
                        throw new InvalidOperationException (xstr);
                    
                    return new DictionaryEntry (currentKey,
                                                currentValue);
                }
            }

            public Object Key {
                get {
                    if (invalid || pos >= size || pos == -1)
                        throw new InvalidOperationException (xstr);
                    return currentKey;
                }
            }

            public Object Value {
                get {
                    if (invalid || pos >= size || pos == -1)
                        throw new InvalidOperationException (xstr);
                    return currentValue;
                }
            }

            public Object Current {
                get {
                    if (invalid || pos >= size || pos == -1)
                        throw new InvalidOperationException (xstr);

                    switch (mode) {
                                        case EnumeratorMode.KEY_MODE:
                                                return currentKey;
                                        case EnumeratorMode.VALUE_MODE:
                                                return currentValue;
                                        case EnumeratorMode.ENTRY_MODE:
                                                return this.Entry;

                                        default:
                                                throw new NotSupportedException (mode + " is not a supported mode.");
                                        }
                }
            }

            // ICloneable

            public object Clone ()
            {
                Enumerator e = new Enumerator (host, mode);
                e.stamp = stamp;
                e.pos = pos;
                e.size = size;
                e.currentKey = currentKey;
                e.currentValue = currentValue;
                e.invalid = invalid;
                return e;
            }
        }

        //[Serializable] AJP
        struct KeyEnumerator : IEnumerator <TKey>, IDisposable {
            const int NOT_STARTED = -2;
            
            // this MUST be -1, because we depend on it in move next.
            // we just decr the size, so, 0 - 1 == FINISHED
            const int FINISHED = -1;
            
            SortedList <TKey, TValue> l;
            int idx;
            int ver;
            
            internal KeyEnumerator (SortedList<TKey, TValue> l)
            {
                this.l = l;
                idx = NOT_STARTED;
                ver = l.modificationCount;
            }
            
            public void Dispose ()
            {
                idx = NOT_STARTED;
            }
            
            public bool MoveNext ()
            {
                if (ver != l.modificationCount)
                    throw new InvalidOperationException ("Collection was modified after the enumerator was instantiated.");
                
                if (idx == NOT_STARTED)
                    idx = l.Count;
                
                return idx != FINISHED && -- idx != FINISHED;
            }
            
            public TKey Current {
                get {
                    if (idx < 0)
                        throw new InvalidOperationException ();
                    
                    return l.KeyAt (l.Count - 1 - idx);
                }
            }
            
            void IEnumerator.Reset ()
            {
                if (ver != l.modificationCount)
                    throw new InvalidOperationException ("Collection was modified after the enumerator was instantiated.");
                
                idx = NOT_STARTED;
            }
            
            object IEnumerator.Current {
                get { return Current; }
            }
        }

        //[Serializable] AJP
        struct ValueEnumerator : IEnumerator <TValue>, IDisposable {
            const int NOT_STARTED = -2;
            
            // this MUST be -1, because we depend on it in move next.
            // we just decr the size, so, 0 - 1 == FINISHED
            const int FINISHED = -1;
            
            SortedList <TKey, TValue> l;
            int idx;
            int ver;
            
            internal ValueEnumerator (SortedList<TKey, TValue> l)
            {
                this.l = l;
                idx = NOT_STARTED;
                ver = l.modificationCount;
            }
            
            public void Dispose ()
            {
                idx = NOT_STARTED;
            }
            
            public bool MoveNext ()
            {
                if (ver != l.modificationCount)
                    throw new InvalidOperationException (
                        "Collection was modified after the enumerator was instantiated.");
                
                if (idx == NOT_STARTED)
                    idx = l.Count;
                
                return idx != FINISHED && -- idx != FINISHED;
            }
            
            public TValue Current {
                get {
                    if (idx < 0)
                        throw new InvalidOperationException ();
                    
                    return l.ValueAt (l.Count - 1 - idx);
                }
            }
            
            void IEnumerator.Reset ()
            {
                if (ver != l.modificationCount)
                    throw new InvalidOperationException (
                        "Collection was modified after the enumerator was instantiated.");
                
                idx = NOT_STARTED;
            }
            
            object IEnumerator.Current {
                get { return Current; }
            }
        }

        class ListKeys : IList<TKey>, ICollection, IEnumerable {

            SortedList<TKey, TValue> host;

            public ListKeys (SortedList<TKey, TValue> host)
            {
                if (host == null)
                    throw new ArgumentNullException ();

                this.host = host;
            }

            // ICollection<TKey>

            public virtual void Add (TKey item) {
                throw new NotSupportedException();
            }

            public virtual bool Remove (TKey key) {
                throw new NotSupportedException ();
            }

            public virtual void Clear () {
                throw new NotSupportedException();
            }

            public virtual void CopyTo (TKey[] array, int arrayIndex) {
                if (host.Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException ("array");
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException();
                if (arrayIndex >= array.Length)
                    throw new ArgumentOutOfRangeException ("arrayIndex is greater than or equal to array.Length");
                if (Count > (array.Length - arrayIndex))
                    throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");

                int j = arrayIndex;
                for (int i = 0; i < Count; ++i)
                    array [j ++] = host.KeyAt (i);
            }

            public virtual bool Contains (TKey item) {
                return host.IndexOfKey (item) > -1;
            }

            //
            // IList<TKey>
            //
            public virtual int IndexOf (TKey item) {
                return host.IndexOfKey (item);
            }

            public virtual void Insert (int index, TKey item) {
                throw new NotSupportedException ();
            }

            public virtual void RemoveAt (int index) {
                throw new NotSupportedException ();
            }

            public virtual TKey this [int index] {
                get {
                    return host.KeyAt (index);
                }
                set {
                    throw new NotSupportedException("attempt to modify a key");
                }
            }

            //
            // IEnumerable<TKey>
            //

            public virtual IEnumerator<TKey> GetEnumerator ()
            {
                /* We couldn't use yield as it does not support Reset () */
                return new KeyEnumerator (host);
            }

            //
            // ICollection
            //

            public virtual int Count {
                get {
                    return host.Count;
                }
            }

            public virtual bool IsSynchronized {
                get {
                    return ((ICollection)host).IsSynchronized;
                }
            }

            public virtual bool IsReadOnly {
                get {
                    return true;
                }
            }

            public virtual Object SyncRoot {
                get {
                    return ((ICollection)host).SyncRoot;
                }
            }

            public virtual void CopyTo (Array array, int arrayIndex)
            {
                host.CopyToArray (array, arrayIndex, EnumeratorMode.KEY_MODE);
            }

            //
            // IEnumerable
            //

            IEnumerator IEnumerable.GetEnumerator ()
            {
                for (int i = 0; i < host.Count; ++i)
                    yield return host.KeyAt (i);
            }
        }           

        class ListValues : IList<TValue>, ICollection, IEnumerable {

            SortedList<TKey, TValue>host;

            public ListValues (SortedList<TKey, TValue>host)
            {
                if (host == null)
                    throw new ArgumentNullException ();

                this.host = host;
            }

            // ICollection<TValue>

            public virtual void Add (TValue item) {
                throw new NotSupportedException();
            }

            public virtual bool Remove (TValue value) {
                throw new NotSupportedException ();
            }

            public virtual void Clear () {
                throw new NotSupportedException();
            }

            public virtual void CopyTo (TValue[] array, int arrayIndex) {
                if (host.Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException ("array");
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException();
                if (arrayIndex >= array.Length)
                    throw new ArgumentOutOfRangeException ("arrayIndex is greater than or equal to array.Length");
                if (Count > (array.Length - arrayIndex))
                    throw new ArgumentOutOfRangeException("Not enough space in array from arrayIndex to end of array");

                int j = arrayIndex;
                for (int i = 0; i < Count; ++i)
                    array [j ++] = host.ValueAt (i);
            }

            public virtual bool Contains (TValue item) {
                return host.IndexOfValue (item) > -1;
            }

            //
            // IList<TValue>
            //
            public virtual int IndexOf (TValue item) {
                return host.IndexOfValue (item);
            }

            public virtual void Insert (int index, TValue item) {
                throw new NotSupportedException ();
            }

            public virtual void RemoveAt (int index) {
                throw new NotSupportedException ();
            }

            public virtual TValue this [int index] {
                get {
                    return host.ValueAt (index);
                }
                set {
                    throw new NotSupportedException("attempt to modify a key");
                }
            }

            //
            // IEnumerable<TValue>
            //

            public virtual IEnumerator<TValue> GetEnumerator ()
            {
                /* We couldn't use yield as it does not support Reset () */
                return new ValueEnumerator (host);
            }

            //
            // ICollection
            //

            public virtual int Count {
                get {
                    return host.Count;
                }
            }

            public virtual bool IsSynchronized {
                get {
                    return ((ICollection)host).IsSynchronized;
                }
            }

            public virtual bool IsReadOnly {
                get {
                    return true;
                }
            }

            public virtual Object SyncRoot {
                get {
                    return ((ICollection)host).SyncRoot;
                }
            }

            public virtual void CopyTo (Array array, int arrayIndex)
            {
                host.CopyToArray (array, arrayIndex, EnumeratorMode.VALUE_MODE);
            }

            //
            // IEnumerable
            //

            IEnumerator IEnumerable.GetEnumerator ()
            {
                for (int i = 0; i < host.Count; ++i)
                    yield return host.ValueAt (i);
            }
        }

    } // SortedList

} // System.Collections.Generic


#endif



// ────────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

#if WP7

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

//
// System.Diagnostics.Stopwatch.cs
//
// Authors:
//   Zoltan Varga (vargaz@gmail.com)
//   Atsushi Enomoto  <atsushi@ximian.com>
//
// (C) 2006 Novell, Inc.
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

//#if NET_2_0 AJP

namespace System.Diagnostics
{
    public class Stopwatch
    {
        //[MethodImplAttribute(MethodImplOptions.InternalCall)] AJP
        //public static extern long GetTimestamp (); AJP

        public static long GetTimestamp() { return DateTime.Now.ToFileTimeUtc(); }

        public static readonly long Frequency = 10000000;

        public static readonly bool IsHighResolution = true;

        public static Stopwatch StartNew ()
        {
            Stopwatch s = new Stopwatch ();
            s.Start ();
            return s;
        }

        public Stopwatch ()
        {
        }

        long elapsed;
        long started;
        bool is_running;

        public TimeSpan Elapsed {
            get {
                if (IsHighResolution) {
                    // convert our ticks to TimeSpace ticks, 100 nano second units
                    // using two divisions helps avoid overflow
                    return TimeSpan.FromTicks ((long)(ElapsedTicks / (Frequency / TimeSpan.TicksPerSecond)));
                }
                else {
                    return TimeSpan.FromTicks (ElapsedTicks); 
                }
            }
        }

        public long ElapsedMilliseconds {
            get { 
                checked {
                    if (IsHighResolution) {
                        return (long)(ElapsedTicks / (Frequency / 1000));
                    }
                    else {
                        return (long) Elapsed.TotalMilliseconds;
                    }
                } 
            }
        }

        public long ElapsedTicks {
            get { return is_running ? GetTimestamp () - started + elapsed : elapsed; }
        }

        public bool IsRunning {
            get { return is_running; }
        }

        public void Reset ()
        {
            elapsed = 0;
            is_running = false;
        }

        public void Start ()
        {
            if (is_running)
                return;
            started = GetTimestamp ();
            is_running = true;
        }

        public void Stop ()
        {
            if (!is_running)
                return;
            elapsed += GetTimestamp () - started;
            is_running = false;
        }

//#if NET_4_0 || MOBILE AJP
        public void Restart ()
        {
            started = GetTimestamp ();
            elapsed = 0;
            is_running = true;
        }
//#endif AJP
    }
}


#endif

