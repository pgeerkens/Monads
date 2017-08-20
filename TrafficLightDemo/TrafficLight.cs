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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PGSolutions.Monads.TrafficLightDemo {
    using ITrafficLight = ITrafficLight<SettableLight,Image>;
    using static Properties.Resources;

    internal static class Extensions {
        public static Bitmap SetTransparent(this Bitmap @this) {
            @this.MakeTransparent(@this.GetPixel(1, 1));
            return @this;
        }
    }

    struct TrafficLight : ITrafficLight {
        public TrafficLight(ICancellationTokenAgent ctsSource, params PictureBox[] lights) : this() {
            _cancellationTokenSource = ctsSource;
            CrossTown          = new SettableLight(lights[0]);
            UpTownLeftTurn     = new SettableLight(lights[1]);
            DownTownLeftTurn   = new SettableLight(lights[2]);
            UpDownTown         = new SettableLight(lights[3]);
        }

        public CancellationToken CancellationToken {
            get { return _cancellationTokenSource.Source.Token; }
        } readonly ICancellationTokenAgent _cancellationTokenSource;

        public Image         Red              => RedLight.SetTransparent();
        public Image         Yellow           => YellowLight.SetTransparent();
        public Image         Green            => GreenLight.SetTransparent();

        public SettableLight CrossTown        { get; }
        public SettableLight UpTownLeftTurn   { get; }
        public SettableLight DownTownLeftTurn { get; }
        public SettableLight UpDownTown       { get; }
    }
}
