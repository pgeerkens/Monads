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

using PGSolutions.Utilities.Monads;

namespace TrafficLightDemo {
  using ITrafficLight   = ITrafficLight<Image>;
  using ISettableLight  = ISettableLight<Image>;

  public interface ICancellationTokenSource {
      CancellationTokenSource Source { get; }
  }

  public partial class Form1 : Form, ICancellationTokenSource {
    public Form1() {
      Contract.Ensures( (Source == null) == (_cts == null) );
      Contract.Ensures( _cts != null );

      InitializeComponent();

      _cts = new CancellationTokenSource();

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

    public CancellationTokenSource Source {
      get {Contract.Ensures(Contract.Result<CancellationTokenSource>() != null); return _cts; }
    }
    CancellationTokenSource _cts;

    static readonly Image _red    = SetTransparent(Properties.Resources.RedLight);
    static readonly Image _yellow = SetTransparent(Properties.Resources.YellowLight);
    static readonly Image _green  = SetTransparent(Properties.Resources.GreenLight);

    /// <summary>The invariants enforced by this struct type.</summary>
    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [ContractInvariantMethod]
    [Pure]
    private void ObjectInvariant() {
      Contract.Invariant(UpTownLeftTurnLight   != null);
      Contract.Invariant(DownTownLeftTurnLight != null);
      Contract.Invariant(CrossTownLight        != null);
      Contract.Invariant(UpDownTownLight       != null);

      Contract.Invariant(label1 != null);
      Contract.Invariant(label2 != null);
      Contract.Invariant(label3 != null);
      Contract.Invariant(label4 != null);

      Contract.Invariant( (Source == null) == (_cts == null) );
      Contract.Invariant( _cts != null );
    }

    struct SettableLight : ISettableLight {
        public SettableLight(PictureBox pictureBox) : this() {
          pictureBox.ContractedNotNull("pictureBox");
          Contract.Ensures(_pictureBox != null);

          _pictureBox = pictureBox;
        }

            PGSolutions.Utilities.Monads.IO2.IO<Unit> ISettableLight.SetColour(Image image) {
                _pictureBox.Image = image;
               // return Unit._.ToIO();
                return PGSolutions.Utilities.Monads.IO2.IO.ToIO(Unit._);
            }
        readonly PictureBox _pictureBox;

      /// <summary>The invariants enforced by this struct type.</summary>
      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
      [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ContractInvariantMethod]
      [Pure]
      private void ObjectInvariant() {
          Contract.Invariant( _pictureBox != null );
      }
    }

    struct TrafficLight : ITrafficLight<Image> {
      public TrafficLight(ICancellationTokenSource ctsSource, params PictureBox[] lights) : this() {
        ctsSource.ContractedNotNull("ctsSource");
        Contract.Requires(ctsSource.Source != null);
        lights.ContractedNotNull("lights");
        Contract.Requires(3 < lights.Length);
        Contract.Requires( lights[0] != null );
        Contract.Requires( lights[1] != null );
        Contract.Requires( lights[2] != null );
        Contract.Requires( lights[3] != null );

        Contract.Ensures(_ctsSource != null);
        Contract.Ensures(_ctsSource.Source != null);

        Contract.Ensures( _crossTownLight != null );
        Contract.Ensures( _upTownLeftTurnLight != null );
        Contract.Ensures( _downTownLeftTurnLight != null );
        Contract.Ensures( _upDownTownLight != null );

        _ctsSource             = ctsSource;
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

      /// <summary>The invariants enforced by this struct type.</summary>
      [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
      [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      [ContractInvariantMethod]
      [Pure]
      private void ObjectInvariant() {
          Contract.Invariant( _ctsSource != null );
          Contract.Invariant( _ctsSource.Source != null );

          Contract.Invariant( _crossTownLight != null );
          Contract.Invariant( _upTownLeftTurnLight != null );
          Contract.Invariant( _downTownLeftTurnLight != null );
          Contract.Invariant( _upDownTownLight != null );
      }
    }
    #endregion
  }
}
