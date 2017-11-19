using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Target.Utility.Controllers;
using Target.Utility.Core;
using Target.Utility.Events;
using Target.Utility.Models;
using Target.Utility.Properties;
using Target.Utility.ViewModels;

namespace Target.Utility.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Point origin;
        //private Point startP;

        private List<Line> gridLines;
        private List<JayClass> _jayRectangles;

        public MainWindow()
        {
            InitializeComponent();
            Bootstrapper.GetEventAggregator().GetEvent<Add32PxGridEvent>().Subscribe(Add32PxGrid);
            Bootstrapper.GetEventAggregator().GetEvent<ResetTargetSelectionEvent>().Subscribe(ResetTargetSelection);

            this.DataContext = new SplitImagViewViewModel(this);
            this.gridLines = new List<Line>();

            //var group = new TransformGroup();
            //var st = new ScaleTransform();
            //var tt = new TranslateTransform();
            //group.Children.Add(st);
            //group.Children.Add(tt);
            //this.Image.RenderTransform = group;
            //this.Image.RenderTransformOrigin = new Point(0.0, 0.0);

            this._jayRectangles = new List<JayClass>();


            if (Environment.UserName.ToLower().Contains("jer") || Environment.UserName.ToLower().Contains("jay") ||
                Environment.UserName.ToLower().Contains("jér"))
            {
                MessageBox.Show("Attention, ordinateur lent détecté. Le soft passera en mode low power consumption");
            }
        }

        private void Add32PxGrid()
        {
            var m = Settings.Default.ResizeMultiple;
            var horLinesCount = this.Canvas.ActualHeight / m;
            var verLinesCount = this.Canvas.ActualWidth / m;

            for (int i = 0; i < horLinesCount; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Firebrick,
                    X1 = 0,
                    X2 = this.Canvas.ActualWidth,
                    Y1 = i * m,
                    Y2 = i * m,
                    StrokeThickness = 1
                };
                gridLines.Add(line);
                this.Canvas.Children.Add(line);
            }

            for (int i = 0; i < verLinesCount; i++)
            {
                var line = new Line
                {
                    Stroke = Brushes.Firebrick,
                    X1 = i * m,
                    X2 = i * m,
                    Y1 = 0,
                    Y2 = this.Canvas.ActualHeight,
                    StrokeThickness = 1
                };
                gridLines.Add(line);
                this.Canvas.Children.Add(line);
            }
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            //var path = e.Data.GetData("FileNameW") as string[];
            //if (path != null)
            //{
            //    // todo in vm
            //    throw new NotImplementedException();
            //}
        }

        //private TranslateTransform GetTranslateTransform(UIElement element)
        //{
        //    return (TranslateTransform)((TransformGroup)element.RenderTransform)
        //      .Children.First(tr => tr is TranslateTransform);
        //}

        //private ScaleTransform GetScaleTransform(UIElement element)
        //{
        //    return (ScaleTransform)((TransformGroup)element.RenderTransform)
        //      .Children.First(tr => tr is ScaleTransform);
        //}

        //public void Reset()
        //{
        //    if (this.Image != null)
        //    {
        //        // reset zoom
        //        var st = GetScaleTransform(this.Image);
        //        st.ScaleX = 1.0;
        //        st.ScaleY = 1.0;

        //        // reset pan
        //        //var tt = GetTranslateTransform(this.Image);
        //        //tt.X = 0.0;
        //        //tt.Y = 0.0;
        //    }
        //}

        public void ResetTargetSelection()
        {
            foreach (var jayRect in _jayRectangles)
            {
                this.Canvas.Children.Remove(jayRect.Rectangle);
            }
            _jayRectangles = new List<JayClass>();
        }
        public void RemoveLastSelection()
        {
            var jayRect = this._jayRectangles.LastOrDefault();
            this.Canvas.Children.Remove(jayRect?.Rectangle);
            this._jayRectangles.Remove(jayRect);
        }

        /// <summary>
        /// Used if translation is enabled. If Zoom is enabled too, we'll have to update this
        /// </summary>
        /// <returns></returns>
        //public List<TargetSelectionRegion> GetSelectionPoints()
        //{
        //    var selections = new List<TargetSelectionRegion>();
        //    foreach (var jayRectangle in _jayRectangles)
        //    {
        //        var selection = new TargetSelectionRegion();

        //        var ttr = GetTranslateTransform(jayRectangle.Rectangle);
        //        var tt = GetTranslateTransform(this.Image);
        //        double topRect = Canvas.GetTop(jayRectangle.Rectangle) + (ttr.Y - tt.Y);
        //        double leftRect = Canvas.GetLeft(jayRectangle.Rectangle) + (ttr.X - tt.X);

        //        var startX = leftRect;
        //        var startY = topRect;

        //        selection.StartPixel = new System.Drawing.Point((int)startX, (int)startY);

        //        var rectWidth = ((Rectangle)jayRectangle.Rectangle).ActualWidth;
        //        var rectHeight = ((Rectangle)jayRectangle.Rectangle).ActualHeight;

        //        var endX = Math.Round(rectWidth + startX);
        //        var endY = Math.Round(rectHeight + startY);

        //        selection.EndPixel = new System.Drawing.Point((int)endX, (int)endY);
        //        selections.Add(selection);
        //    }

        //    return selections;
        //}

        private void Image_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                // todo this is not cool
                ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize += 2;

                var m = ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize;
                var pos = e.GetPosition(this.Image);
                var offset = m / 2;
                Canvas.SetTop(this.PreviewSelectionRectangle, pos.Y - offset);
                Canvas.SetLeft(this.PreviewSelectionRectangle, pos.X - offset);
            }
            else
            {
                // todo this is not cool
                ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize -= 2;

                var m = ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize;
                var pos = e.GetPosition(this.Image);
                var offset = m / 2;
                Canvas.SetTop(this.PreviewSelectionRectangle, pos.Y - offset);
                Canvas.SetLeft(this.PreviewSelectionRectangle, pos.X - offset);
            }
            //if (this.Image != null)
            //{
            //    var st = GetScaleTransform(this.Image);
            //    var tt = GetTranslateTransform(this.Image);

            //    double zoom = e.Delta > 0 ? .1 : -.1;
            //    if (!(e.Delta > 0) && (st.ScaleX < .25 || st.ScaleY < .25))
            //        return;

            //    Point relative = e.GetPosition(this.Image);
            //    double abosuluteX;
            //    double abosuluteY;

            //    abosuluteX = relative.X * st.ScaleX + tt.X;
            //    abosuluteY = relative.Y * st.ScaleY + tt.Y;

            //    st.ScaleX += zoom;
            //    st.ScaleY += zoom;

            //    tt.X = abosuluteX - relative.X * st.ScaleX;
            //    tt.Y = abosuluteY - relative.Y * st.ScaleY;

            //    var str = GetScaleTransform(this.TargetSelectionRectanlge);
            //    var ttr = GetTranslateTransform(this.TargetSelectionRectanlge);

            //    Point relativerPosition = e.GetPosition(this.TargetSelectionRectanlge);
            //    double abosuluteXr;
            //    double abosuluteYr;

            //    abosuluteXr = relativerPosition.X * str.ScaleX + ttr.X;
            //    abosuluteYr = relativerPosition.Y * str.ScaleY + ttr.Y;

            //    str.ScaleX += zoom;
            //    str.ScaleY += zoom;

            //    ttr.X = abosuluteXr - relativerPosition.X * str.ScaleX;
            //    ttr.Y = abosuluteYr - relativerPosition.Y * str.ScaleY;
            //}
        }

        private void Image_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var m = ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize;
            var jayRect = new Rectangle
            {
                Stroke = Brushes.Red,
                Width = m,
                Height = m,
                RenderTransformOrigin = new Point(0.0, 0.0)
            };

            //var groupR = new TransformGroup();
            //var stR = new ScaleTransform();
            //var ttR = new TranslateTransform();
            //groupR.Children.Add(stR);
            //groupR.Children.Add(ttR);
            //jayRect.RenderTransform = groupR;

            var offset = m / 2;
            var clickedPoint = e.GetPosition(this.Image);
            Canvas.SetTop(jayRect, clickedPoint.Y - offset);
            Canvas.SetLeft(jayRect, clickedPoint.X - offset);

            var start = new Point(clickedPoint.X - offset, clickedPoint.Y - offset);
            this.Canvas.Children.Add(jayRect);
            this._jayRectangles.Add(new JayClass { Start = start, Rectangle = jayRect });
            //if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    var tt = GetTranslateTransform(this.Image);
            //    startP = e.GetPosition(this.Canvas);
            //    origin = new Point(tt.X, tt.Y);
            //    this.Cursor = Cursors.Hand;
            //    this.Image.CaptureMouse();

            //    foreach (var jayrect in _jayRectangles)
            //    {
            //        var ttr = GetTranslateTransform(jayrect.Rectangle);
            //        jayrect.Start = e.GetPosition(this.Canvas);
            //        jayrect.Origine = new Point(ttr.X, ttr.Y);
            //    }
            //}
            //else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    e.Handled = false;
            //}
        }

        private void Image_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    this.Image.ReleaseMouseCapture();
            //    this.Cursor = Cursors.Arrow;
            //}
            //else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    e.Handled = false;
            //}
        }

        private void Image_OnMouseMove(object sender, MouseEventArgs e)
        {
            //if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    if (this.Image.IsMouseCaptured)
            //    {
            //        var tt = GetTranslateTransform(this.Image);
            //        Vector v = startP - e.GetPosition(this.Canvas);
            //        tt.X = origin.X - v.X;
            //        tt.Y = origin.Y - v.Y;

            //        foreach (var jayrect in _jayRectangles)
            //        {
            //            var tr = GetTranslateTransform(jayrect.Rectangle);
            //            Vector vr = startP - e.GetPosition(this.Canvas);
            //            tr.X = jayrect.Origine.X - vr.X;
            //            tr.Y = jayrect.Origine.Y - vr.Y;
            //        }
            //    }
            //}
            //else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //{
            //    e.Handled = false;
            //}
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this.Canvas);
            var m = ((SplitImagViewViewModel)this.DataContext).SelectionSquaredSize;
            var offset = m / 2;
            Canvas.SetTop(this.PreviewSelectionRectangle, pos.Y - offset);
            Canvas.SetLeft(this.PreviewSelectionRectangle, pos.X - offset);
        }

        /// <summary>
        /// this method should only be used if no translation and no zoom
        /// </summary>
        /// <returns></returns>
        public List<TargetSelectionRegion> GetSelections()
        {
            var selections = new List<TargetSelectionRegion>();
            foreach (var jayRectangle in _jayRectangles)
            {
                var selection = new TargetSelectionRegion();

                var startY = Canvas.GetTop(jayRectangle.Rectangle);
                var startX = Canvas.GetLeft(jayRectangle.Rectangle);
                //If selection is out of the image bound, we move it so it will fit in the image boundaries
                var offsetY = startY < 0 ? Math.Abs(startY) : 0; // if selection is before image
                var offsetX = startX < 0 ? Math.Abs(startX) : 0; // if selection is before image 

                selection.StartPixel = new System.Drawing.Point((int)(startX + offsetX), (int)(startY + offsetY));

                var rectWidth = ((Rectangle)jayRectangle.Rectangle).ActualWidth;
                var rectHeight = ((Rectangle)jayRectangle.Rectangle).ActualHeight;

                var endX = Math.Round(rectWidth + startX + offsetX);
                var endY = Math.Round(rectHeight + startY + offsetY);

                selection.EndPixel = new System.Drawing.Point((int)endX, (int)endY);
                selections.Add(selection);
            }

            return selections;
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            //// Move selection
            //var p = e.GetPosition(this.PreviewSelectionRectangle);
            //Canvas.SetTop(this.PreviewSelectionRectangle, p.Y);
            //Canvas.SetLeft(this.PreviewSelectionRectangle, p.X);
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            UserController.ShowSelectUserWindow();
        }
    }
}
