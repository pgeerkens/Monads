using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using PGSolutions.Utilities.Monads;

#if false
namespace CheckMonad {
  public static class CheckMonad {
    public static Func<Check.CheckM<TB>> Lift<TB>(this Func<TB> f)
          where TB : class {
      return () => Check.CheckM<TB>.ToCheck(f());
    }
    public static Func<Check.CheckM<TB>> Lift<TB>(
        this Func<Check.CheckM<TB>> f)
          where TB : class {
      return () => f();
    }
    public static Func<CheckForT<TMI>.CheckT<TB>> LiftT<TB, TMI>(
        this Func<IMonad<TB, TMI>> f)
          where TB : class {
      Func<IMonad<Check.CheckM<TB>, TMI>> checkF = () => {
        var m = f();
        return m.Bind(val => m.Return(Check.CheckM<TB>.ToCheck(val)));
      };
      return () => new CheckForT<TMI>.CheckT<TB>(checkF());

      var x = f().Bind(val => f().Return(Check.CheckM<TB>.ToCheck(val)));
      return () => new CheckForT<TMI>.CheckT<TB>(x);
    }
  }


  public interface IGeneric<T, TCONTAINER> { }
  public class Wrapper {
      public sealed class WrapperImpl<T> : Wrapper, IGeneric<T, Wrapper> {
      }
  }
  public static class GenericExts {
      public static TM UpCast<T, TM, TMB> (this IGeneric<T, TMB> m)
          where TM : TMB, IGeneric<T, TMB>
      {
          return (TM)m;//safe for single inheritance
   }
  }

  public interface IFunctor<T> {
      CB FMap<A, B, CA, CB> (Func<A, B> f, CA a)
          where CA : IGeneric<A, T>
          where CB : IGeneric<B, T>;
  }
  public interface IMonad<T, TMI> {
      IMonad<TB,TMI> Return<TB> (TB val);
      IMonad<TB,TMI> Bind<TB> (Func<T, IMonad<TB,TMI>> f);
  }
  public static class MonadExts {
      public static TM UpCast<T, TM, TMB> (this IMonad<T, TMB> m)
          where TM : TMB, IMonad<T, TMB>
      {
          return (TM)m;//safe for single inheritance
   }
  }

  public class CheckForT<TMI>
  {
    CheckForT ()
    {
    }
    public sealed class CheckT<T>: CheckForT<TMI>, IMonad<T, CheckForT<TMI>> {
        #region IMonad implementation
        public IMonad<TB, CheckForT<TMI>> Return<TB> (TB val)
        {
            return new CheckT<TB> (
                Value.Return<Check.CheckM<TB>> (
                    Check.CheckM<TB>.Success (val)
                )
            );
        }
        private IMonad<Check.CheckM<TB>,TMI> BindInternal<TB> (
            Check.CheckM<T> check,
            Func<T, IMonad<TB, CheckForT<TMI>>> f)
        {
            return check.IsFailed
                ? Value.Return<Check.CheckM<TB>> (Check.CheckM<TB>.Fail ())
                : f (check.Value).UpCast<TB, CheckT<TB>,CheckForT<TMI>> ().Value;
        }
        public IMonad<TB, CheckForT<TMI>> Bind<TB> (
            Func<T, IMonad<TB, CheckForT<TMI>>> f)
        {
            var tmp = Value.Bind<Check.CheckM<TB>> (
                check => BindInternal (check, f)
            );
            return new CheckT<TB> (tmp);
        }
        #endregion

        public CheckT (IMonad<Check.CheckM<T>,TMI> val)
        {
            Value = val;
        }
        public IMonad<Check.CheckM<T>,TMI> Value {
            get;
            private set;
        }
    }

  }

  public class Check {
      Check ()
      {
      }
      public sealed class CheckM<T>: Check, IMonad<T, Check> {
          #region IMonad implementation
          public IMonad<TB, Check> Return<TB> (TB val)
          {
              return CheckM<TB>.Success (val);
          }
          public IMonad<TB, Check> Bind<TB> (Func<T, IMonad<TB, Check>> f)
          {
              return this.IsFailed ? CheckM<TB>.Fail () : f (this.Value);
          }
          #endregion
          CheckM (T val) : base() {
              Value = val;
          }
          public static CheckM<T> Success (T val)
          {
              return new CheckM<T> (val){ IsFailed = false };
          }
          public static CheckM<T> Fail ()
          {
              return new CheckM<T> (default(T)){ IsFailed = true };
          }
          public bool IsFailed {
              get;
              private set;
          }
          public T Value {
              get;
              private set;
          }
          public override string ToString ()
          {
              return string.Format (
                  "[Check: IsFailed={0}, Value={1}]",
                  IsFailed,
                  Value);
          }

          public static CheckM<T> ToCheck(T val) { return new CheckM<T>(val); }
      }
  }
}
#endif