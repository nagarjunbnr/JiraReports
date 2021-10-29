using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace JiraReports
{
    public class MouseDecorator : Decorator
    {
        static readonly DependencyProperty MousePositionProperty;

        static MouseDecorator()
        {
            MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(MouseDecorator));
        }

        private Path drawPath;

        public Path DrawPath
        {
            get
            {
                if(drawPath == null)
                {
                    ClippingBorder border = base.Child as ClippingBorder;
                    Grid grid = border.Child as Grid;
                    drawPath = grid.Children.OfType<Path>().Single();
                }

                return drawPath;
            }
        }

        public override UIElement Child
        {
            get
            {
                return base.Child;
            }
            set
            {
                if (base.Child != null)
                    base.Child.MouseMove -= ControlledObject_MouseMove;
                base.Child = value;
                base.Child.MouseMove += ControlledObject_MouseMove;
            }
        }

        public Point MousePosition
        {
            get
            {
                MessageBox.Show("Test!");
                return (Point)GetValue(MouseDecorator.MousePositionProperty);
            }
            set
            {
                SetValue(MouseDecorator.MousePositionProperty, value);
            }
        }

        private void ControlledObject_MouseMove(object sender, MouseEventArgs e)
        {
            if(DrawPath.Opacity == 0)
            {
                Point p = e.GetPosition(base.Child);

                // Here you can add some validation logic
                MousePosition = p;
            }
        }
    }
}
