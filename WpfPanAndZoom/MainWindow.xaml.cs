﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfPanAndZoom
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // https://stackoverflow.com/questions/35165349/how-to-drag-rendertransform-with-mouse-in-wpf

        #region Variables
        private readonly MatrixTransform transform = new MatrixTransform();
        private Point pressedMouse;

        bool dragging;
        UIElement selectedElement;
        Vector draggingDelta;
        #endregion




        public MainWindow()
        {
            InitializeComponent();

            canvas.MouseWheel += MwGrid_MouseWheel;
            canvas.MouseDown += MwGrid_MouseDown;
            canvas.MouseMove += MwGrid_MouseMove;
            canvas.MouseUp += MwGrid_MouseUp;

            CustomControls.Widget w1 = new CustomControls.Widget();
            w1.Width = 200;
            w1.Height = 150;
            canvas.Children.Add(w1);
            w1.Header.Text = "Widget 1";
            Canvas.SetTop(w1, 100);
            Canvas.SetLeft(w1, 100);

            CustomControls.Widget w2 = new CustomControls.Widget();
            w2.Width = 200;
            w2.Height = 150;
            canvas.Children.Add(w2);
            w2.Header.Text = "Widget 2";
            w2.HeaderRectangle.Fill = Brushes.Blue;
            Canvas.SetTop(w2, 400);
            Canvas.SetLeft(w2, 400);

            CustomControls.Widget w3 = new CustomControls.Widget();
            w3.Width = 200;
            w3.Height = 150;
            canvas.Children.Add(w3);
            w3.Header.Text = "Widget 3";
            w3.HeaderRectangle.Fill = Brushes.Red;
            Canvas.SetTop(w3, 400);
            Canvas.SetLeft(w3, 800);
        }

        private void MwGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragging = false;
            selectedElement = null;
        }

        private void MwGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point mouse = transform.Inverse.Transform(e.GetPosition(this));
                Vector delta = Point.Subtract(mouse, pressedMouse); // delta from old mouse to current mouse
                var translate = new TranslateTransform(delta.X, delta.Y);
                transform.Matrix = translate.Value * transform.Matrix;

                foreach( UIElement child in canvas.Children )
                {
                    child.RenderTransform = transform;
                }
                
            }

            if (dragging && e.LeftButton == MouseButtonState.Pressed)
            {
                double x = Mouse.GetPosition(canvas).X;
                double y = Mouse.GetPosition(canvas).Y;

                if (selectedElement != null)
                {
                    Canvas.SetLeft(selectedElement, x + draggingDelta.X);
                    Canvas.SetTop(selectedElement, y + draggingDelta.Y);
                }
            }

            e.Handled = true;
        }

  

        private void MwGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                pressedMouse = transform.Inverse.Transform(e.GetPosition(this));
            }
            

            if (e.ChangedButton == MouseButton.Left)
            {
                if( canvas.Children.Contains( (UIElement)e.Source))
                {
                    selectedElement = (UIElement)e.Source;
                    Point mousePosition = Mouse.GetPosition(canvas);
                    double x = Canvas.GetLeft(selectedElement);
                    double y = Canvas.GetTop(selectedElement);
                    Point elementPosition = new Point(x, y);
                    draggingDelta = elementPosition - mousePosition; 
                }
                dragging = true;
            }

            e.Handled = true;
        }

      

        private void MwGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float scale = 1.1f;
            if (e.Delta < 0)
            {
                scale = 1f / scale;
            }
            Point mouse = e.GetPosition(this);

            Matrix matrix = transform.Matrix;
            matrix.ScaleAt(scale, scale, mouse.X, mouse.Y);
            transform.Matrix = matrix;

            foreach (UIElement child in canvas.Children)
            {
                double x = Canvas.GetLeft(child);
                double y = Canvas.GetTop(child);

                double sx = x * scale;
                double sy = y * scale;

                Canvas.SetLeft(child, sx);
                Canvas.SetTop(child, sy);


                child.RenderTransform = transform;
            }

            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            transform.Matrix = new ScaleTransform(1, 1).Value;
            canvas.RenderTransform = transform;

            e.Handled = true;
        }
    }
}
