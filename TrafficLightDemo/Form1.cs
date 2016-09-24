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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGSolutions.Monads.TrafficLightDemo {
    using static Contract;
    using LightStates = LightStates<SettableLight,ITrafficLight<SettableLight,Image>>;

    public interface ICancellationTokenAgent {
        CancellationTokenSource Source { get; }
    }

    public partial class Form1 : Form, ICancellationTokenAgent {
        public Form1() {
            Ensures( _cts != null );

            InitializeComponent();

            _cts = new CancellationTokenSource();   Assert(_cts != null);

            this.AssumeInvariant();
            _trafficLight = new TrafficLight(this,
                    CrossTownLight,
                    UpTownLeftTurnLight,
                    DownTownLeftTurnLight,
                    UpDownTownLight
            );
        }

        readonly TrafficLight _trafficLight;
    
        private void Form1_Load(object sender, EventArgs e) { ResetLights(sender,e); }

        private async void ResetLights(object sender, EventArgs e) {
            if (_cts != null) _cts.Cancel();
            _cts = new CancellationTokenSource();
            await StartLightsAsync();
        }

        private async Task<Unit> StartLightsAsync() => await new LightStates().Run(_trafficLight);

        public CancellationTokenSource Source {
            get {Ensures(Result<CancellationTokenSource>() != null); return _cts; }
        }
        CancellationTokenSource _cts;

        /// <summary>The invariants enforced by this struct type.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        [Pure]
        private void ObjectInvariant() {
          Invariant(UpTownLeftTurnLight   != null);
          Invariant(DownTownLeftTurnLight != null);
          Invariant(CrossTownLight        != null);
          Invariant(UpDownTownLight       != null);

          Invariant(label1 != null);
          Invariant(label2 != null);
          Invariant(label3 != null);
          Invariant(label4 != null);

          Invariant( _cts != null );
        }
    }
}
