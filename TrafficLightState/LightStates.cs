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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using PGSolutions.Utilities.Monads;

namespace TrafficLightDemo {
  /// <summary>TODO</summary>
  public static class LightStates {
    /// <summary>TODO</summary>
    public static async Task<Unit> ExecuteTrafficLight<T>
      (this ITrafficLight<T> e) {
        e.ContractedNotNull(nameof(e));
        Contract.Ensures(Contract.Result<Task<Unit>>() != null);  // not supported for async/await
        try {
            var trafficLightState = await LightStates<T>.Start(e)(e);
            while (trafficLightState != null)  trafficLightState = await trafficLightState()(e);
        } catch (OperationCanceledException) { ; }
        return Unit._;
    }

    //[Pure]
    internal static Reader<ITrafficLight<T>, Task<LightStates<T>.TrafficLightState>> GetState<T>(
      this LightStates<T>.Transition transition,
      int delayTenthsSeconds,
      IO<Unit> action
    ) {
        transition.ContractedNotNull(nameof(transition));
        Contract.Ensures(Contract.Result<Reader<ITrafficLight<T>,Task<LightStates<T>.TrafficLightState>>>() != null);

        return e => from _ in ( from _ in action
                                select Task.Delay(TimeSpan.FromMilliseconds(100*delayTenthsSeconds)
                                                 ,e.CancellationToken)
                              ).Invoke()
                    select new LightStates<T>.TrafficLightState(() => transition(e));
    }
  }
  static class LightStates<T> {
    public delegate Reader<ITrafficLight<T>, Task<TrafficLightState>>   TrafficLightState();
    public delegate Reader<ITrafficLight<T>, Task<TrafficLightState>>   Transition(ITrafficLight<T> e);

    public static readonly Transition Start        = e => Reset.GetState(             5,
          ( from _    in e.UpDownTown.SetColor      (e.Green)
            from __   in e.CrossTown.SetColor       (e.Yellow)
            from ___  in e.UpTownLeftTurn.SetColor  (e.Green)
            from ____ in e.DownTownLeftTurn.SetColor(e.Red)
            select _) );
    public static readonly Transition Reset        = e => LeftTurnGreen.GetState(    20,
          ( from _    in e.UpDownTown.SetColor      (e.Red)
            from __   in e.CrossTown.SetColor       (e.Red)
            from ___  in e.UpTownLeftTurn.SetColor  (e.Red)
            from ____ in e.DownTownLeftTurn.SetColor(e.Red)
            select _) );

    #region Transitions
    static readonly Transition CrossTownRed        = e => LeftTurnGreen.GetState(     3,
          ( from _    in e.CrossTown.SetColor       (e.Red)
            select _) );

    static readonly Transition LeftTurnGreen       = e => LeftTurnYellow.GetState(   37,
          ( from _    in e.UpTownLeftTurn.SetColor  (e.Green)
            from __   in e.DownTownLeftTurn.SetColor(e.Green)
            select _) );

    static readonly Transition LeftTurnYellow      = e => LeftTurnRed.GetState(      10,
          ( from _    in e.UpTownLeftTurn.SetColor  (e.Yellow)
            from __   in e.DownTownLeftTurn.SetColor(e.Yellow)
            select _) );

    static readonly Transition LeftTurnRed         = e => UpDownTownGreen.GetState(   0,
          ( from _    in e.UpTownLeftTurn.SetColor  (e.Red)
            from __   in e.DownTownLeftTurn.SetColor(e.Red)
            select _) );

    static readonly Transition UpDownTownGreen     = e => UpDownTownYellow.GetState(100,
          ( from _    in e.UpDownTown.SetColor      (e.Green)
            select _) );

    static readonly Transition UpDownTownYellow    = e => UpDownTownRed.GetState(    10,
          ( from _    in e.UpDownTown.SetColor      (e.Yellow)
            select _) );

    static readonly Transition UpDownTownRed       = e => CrossTownGreen.GetState(    3,
          ( from _    in e.UpDownTown.SetColor      (e.Red)
            select _) );

    static readonly Transition CrossTownGreen      = e => CrossTownYellow.GetState(  67,
          ( from _    in e.CrossTown.SetColor       (e.Green)
            select _) );

    static readonly Transition CrossTownYellow     = e => CrossTownRed.GetState(     10,
          ( from _    in e.CrossTown.SetColor       (e.Yellow)
            select _) );
    #endregion
  }
}
