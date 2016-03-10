using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WorkflowMonad {
  public class WorkflowComposition : Workflow<int> {
    public override WorkflowStep<int> GetResultInt ()     {
      return  from a in new SumWorkflow (Context).GetResult ()
              from b in new SumWorkflow (Context).GetResult ()
              let res = a + b
              from v in Show ("Result of two = " + (a + b))
              select a + b;
    }
  }
  public class WorkflowStep<T> {
    public WorkflowStep (Action act, ExecutionContext context, int index) {
        Action = act;
        Context = context;
        Index = index;
    }
    public Action Action {
        get;
        private set;
    }
    public WorkflowStep (T value) {
        Context.Memory.Add (value.ValueToString ());
    }
    public ExecutionContext Context = new ExecutionContext ();
    public bool IsExecuted () { return (this.Index) < Context.Memory.Count; }
    public T GetValue () { return Context.Memory [this.Index].ParseValue<T> (); }
    public int Index {
        get;
        private set;
    }
  }
  public class ExecutionContext {
    public ExecutionContext () {
        Memory = new List<string> ();
    }
    public List<string> Memory {
        get;
        private set;
    }
    public int Index {
        get;
        private set;
    }
    public void Restart () {
        Index = 0;
    }
    public void Inc () {
        Index = Index + 1;
    }
  }

  public class SumWorkflow : Workflow<int> {
    public SumWorkflow () : base() { }
    public SumWorkflow(ExecutionContext context) : base(context) { }

    public override WorkflowStep<int> GetResult () {
      Ask<int>("enter a").Bind( a=>
      Ask<int>("enter b").Bind( b=>
        {
          var res = a + b;
          return Show("Result= " + res).Bind(unit => new WorkflowStep<int>(res) );
        } ) );
      return new WorkflowStep<int>();
    }
  }

  public static class SerializationHelpers {
    public static T ParseValue<T> (this string json)
    {
        return JsonConvert.DeserializeObject<T> (json);
    }
    public static string ValueToString<T> (this T value)
    {
        return JsonConvert.SerializeObject (value);
    }
  }

  public abstract class Workflow<TB> {
    public Workflow () {
        Context = new ExecutionContext ();
    }
    public Workflow (ExecutionContext ctx) {
        Context = ctx;
        IsSubWorkflow = true;
    }
    public bool IsSubWorkflow {
        get;
        private set;
    }
    public ExecutionContext Context {
        get;
        private set;
    }
    protected WorkflowStep<T> Do<T> (Action act) {
        var step = new WorkflowStep<T> (act, Context, Context.Index);
        Context.Inc ();
        return step;
    }
    protected WorkflowStep<T> Ask<T> (String what) {
        return Do<T> (new Ask<T> (){ What = what });
    }
    protected WorkflowStep<Unit> Show (String what) {
        return Do<Unit> (new Show (){ What = what });
    }
    public abstract WorkflowStep<TB> GetResultInt ();
    public WorkflowStep<TB> GetResult () {
        if (!IsSubWorkflow)
            Context.Restart ();
        return GetResultInt ();
    }
    public void AddResult (object val) {
        Context.Memory.Add (val.ValueToString ());
    }
  }
  public static class WorkflowMonad {
    public static WorkflowStep<T> Return<T> (this T value)
    {
        return new WorkflowStep<T> (value);
    }
    public static WorkflowStep<U> Bind<T, U> (
        this WorkflowStep<T> m,
        Func<T, WorkflowStep<U>> k)
    {
        if (m.IsExecuted ()) {
            return k (m.GetValue());
        }
        return new WorkflowStep<U> (m.Action);
    }
    public static WorkflowStep<V> SelectMany<T, U, V> (
        this WorkflowStep<T> id,
        Func<T, WorkflowStep<U>> k,
        Func<T, U, V> s)
    {
        return id.Bind (x => k (x).Bind (y => s (x, y).Return ()));
    }
    public static WorkflowStep<B> Select<A, B> (
        this WorkflowStep<A> a,
        Func<A, B> select)
    {
        return a.Bind (aval => WorkflowMonad.Return (select (aval)));
    }
  }

  public class Action { }

  //show some text to user
  public class Show : Action {
      public string What {
          get;
          set;
      }
  }

  //show some text to user and ask a value of specified type
  public class Ask<T> : Action {
      public string What {
          get;
          set;
      }
  }

  /// <summary>no result object</summary>
  public class Unit {
      Unit () { }
      public static Unit Value = new Unit ();
  }
}
