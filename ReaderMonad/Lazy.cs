#if false
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace PGSolutions.Utilities.Monads {
  public class Lazy<T1, T2> {
    private readonly Lazy<StructTuple<T1, T2>> _factory;

    public Lazy(Func<T1> factory1, Func<T2> factory2)
      : this(() => StructTuple.Create(factory1(), factory2())) {
        factory1.ContractedNotNull("factory1");
        factory2.ContractedNotNull("factory2");
    }

    public Lazy(T1 value1, T2 value2)
      : this(() => StructTuple.Create(value1, value2)) {
        value1.ContractedNotNull("value1");
        value2.ContractedNotNull("value2");
    }

    public Lazy(Func<StructTuple<T1, T2>> factory) {
      factory.ContractedNotNull("factory");
      this._factory = new Lazy<StructTuple<T1, T2>>(factory);
    }

    public T1 Value1 {
      [Pure]
      get { return this._factory.Value.Item1; }
    }

    public T2 Value2 {
      [Pure]
      get { return this._factory.Value.Item2; }
    }

    /// <summary>The invariants enforced by this type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]private void ObjectInvariant() {
      Contract.Invariant( _factory != null );
    }
  }

  [Pure]
  public static class LazyExtensions {
    public static Lazy<TResult1, TResult2> Select<TSource1, TSource2, TResult1, TResult2> (
      this Lazy<TSource1, TSource2> source, 
      Func<TSource1, TResult1> selector1, 
      Func<TSource2, TResult2> selector2
    ) {
        Contract.Ensures(Contract.Result<Lazy<TResult1, TResult2>>() != null);
        return new Lazy<TResult1, TResult2>(
            () => selector1(source.Value1),
            () => selector2(source.Value2)
          );
    }

    public static IMorphism<Lazy<TSource1, TSource2>, Lazy<TResult1, TResult2>, DotNet> Select<TSource1, TSource2, TResult1, TResult2> (
      IMorphism<TSource1, TResult1, DotNet> selector1,
      IMorphism<TSource2, TResult2, DotNet> selector2
    ) {
        Contract.Ensures(Contract.Result<IMorphism<Lazy<TSource1, TSource2>, Lazy<TResult1, TResult2>, DotNet>>() != null);
        return new DotNetMorphism<Lazy<TSource1, TSource2>, Lazy<TResult1, TResult2>>(
              source => source.Select(selector1.Invoke, selector2.Invoke)
        );
    }
  }

    [ContractClass(typeof(IStructTupleContract))]
    internal interface IStructTuple {
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer);
        int Size { get; }
 
    }

    [ContractClassFor(typeof(IStructTuple))]
    abstract class IStructTupleContract : IStructTuple {
      public string ToString(StringBuilder sb) {
        sb.ContractedNotNull("sb"); return default(string);
      }

      public int GetHashCode(IEqualityComparer comparer) { return default(int); }
      public int Size { get { return default(int); } }
    }

    internal static class StructTuple {
        internal static int CombineHashCodes(int h1, int h2) {
            return (((h1 << 5) + h1) ^ h2);
        }
       public static StructTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
            return new StructTuple<T1, T2>(item1, item2);
        }
    }
    [Serializable]
    public struct StructTuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, IStructTuple {
 
        private readonly T1 m_Item1;
        private readonly T2 m_Item2;
 
        public T1 Item1 { get { return m_Item1; } }
        public T2 Item2 { get { return m_Item2; } }
 
        public StructTuple(T1 item1, T2 item2) {
            m_Item1 = item1;
            m_Item2 = item2;
        }
 
        public override Boolean Equals(Object obj) {
            return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);;
        }

        /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
        [Pure]
        public static bool operator ==(StructTuple<T1, T2> lhs, StructTuple<T1, T2> rhs) { return lhs.Equals(rhs); }

        /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
        [Pure]
        public static bool operator !=(StructTuple<T1, T2> lhs, StructTuple<T1, T2> rhs) { return ! lhs.Equals(rhs); }
 
        Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer) {
            //comparer.ContractedNotNull("comparer");
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (other == null) return false;
 
            var objTuple = other as StructTuple<T1, T2>?;
 
            return objTuple.HasValue 
                && comparer.Equals(m_Item1, objTuple.Value.m_Item1) 
                && comparer.Equals(m_Item2, objTuple.Value.m_Item2);
        }
 
        Int32 IComparable.CompareTo(Object obj) {
            return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
        }
 
        Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer) {
            //comparer.ContractedNotNull("comparer");
            if (comparer == null) throw new ArgumentNullException("comparer");
            if (other == null) return 1;
 
            var objTuple = other as StructTuple<T1, T2>?;
 
            if ( ! objTuple.HasValue) {
                throw new ArgumentException("Tuple is Incorrect Type", "other");
            }
 
            int c = comparer.Compare(m_Item1, objTuple.Value.m_Item1);
 
            if (c != 0) return c;
 
            return comparer.Compare(m_Item2, objTuple.Value.m_Item2);
        }
 
        public override int GetHashCode() {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
        }
 
        Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer) {
            //comparer.ContractedNotNull("comparer");
            if (comparer == null) throw new ArgumentNullException("comparer");
            else {
              var hc1 = m_Item1 == null ? 0 : comparer.GetHashCode(m_Item1);
              var hc2 = m_Item2 == null ? 0 : comparer.GetHashCode(m_Item2);
              return StructTuple.CombineHashCodes(hc1,hc2);
            }

            //return StructTuple.CombineHashCodes(comparer.GetHashCode(m_Item1), 
            //                                    comparer.GetHashCode(m_Item2));
        }
 
        Int32 IStructTuple.GetHashCode(IEqualityComparer comparer) {
            return ((IStructuralEquatable) this).GetHashCode(comparer);
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((IStructTuple)this).ToString(sb) ?? this.GetType().Name;
        }
 
        string IStructTuple.ToString(StringBuilder sb) {
            Contract.Ensures(Contract.Result<string>() != null);

            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(")");
            return sb.ToString();
        }
 
        int IStructTuple.Size {
            get {
                return 2;
            }
        }
    }
}
#endif