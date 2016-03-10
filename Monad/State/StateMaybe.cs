#region The MIT License - Copyright (C) 2012-2016 Pieter Geerkens
/////////////////////////////////////////////////////////////////////////////////////////
//                PG Software Solutions - Monad Utilities
/////////////////////////////////////////////////////////////////////////////////////////
// The MIT License:
// ----------------
// 
// Copyright (c) 2012-2016 Pieter Geerkens (email: pgeerkens@hotmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to the following 
// conditions:
//     The above copyright notice and this permission notice shall be 
//     included in all copies or substantial portions of the Software.
// 
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//     EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//     OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
//     NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//     HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//     FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//     OTHER DEALINGS IN THE SOFTWARE.
/////////////////////////////////////////////////////////////////////////////////////////
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace PGSolutions.Utilities.Monads {
  /// <summary>TODO</summary>
  public delegate StatePayload<TState,Maybe<TValue>>    StateMaybe<TState,TValue>(TState s);

#if false

  //public interface IBindable<TValue> {
  //  IBindable<TResult> Bind<TResult> (Func<TValue,IBindable<TResult>> selector);
  //}

  /// <summary>TODO</summary>
  [Pure]
  public static partial class StateMaybe {
    public static StateMaybe<bool,TState>        DoWhile<TState>( this
        StateMaybe<bool,TState>.Transformer @this
    ) where TState:struct {
        @this.ContractedNotNull("this");

        return new StateMaybe<bool,TState>(s => {
            StateMaybe<bool,TState>.StateTuple tuple;
            do { tuple = @this(s); s = tuple.State; } while (tuple.Value.Extract(false));
            return tuple;
        } );
    }
    public static StateMaybe<bool,TState>        DoWhile<TState>( this
        StateMaybe<bool,TState> @this
    ) where TState:struct {
      @this.AssumeInvariant();
      return DoWhile(@this.RunState);
    }

    public static StateMaybe<Unit,TState>        Put<TState>(TState state) where TState:struct {
        return new StateMaybe<Unit,TState>( s => new StateMaybe<Unit,TState>.StateTuple(Unit._,state) );
    }
  }

  /// <summary>TODO</summary>
  [Pure]
  public static partial class StateMaybe<TState> where TState:struct {

    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public readonly static StateMaybe<TState, TState>    Get
        = new StateMaybe<TState, TState>(s => new StateMaybe<TState,TState>.StateTuple(s, s));

    #region Convenience extensions to Get() for efficiency
    /// <summary>Optimized implementation of Get.Bind(<see cref="selector"/>).</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="selector"></param>
    [Pure, SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static StateMaybe<TValue, TState>     GetBind<TValue>(
        Func<TState,StateMaybe<TValue,TState>> selector
    ) where TValue:struct {
        selector.ContractedNotNull("selector");

        return GetCompose(selector);      // Since type of Value for Get() is TState.
    }

    /// <summary>TODO</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="selector"></param>
    [Pure, SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static StateMaybe<TValue,TState>      GetCompose<TValue>(
        Func<TState,StateMaybe<TValue,TState>> selector
    ) where TValue:struct {
        selector.ContractedNotNull("selector");

        return new StateMaybe<TValue,TState>( s => selector(s).RunState(s) );
    }

    /// <summary>Optimized implementation of Get.Compose(s => Put(<see cref="transform"/>)).</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="transform"></param>
    [Pure, SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static StateMaybe<TState,TState>        Modify(
        Func<TState,TState> transform
    ) {
        transform.ContractedNotNull("transform");

        return new StateMaybe<TState,TState>( s => new StateMaybe<TState,TState>.StateTuple(s,transform(s)) );
    }

    /// <summary>Optimized implementation of Get.Select(<see cref="projector"/>).</summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <param name="projector"></param>
    [Pure, SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public static StateMaybe<TValue,TState>      GetSelect<TValue>(
        StateMaybe<TValue,TState>.Transformer projector
    ) where TValue:struct {
        projector.ContractedNotNull("projector");

        return new StateMaybe<TValue,TState>(projector);
    }
    #endregion
  }

  public static class StateMaybeExtensions {
    public static Func<TState,StateMaybe<TValue,TState>.StateTuple> Lift<TValue,TState>( this
        StateMaybe<TValue,TState> @this
    ) where TValue:struct where TState:struct {
        Contract.Ensures(Contract.Result<Func<TState,StateMaybe<TValue,TState>.StateTuple>>() != null);
        return s => @this.RunState(s).Bind(x => StateMaybe<TValue,TState>.Return(x,s));
    }
  }

  public struct StateMaybe<TValue,TState> where TValue:struct where TState:struct {

    public delegate StateTuple Transformer(TState state);

    public StateMaybe(
        Transformer runStateT
    ) {
        runStateT.ContractedNotNull("runStateT");
        RunState  = runStateT;
    }
          public readonly Transformer  RunState;
    [Pure]public Maybe<TValue>          EvalState(TState state) { return RunState(state).Value; }
    [Pure]public TState                 ExecState(TState state) { return RunState(state).State; }

    public StateMaybe<TResult,TState> Bind<TResult>(
        Func<Maybe<TValue>,StateMaybe<TResult,TState>> selector
    ) where TResult:struct {
        selector.ContractedNotNull("selector");

        var @this = this;
        return new StateMaybe<TResult,TState>(s => 
                       @this.RunState(s).Bind(t => 
                       selector(t.Value).RunState(t.State)) );
    }

    public StateMaybe<TValue,TState>.StateTuple Lift(TState s) {//where TResult:struct {
        //var @this = this;
        return //new StateMaybe<TValue,TState>(ss => 
          RunState(s).Bind(x => Return(x,s))
        //  )
        ;
    }

    public static StateTuple Return(StateTuple t, TState s) {
        return t.Value.Bind(v => new StateTuple(v,s));
    }

    [Pure]
    public static implicit operator StateMaybe<TValue,TState>(
        Transformer runStateT
    ) {
        runStateT.ContractedNotNull("runStateT");
        return new StateMaybe<TValue,TState>(runStateT);
    }

    /// <summary>Implementation of (return): return a = StateT (\s -> return(a,s)).</summary>
    /// <param name="runStateT"></param>
    public static implicit operator StateMaybe<TValue, TState>(TValue value) {
      return new StateMaybe<TValue,TState>(s => new StateTuple(value,s));
    }

    /// <summary>The invariants enforced by this type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]private void ObjectInvariant() {
      Contract.Invariant( RunState != null );
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
      Justification="This nested type shares the Generic Type parameters of its parent, and is structurally associated with it.")]
    public struct StateTuple {
      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      public StateTuple(Tuple<Maybe<TValue>, TState> content) : this(content.Item1,content.Item2) {
          content.ContractedNotNull("content");
      }
      public StateTuple(Maybe<TValue> value, TState state ) : this() {
          _value = value; _state = state;
      }
      public Maybe<TValue> Value { get {return _value;} } readonly Maybe<TValue> _value;
      public TState State { get {return _state;} } readonly TState _state;

      #region Value Equality with IEquatable<T>.
      /// <inheritdoc/>
      [Pure]public override bool Equals(object obj) { 
        var other = obj as StateTuple?;
        return other != null  &&  other.Equals(obj);
      }

      /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
      [Pure]public bool Equals(StateTuple other) {
        return this.Value.Equals(other.Value)  &&  this.State.Equals(other.State);
      }

      /// <inheritdoc/>
      [Pure]public override int GetHashCode() { unchecked { return Value.GetHashCode() ^ State.GetHashCode(); } }

      /// <inheritdoc/>
      [Pure]public override string ToString() {
        Contract.Ensures(Contract.Result<string>() != null);
        return String.Format(CultureInfo.InvariantCulture,"({0},{1})",Value,State);
      }

      /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
      [Pure]public static bool operator == (StateTuple lhs, StateTuple rhs) { return lhs.Equals(rhs); }

      /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
      [Pure]public static bool operator != (StateTuple lhs, StateTuple rhs) { return ! lhs.Equals(rhs); }
      #endregion
    }
  }

  public struct StateMaybe2<TValue,TState> where TValue:struct where TState:struct {

    public delegate Maybe<StateTuple> Transformer(TState state);

    public StateMaybe2(
        Transformer runStateT
    ) {
        runStateT.ContractedNotNull("runStateT");
        RunState  = runStateT;
    }
          public readonly Transformer  RunState;
    //[Pure]public Maybe<TValue>          EvalState(TState state) { return RunState(state).Value; }
    //[Pure]public TState                 ExecState(TState state) { return RunState(state).State; }

    public StateMaybe2<TResult,TState> Bind<TResult>(
        Func<Maybe<TValue>,StateMaybe2<TResult,TState>> selector
    ) where TResult:struct {
        selector.ContractedNotNull("selector");

        var @this = this;
        return new StateMaybe2<TResult,TState>(s => 
                       @this.RunState(s).Bind(t => 
                       selector(t.Value).RunState(t.State)) );
    }

    public StateMaybe2<TValue,TState> Lift<TResult>() where TResult:struct {
        var @this = this;
        return new StateMaybe2<TValue,TState>(s => @this.RunState(s).Bind(@this.Return) );
    }

    public Maybe<StateTuple> Return(StateTuple t) { return new Maybe<StateTuple>(t); }

    [Pure]
    public static implicit operator StateMaybe2<TValue,TState>(
        Transformer runStateT
    ) {
        runStateT.ContractedNotNull("runStateT");
        return new StateMaybe2<TValue,TState>(runStateT);
    }

    /// <summary>Implementation of (return): return a = StateT (\s -> return(a,s)).</summary>
    /// <param name="runStateT"></param>
    public static implicit operator StateMaybe2<TValue, TState>(TValue value) {
      return new StateMaybe2<TValue,TState>(s => new StateTuple(value,s));
    }

    /// <summary>The invariants enforced by this type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]private void ObjectInvariant() {
      Contract.Invariant( RunState != null );
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible",
      Justification="This nested type shares the Generic Type parameters of its parent, and is structurally associated with it.")]
    public struct StateTuple {
      [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
      public StateTuple(Tuple<TValue, TState> content) : this(content.Item1,content.Item2) {
          content.ContractedNotNull("content");
      }
      public StateTuple(TValue value, TState state ) : this() {
          _value = value; _state = state;
      }
      public TValue Value { get {return _value;} } readonly TValue _value;
      public TState State { get {return _state;} } readonly TState _state;

      #region Value Equality with IEquatable<T>.
      /// <inheritdoc/>
      [Pure]public override bool Equals(object obj) { 
        var other = obj as StateTuple?;
        return other != null  &&  other.Equals(obj);
      }

      /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
      [Pure]public bool Equals(StateTuple other) {
        return this.Value.Equals(other.Value)  &&  this.State.Equals(other.State);
      }

      /// <inheritdoc/>
      [Pure]public override int GetHashCode() { unchecked { return Value.GetHashCode() ^ State.GetHashCode(); } }

      /// <inheritdoc/>
      [Pure]public override string ToString() {
        Contract.Ensures(Contract.Result<string>() != null);
        return String.Format(CultureInfo.InvariantCulture,"({0},{1})",Value,State);
      }

      /// <summary>Tests value-equality, returning <b>false</b> if either value doesn't exist.</summary>
      [Pure]public static bool operator == (StateTuple lhs, StateTuple rhs) { return lhs.Equals(rhs); }

      /// <summary>Tests value-inequality, returning <b>false</b> if either value doesn't exist..</summary>
      [Pure]public static bool operator != (StateTuple lhs, StateTuple rhs) { return ! lhs.Equals(rhs); }
      #endregion
    }
  }
#endif
}
