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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PGSolutions.Utilities.Monads;

namespace TrafficLightDemo {
  using ITrafficLight   = ITrafficLight<Image>;
  using ISettableLight  = ISettableLight<Image>;

  public interface ICancellationTokenSource {
      CancellationTokenSource Source { get; }
  }

  public partial class Form1 : Form, ICancellationTokenSource {
    public Form1() {
      InitializeComponent();

      _trafficLight = new TrafficLight(this,
              CrossTownLight,
              UpTownLeftTurnLight,
              DownTownLeftTurnLight,
              UpDownTownLight
            );
    }

    TrafficLight _trafficLight;
    
    private void Form1_Load(object sender, EventArgs e) { ResetLights(sender,e); }

    private async void ResetLights(object sender, EventArgs e) {
        if (_cts != null) _cts.Cancel();
        _cts = new CancellationTokenSource();
        await StartLights();
    }

    private async Task<Unit> StartLights() {
        return await LightStates.ExecuteTrafficLight<Image>(_trafficLight);
    }
    #region Implementation
    [Pure]
    static Bitmap SetTransparent(Bitmap bitmap) {
        bitmap.ContractedNotNull("bitmap");
        Contract.Ensures(Contract.Result<Bitmap>() != null);

        bitmap.MakeTransparent(bitmap.GetPixel(1,1));
        return bitmap;
    }

    [Pure]
    static Unit SetLight(PictureBox light, Image lightColour) {
        light.ContractedNotNull("light");
        light.Image = lightColour;
        return Unit._;
    }

    CancellationTokenSource ICancellationTokenSource.Source { get { return _cts; } }
    CancellationTokenSource _cts;

    static readonly Image _red    = SetTransparent(Properties.Resources.RedLight);
    static readonly Image _yellow = SetTransparent(Properties.Resources.YellowLight);
    static readonly Image _green  = SetTransparent(Properties.Resources.GreenLight);

    struct SettableLight : ISettableLight {
        public SettableLight(PictureBox pictureBox) { _pictureBox = pictureBox; }
        IO<Unit> ISettableLight.SetColour(Image image) { _pictureBox.Image = image; return Unit._.ToIO(); }
        readonly PictureBox _pictureBox;
    }

    struct TrafficLight : ITrafficLight<Image> {
      public TrafficLight(ICancellationTokenSource ctsSource, params PictureBox[] lights) : this() {
        _ctsSource = ctsSource;
        _crossTownLight        = lights[0];
        _upTownLeftTurnLight   = lights[1];
        _downTownLeftTurnLight = lights[2];
        _upDownTownLight       = lights[3];
      }

      readonly ICancellationTokenSource _ctsSource;
      readonly PictureBox _crossTownLight;
      readonly PictureBox _upTownLeftTurnLight;
      readonly PictureBox _downTownLeftTurnLight;
      readonly PictureBox _upDownTownLight;

      public CancellationToken CancellationToken    { get { return _ctsSource.Source.Token; } }

      Image          ITrafficLight.Red              { [Pure]get {return _red;   } }
      Image          ITrafficLight.Yellow           { [Pure]get {return _yellow;} }
      Image          ITrafficLight.Green            { [Pure]get {return _green; } }

      ISettableLight ITrafficLight.CrossTown        { [Pure]get {return new SettableLight(_crossTownLight);       } }
      ISettableLight ITrafficLight.UpTownLeftTurn   { [Pure]get {return new SettableLight(_upTownLeftTurnLight);  } }
      ISettableLight ITrafficLight.DownTownLeftTurn { [Pure]get {return new SettableLight(_downTownLeftTurnLight);} }
      ISettableLight ITrafficLight.UpDownTown       { [Pure]get {return new SettableLight(_upDownTownLight);      } }
    }
    #endregion
  }
}
