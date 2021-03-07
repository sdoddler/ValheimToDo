using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using GameOverlay.Drawing;
using GameOverlay.Windows;
using ValheimToDo;
using System.Linq;

namespace ValheimToDo
{
    public class ValheimOverlay : IDisposable
    {
        private readonly Dictionary<string, SolidBrush> _brushes;
        private readonly Dictionary<string, Font> _fonts;
        private readonly Dictionary<string, Image> _images;

        private List<Action<Graphics, float, float>> _randomFigures;

        private readonly SharedData _sharedData;

        public ValheimOverlay(SharedData sharedData)
        {
            _brushes = new Dictionary<string, SolidBrush>();
            _fonts = new Dictionary<string, Font>();
            _images = new Dictionary<string, Image>();

            _sharedData = sharedData;

            var gfx = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };



            Window = new GraphicsWindow(0 + _sharedData.monitorOffsetX + _sharedData.userOffsetX,  (int)Math.Round(_sharedData.monitorHeight/10f) + _sharedData.monitorOffsetY + _sharedData.userOffsetY, 500, 600, gfx)
            {
                FPS = 60,
                IsTopmost = true,
                IsVisible = true

            };

            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
            Window.SetupGraphics += _window_SetupGraphics;
        }

        public GraphicsWindow Window { get; }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (e.RecreateResources)
            {
                foreach (var pair in _brushes) pair.Value.Dispose();
                foreach (var pair in _images) pair.Value.Dispose();
            }

            _brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
            _brushes["black-transparent"] = gfx.CreateSolidBrush(0, 0, 0, 20);
            _brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
            _brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
            _brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
            _brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
            _brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F, 130);
            _brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.2f);
            _brushes["random"] = gfx.CreateSolidBrush(0, 0, 0);

            if (e.RecreateResources) return;

            _fonts["arial"] = gfx.CreateFont("Arial", 12);
            _fonts["consolas"] = gfx.CreateFont("Consolas", 14);
            _fonts["comic sans"] = gfx.CreateFont("Comic Sans MS", 28);


        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            foreach (var pair in _brushes) pair.Value.Dispose();
            foreach (var pair in _fonts) pair.Value.Dispose();
            foreach (var pair in _images) pair.Value.Dispose();
        }


        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {



            var gfx = e.Graphics;

            string bkColor = _sharedData.bkColor.Value.ToString();
            if (!_brushes.ContainsKey(bkColor))
                _brushes.Add(bkColor, gfx.CreateSolidBrush(_sharedData.bkColor.Value.R, _sharedData.bkColor.Value.G, _sharedData.bkColor.Value.B, _sharedData.bkColor.Value.A));



            gfx.ClearScene(_brushes[bkColor]);


            var inc = 0;
            var indent = 20;



            foreach (ValheimToDo.ToDo t in _sharedData.toDos.ToList())//_sharedData.toDos)
            {
                if (t.IsSelected)
                    continue;
                string color = t.O_Color.ToString();
                if (!_brushes.ContainsKey(color))
                {
                    Debug.WriteLine(color);
                    _brushes.Add(color, gfx.CreateSolidBrush(t.O_Color.R, t.O_Color.G, t.O_Color.B, t.O_Color.A));
                }



                var incX = 0;
                string amountXTitle = t.Amount + " x " + t.Title;


                gfx.DrawText(_fonts["arial"], 18, _brushes[color], 20, 40 + inc, amountXTitle);
                gfx.DrawText(_fonts["arial"], 18, _brushes[color], 280, 40 + inc, t.Location);
                inc += 20;
                if (t.Components != null)
                    if (t.Components.Count > 0)
                    {
                        int skip = 0;
                        foreach (ValComponent c in t.Components)
                        {
                            if (c.IsSelected) continue;
                            gfx.DrawText(_fonts["consolas"], _fonts["consolas"].FontSize, _brushes[color], 20 + indent + incX, 40 + inc, c.Amount + " x " + c.Title);
                            incX += 175;
                            skip++;
                            if (skip == 2)
                            {
                                incX = 0;
                                skip = 0;
                                inc += 20;
                            }
                        }
                        if (skip == 1) inc += 30;
                    }
            }
        }



        public void Run()
        {
            Window.Create();
            Window.Join();
        }

        ~ValheimOverlay()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Window.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void RefreshWindowLocation()
        {

            Window.Move(0 + _sharedData.monitorOffsetX + _sharedData.userOffsetX, (int)Math.Round(_sharedData.monitorHeight / 10f) + _sharedData.monitorOffsetY + _sharedData.userOffsetY);
        }
        #endregion
    }
}