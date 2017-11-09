using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Target.Utility.Events;
using Target.Utility.ViewModels;

namespace Target.Utility.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point origin;
        private Point startP;

        private bool dragRectangle;
        private List<Line> gridLines;
        private List<JayClass> _jayRectangles;

        public MainWindow()
        {
            InitializeComponent();
            Bootstrapper.GetEventAggregator().GetEvent<RestoreImageEditionsEvent>().Subscribe(Reset);
            Bootstrapper.GetEventAggregator().GetEvent<Add32PxGridEvent>().Subscribe(Add32PxGrid);
            Bootstrapper.GetEventAggregator().GetEvent<ResetTargetSelectionEvent>().Subscribe(ResetTargetSelection);
            Bootstrapper.GetEventAggregator().GetEvent<SliceImageEvent>().Subscribe(SliceImage);

            this.DataContext = new SplitImagViewViewModel();
            this.gridLines = new List<Line>();


            var group = new TransformGroup();
            var st = new ScaleTransform();
            var tt = new TranslateTransform();
            group.Children.Add(st);
            group.Children.Add(tt);
            this.Image.RenderTransform = group;
            this.Image.RenderTransformOrigin = new Point(0.0, 0.0);

            this._jayRectangles = new List<JayClass>();


            if (Environment.UserName.ToLower().Contains("jer") || Environment.UserName.ToLower().Contains("jay") ||
                Environment.UserName.ToLower().Contains("jér"))
            {
                MessageBox.Show("Attention, ordinateur lent détecté. Le soft passera en mode low power consumption");
            }
        }

        private void Add32PxGrid()
        {
            var horLinesCount = this.Canvas.ActualHeight / 32;
            var verLinesCount = this.Canvas.ActualWidth / 32;

            for (int i = 0; i < horLinesCount; i++)
            {
                var line = new Line(); line.Stroke = Brushes.Firebrick;
                line.X1 = 0;
                line.X2 = this.Canvas.ActualWidth;
                line.Y1 = i * 32;
                line.Y2 = i * 32;
                line.StrokeThickness = 1;

                gridLines.Add(line);
                this.Canvas.Children.Add(line);
            }

            for (int i = 0; i < verLinesCount; i++)
            {
                var line = new Line(); line.Stroke = Brushes.Firebrick;
                line.X1 = i * 32;
                line.X2 = i * 32;
                line.Y1 = 0;
                line.Y2 = this.Canvas.ActualHeight;
                line.StrokeThickness = 1;

                gridLines.Add(line);
                this.Canvas.Children.Add(line);
            }
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            var path = e.Data.GetData("FileNameW") as string[];
            if (path != null)
            {
                ImageController.Instance.LoadImage(path[0]);
            }
        }

        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public void Reset()
        {
            if (this.Image != null)
            {
                // reset zoom
                var st = GetScaleTransform(this.Image);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                //var tt = GetTranslateTransform(this.Image);
                //tt.X = 0.0;
                //tt.Y = 0.0;
            }
        }

        public void ResetTargetSelection()
        {
            foreach (var jayRect in _jayRectangles)
            {
                this.Canvas.Children.Remove(jayRect.Rectangle);
            }
            _jayRectangles = new List<JayClass>();
        }

        public List<TargetSelectionRegion> GetSelectionPoints()
        {
            var selections = new List<TargetSelectionRegion>();
            foreach (var jayRectangle in _jayRectangles)
            {
                var selection = new TargetSelectionRegion();

                var ttr = GetTranslateTransform(jayRectangle.Rectangle);
                var tt = GetTranslateTransform(this.Image);
                double topRect = Canvas.GetTop(jayRectangle.Rectangle) + (ttr.Y - tt.Y);
                double leftRect = Canvas.GetLeft(jayRectangle.Rectangle) + (ttr.X - tt.X);

                var startX = leftRect;
                var startY = topRect;

                selection.StartPixel = new System.Drawing.Point((int)startX, (int)startY);

                var rectWidth = ((Rectangle)jayRectangle.Rectangle).ActualWidth;
                var rectHeight = ((Rectangle)jayRectangle.Rectangle).ActualHeight;

                var endX = Math.Round(rectWidth + startX);
                var endY = Math.Round(rectHeight + startY);

                selection.EndPixel = new System.Drawing.Point((int)endX, (int)endY);
                selections.Add(selection);
            }

            return selections;
        }

        private void Image_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
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
            if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var tt = GetTranslateTransform(this.Image);
                startP = e.GetPosition(this.Canvas);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                this.Image.CaptureMouse();

                foreach (var jayrect in _jayRectangles)
                {
                    var ttr = GetTranslateTransform(jayrect.Rectangle);
                    jayrect.Start = e.GetPosition(this.Canvas);
                    jayrect.Origine = new Point(ttr.X, ttr.Y);
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = false;
            }
        }

        private void Image_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                this.Image.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = false;
            }
        }

        private void Image_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Reset();
        }

        private void Image_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (this.Image.IsMouseCaptured)
                {
                    var tt = GetTranslateTransform(this.Image);
                    Vector v = startP - e.GetPosition(this.Canvas);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;

                    foreach (var jayrect in _jayRectangles)
                    {
                        var tr = GetTranslateTransform(jayrect.Rectangle);
                        Vector vr = startP - e.GetPosition(this.Canvas);
                        tr.X = jayrect.Origine.X - vr.X;
                        tr.Y = jayrect.Origine.Y - vr.Y;
                    }
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = false;
            }
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var jayRect = new Rectangle();
                jayRect.Stroke = Brushes.Red;

                var groupR = new TransformGroup();
                var stR = new ScaleTransform();
                var ttR = new TranslateTransform();
                groupR.Children.Add(stR);
                groupR.Children.Add(ttR);
                jayRect.RenderTransform = groupR;
                jayRect.RenderTransformOrigin = new Point(0.0, 0.0);

                this.dragRectangle = true;
                var start = e.GetPosition(this.Canvas);
                Canvas.SetTop(jayRect, start.Y);
                Canvas.SetLeft(jayRect, start.X);

                this.Canvas.Children.Add(jayRect);
                this._jayRectangles.Add(new JayClass{Start = start, Rectangle = jayRect});
            }
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dragRectangle = false;
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (dragRectangle)
            {
                var jayclass = this._jayRectangles.LastOrDefault();
                var pos = e.GetPosition(this.Canvas);
                var x = Math.Min(pos.X, jayclass.Start.X);
                var y = Math.Min(pos.Y, jayclass.Start.Y);

                var w = Math.Max(pos.X, jayclass.Start.X) - x;
                var h = Math.Max(pos.Y, jayclass.Start.Y) - y;

                ((Rectangle)jayclass.Rectangle).Width = w;
                ((Rectangle)jayclass.Rectangle).Height = h;

                Canvas.SetTop(jayclass.Rectangle, y);
                Canvas.SetLeft(jayclass.Rectangle, x);
            }
        }

        private void SliceImage()
        {
            // get rectangle position in image
            MessageBox.Show("Le slicing commence. P.S. vous avez surment plus de processing power que le foetus laptop à Jay.");
            ImageController.Instance.SliceImage(this.GetSelectionPoints());
            MessageBox.Show("Fini. En passant la blonde à Jay sera contente en criss s'il gagne 1 pouce");
        }

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            //// Move selection
            //var p = e.GetPosition(this.PreviewSelectionRectangle);
            //Canvas.SetTop(this.PreviewSelectionRectangle, p.Y);
            //Canvas.SetLeft(this.PreviewSelectionRectangle, p.X);
        }
    }
}
