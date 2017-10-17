using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        private Point start;

        private Point originRectangle;
        private Point startRectangle;
        private bool dragRectangle;
        private List<Line> gridLines;

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


            var groupR = new TransformGroup();
            var stR = new ScaleTransform();
            var ttR = new TranslateTransform();
            groupR.Children.Add(stR);
            groupR.Children.Add(ttR);
            this.TargetSelectionRectanlge.RenderTransform = groupR;
            this.TargetSelectionRectanlge.RenderTransformOrigin = new Point(0.0, 0.0);
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
            var st = GetScaleTransform(this.TargetSelectionRectanlge);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            var tt = GetTranslateTransform(this.TargetSelectionRectanlge);
            tt.X = 0.0;
            tt.Y = 0.0;
            this.TargetSelectionRectanlge.Width = 0;
            this.TargetSelectionRectanlge.Height = 0;

            this.TargetSelectionRectanlge.Visibility = Visibility.Collapsed;
        }

        public TargetSelectionRegion GetSelectionPoints()
        {
            var selection = new TargetSelectionRegion();

            var ttr = GetTranslateTransform(this.TargetSelectionRectanlge);
            var tt = GetTranslateTransform(this.Image);
            double topRect = Canvas.GetTop(this.TargetSelectionRectanlge) + (ttr.Y - tt.Y);
            double leftRect = Canvas.GetLeft(this.TargetSelectionRectanlge) + (ttr.X - tt.X);

            var startX = leftRect;
            var startY = topRect;

            selection.StartPixel = new System.Drawing.Point((int)startX, (int)startY);

            var rectWidth = this.TargetSelectionRectanlge.ActualWidth;
            var rectHeight = this.TargetSelectionRectanlge.ActualHeight;

            var endX = Math.Round(rectWidth + startX);
            var endY = Math.Round(rectHeight + startY);

            selection.EndPixel = new System.Drawing.Point((int)endX, (int)endY);

            return selection;
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
            //    //var ttr = GetTranslateTransform(this.TargetSelectionRectanlge);

            //    str.ScaleX += zoom;
            //    str.ScaleY += zoom;

            //    //ttr.X = tt.X;
            //    //ttr.Y = tt.Y;
            //}
        }

        private void Image_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Image != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var tt = GetTranslateTransform(this.Image);
                start = e.GetPosition(this.Canvas);
                origin = new Point(tt.X, tt.Y);
                this.Cursor = Cursors.Hand;
                this.Image.CaptureMouse();

                var ttr = GetTranslateTransform(this.TargetSelectionRectanlge);
                startRectangle = e.GetPosition(this.Canvas);
                originRectangle = new Point(ttr.X, ttr.Y);
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
                    Vector v = start - e.GetPosition(this.Canvas);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;

                    if (this.TargetSelectionRectanlge.Visibility == Visibility.Visible)
                    {
                        var tr = GetTranslateTransform(this.TargetSelectionRectanlge);
                        Vector vr = start - e.GetPosition(this.Canvas);
                        tr.X = originRectangle.X - vr.X;
                        tr.Y = originRectangle.Y - vr.Y;
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
                this.ResetTargetSelection();
                this.dragRectangle = true;
                this.TargetSelectionRectanlge.Visibility = Visibility.Visible;
                startRectangle = e.GetPosition(this.Canvas);
                Canvas.SetTop(this.TargetSelectionRectanlge, startRectangle.Y);
                Canvas.SetLeft(this.TargetSelectionRectanlge, startRectangle.X);
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
                var pos = e.GetPosition(this.Canvas);
                var x = Math.Min(pos.X, startRectangle.X);
                var y = Math.Min(pos.Y, startRectangle.Y);

                var w = Math.Max(pos.X, startRectangle.X) - x;
                var h = Math.Max(pos.Y, startRectangle.Y) - y;

                this.TargetSelectionRectanlge.Width = w;
                this.TargetSelectionRectanlge.Height = h;

                Canvas.SetTop(this.TargetSelectionRectanlge, y);
                Canvas.SetLeft(this.TargetSelectionRectanlge, x);
            }
        }

        private void SliceImage()
        {
            // get rectangle position in image
            ImageController.Instance.SliceImage(this.GetSelectionPoints());
        }

    }
}
