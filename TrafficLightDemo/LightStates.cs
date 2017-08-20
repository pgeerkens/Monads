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
using System.Drawing;

namespace PGSolutions.Monads.TrafficLightDemo {
    using ISettableLight = ISettableLight<Image>;

    /// <summary>The Traffic LIght Finite-State-Machine.</summary>
    public class LightStates<TSettableLight,TEnv> : FsmTask<TEnv> 
            where TSettableLight : ISettableLight
            where TEnv           : ITrafficLight<TSettableLight,Image> 
    {
        /// <inheritdoc/>
        protected override FsmTransition Start       => e => Reset.GetState(          500,
            from _    in e.UpDownTown.SetColor         (e.Green)
            from __   in e.CrossTown.SetColor          (e.Yellow)
            from ___  in e.UpTownLeftTurn.SetColor     (e.Green)
            from ____ in e.DownTownLeftTurn.SetColor   (e.Red)
            select Unit.Empty);

        static FsmTransition Reset                   => e => LeftTurnGreen.GetState(    2000,
            from _    in e.UpDownTown.SetColor         (e.Red)
            from __   in e.CrossTown.SetColor          (e.Red)
            from ___  in e.UpTownLeftTurn.SetColor     (e.Red)
            from ____ in e.DownTownLeftTurn.SetColor   (e.Red)
            select Unit.Empty);

        static FsmTransition CrossTownRed            => e => LeftTurnGreen.GetState(     300,
            from _    in e.CrossTown.SetColor          (e.Red)
            select Unit.Empty);

        static FsmTransition LeftTurnGreen           => e => LeftTurnYellow.GetState(   3700,
            from _    in e.UpTownLeftTurn.SetColor     (e.Green)
            from __   in e.DownTownLeftTurn.SetColor   (e.Green)
            select Unit.Empty);

        static FsmTransition LeftTurnYellow          => e => LeftTurnRed.GetState(      1000,
            from _    in e.UpTownLeftTurn.SetColor     (e.Yellow)
            from __   in e.DownTownLeftTurn.SetColor   (e.Yellow)
            select Unit.Empty);

        static FsmTransition LeftTurnRed             => e => UpDownTownGreen.GetState(   000,
            from _    in e.UpTownLeftTurn.SetColor     (e.Red)
            from __   in e.DownTownLeftTurn.SetColor   (e.Red)
            select Unit.Empty);

        static FsmTransition UpDownTownGreen         => e => UpDownTownYellow.GetState(10000,
            from _    in e.UpDownTown.SetColor         (e.Green)
            select Unit.Empty);

        static FsmTransition UpDownTownYellow        => e => UpDownTownRed.GetState(    1000,
            from _    in e.UpDownTown.SetColor         (e.Yellow)
            select Unit.Empty);

        static FsmTransition UpDownTownRed           => e => CrossTownGreen.GetState(    300,
            from _    in e.UpDownTown.SetColor         (e.Red)
            select Unit.Empty);

        static FsmTransition CrossTownGreen          => e => CrossTownYellow.GetState(  6700,
            from _    in e.CrossTown.SetColor          (e.Green)
            select Unit.Empty);

        static FsmTransition CrossTownYellow         => e => CrossTownRed.GetState(     1000,
            from _    in e.CrossTown.SetColor          (e.Yellow)
            select Unit.Empty);
    }
}
