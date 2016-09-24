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
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PGSolutions.Monads.TrafficLightDemo {
    using ITrafficLight = ITrafficLight<SettableLight,Image>;
    using static Properties.Resources;
    using static Contract;

    internal static class Extensions {
        [Pure]
        public static Bitmap SetTransparent(this Bitmap @this) {
            @this.ContractedNotNull(nameof(@this));
            Ensures(Result<Bitmap>() != null);

            @this.MakeTransparent(@this.GetPixel(1, 1));
            return @this;
        }
    }

    struct TrafficLight : ITrafficLight {
        public TrafficLight(ICancellationTokenAgent ctsSource, params PictureBox[] lights) : this() {
            ctsSource.ContractedNotNull(nameof(ctsSource));
            lights.ContractedNotNull(nameof(lights));
            Requires(3 < lights.Length);
            lights[0].ContractedNotNull("lights[0]");
            lights[1].ContractedNotNull("lights[1]");
            lights[2].ContractedNotNull("lights[2]");
            lights[3].ContractedNotNull("lights[3]");

            Ensures(_cancellationTokenSource        != null);
            Ensures(_cancellationTokenSource.Source != null);

            _cancellationTokenSource = ctsSource;
            _crossTownLight          = new SettableLight(lights[0]);
            _upTownLeftTurnLight     = new SettableLight(lights[1]);
            _downTownLeftTurnLight   = new SettableLight(lights[2]);
            _upDownTownLight         = new SettableLight(lights[3]);
        }

        public CancellationToken CancellationToken {
            get { return _cancellationTokenSource.Source.Token; }
        } readonly ICancellationTokenAgent _cancellationTokenSource;

        public Image         Red              {
            [Pure]get { return _red; }
        } static readonly Image _red    = RedLight.SetTransparent();
        public Image         Yellow           {
            [Pure]get { return _yellow; }
        } static readonly Image _yellow = YellowLight.SetTransparent();
        public Image         Green            {
            [Pure]get { return _green; }
        } static readonly Image _green  = GreenLight.SetTransparent();

        public SettableLight CrossTown        {
            get { return _crossTownLight; }
        } readonly SettableLight _crossTownLight;
        public SettableLight UpTownLeftTurn   {
            get { return _upTownLeftTurnLight; }
        } readonly SettableLight _upTownLeftTurnLight;
        public SettableLight DownTownLeftTurn {
            get { return _downTownLeftTurnLight; }
        } readonly SettableLight _downTownLeftTurnLight;
        public SettableLight UpDownTown       {
            get { return _upDownTownLight; }
        } readonly SettableLight _upDownTownLight;

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
            Invariant(_cancellationTokenSource             != null);
            Invariant(_cancellationTokenSource.Source      != null);
        }
    }
}
