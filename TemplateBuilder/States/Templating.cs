using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TemplateBuilderMVVM.Types;

namespace TemplateBuilderMVVM.States
{
    public class Templating : State
    {
        #region Constants

        private const float CIRCLE_RADIUS = 10; // Drawn cicle points in ?pixels?
        private const int LINE_THICKNESS = 3; // Drawn line width
        private const int LINE_LENGTH = 20;

        #endregion

        private InputState m_InputState;
        private IList<Minutia> m_Minutae;
        private int m_CurrentMinutia;


        #region Constructor

        public Templating(MainWindow outer) : base(outer)
        {
            m_InputState = InputState.Location;
            m_Minutae = new List<Minutia>();
        }

        #endregion

        #region Abstract/Virtual Methods

        public override void OnEnteringState()
        {
            if (m_Minutae.Count > 0)
            {
                // If any minutae have been plotted previously, remove them
                foreach (Minutia minutia in m_Minutae)
                {
                    // TODO: make this more efficient? Probably fine...
                    RemoveMinutia(minutia);
                }
            }
            // TODO: Integrity check list is empty
            Console.WriteLine("Removed minutia, m_Minutae.Count() = {0}", m_Minutae.Count);
            m_Minutae = new List<Minutia>();
        }

        public override void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get position of click
            Point pos = e.GetPosition(m_Outer.canvas);

            switch (m_InputState)
            {
                case InputState.Location:
                    // The user is starting to record a new minutia

                    // Start a new minutia data record.
                    MinutiaRecord record = new MinutiaRecord();
                    record.Location = new Point(pos.X, pos.Y);

                    // Make a circle at the position clicked
                    Ellipse userLocation = LocationShape();
                    // Start a line that points to the mouse
                    Line userDirection = DirectionLine(pos);
                    MinutiaShapes shapes = new MinutiaShapes(userLocation, userDirection);

                    // Record minutia information.
                    Minutia minutia = new Minutia(record, shapes);
                    m_Minutae.Add(minutia);
                    m_CurrentMinutia = m_Minutae.Count - 1;

                    // Plot the minutia position
                    Canvas.SetLeft(userLocation, pos.X - userLocation.Width / 2);
                    Canvas.SetTop(userLocation, pos.Y - userLocation.Height / 2);
                    m_Outer.canvas.Children.Add(minutia.Shapes.Location);
                    m_Outer.canvas.Children.Add(minutia.Shapes.Direction);

                    // Indicate next input defines the direction.
                    TransitionTo(InputState.Direction);
                    break;

                case InputState.Direction:
                    // The user has just finalised the direction of the minutia

                    // Get the vector from location to this position
                    Point currentPos = new Point(pos.X, pos.Y);
                    m_Minutae[m_CurrentMinutia].Record.Direction = currentPos - m_Minutae[m_CurrentMinutia].Record.Location;

                    TransitionTo(InputState.Location);
                    break;

                default:
                    throw new InvalidOperationException(String.Format("m_InputState was unexpected value {0}", m_InputState));
            }

        }

        public override void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_InputState == InputState.Direction)
            {
                // Get mouse position
                Canvas canvas = e.Source as Canvas;
                Point pos = e.GetPosition(canvas);

                // Get the relevant line
                Line direction = m_Minutae[m_CurrentMinutia].Shapes.Direction;

                // Get new end position
                Point endPos = CalcEnd(new Point(direction.X1, direction.Y1), pos, LINE_LENGTH);

                // Update direction of current minutia
                direction.X2 = endPos.X;
                direction.Y2 = endPos.Y;
            }
        }

        public override void saveTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (m_InputState == InputState.Location)
            {
                // TODO: Integrity check that m_Outer.Filename is set

                // We are not partway through inputting a point
                // Construct a file name from the original image file name
                string filename = String.Format(
                    "{0}_template.txt",
                    System.IO.Path.GetFileNameWithoutExtension(m_Outer.Filename));
                string filepath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(m_Outer.Filename),
                    filename);

                // Write the Minutia details out to a new file
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filepath))
                {
                    foreach (Minutia minutia in m_Minutae)
                    {
                        file.WriteLine(ToText(minutia.Record));
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        private void Location_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse location = (Ellipse)e.Source;

            RemoveMinutia(m_Minutae.Single(x => x.Shapes.Location == location));
        }

        #endregion

        #region Helper Methods

        private void RemoveMinutia(Minutia minutia)
        {
            int index = m_Minutae.IndexOf(minutia);

            // Remove the minutia shapes from the canvas
            m_Outer.Canvas.Children.Remove(minutia.Shapes.Location);
            m_Outer.Canvas.Children.Remove(minutia.Shapes.Direction);

            // TODO: create a new class implementing IList that manages this step
            // Ensure m_CurrentIndex will remain correct
            m_CurrentMinutia = (m_CurrentMinutia > index) ? m_CurrentMinutia - 1 : m_CurrentMinutia;
            // Remove the minutia entry
            m_Minutae.Remove(m_Minutae[index]);
        }

        private Point CalcEnd(Point start, Point position, double length)
        {
            Vector diff = position - start; // get displacement
            diff.Normalize(); // Normalise (length = 1)
            Point end = start + diff * length;
            if (double.IsNaN(end.X) && double.IsNaN(end.Y))
            {
                // if start == position then Normalize() results in NaNs
                end = position;
            }
            return end;
        }

        private Ellipse LocationShape()
        {
            // Create a circle with a colour and size
            Ellipse location = new Ellipse()
            {
                Height = 2 * CIRCLE_RADIUS,
                Width = 2 * CIRCLE_RADIUS
            };
            location.Fill = new SolidColorBrush(Colors.YellowGreen);

            // Subscribe right-click to its handler
            location.MouseRightButtonDown += Location_MouseRightButtonDown;
            return location;
        }

        private Line DirectionLine(Point pos)
        {
            Line direction = new Line();
            direction.Stroke = new SolidColorBrush(Colors.GreenYellow);
            direction.StrokeThickness = LINE_THICKNESS;
            direction.X1 = pos.X;
            direction.Y1 = pos.Y;
            direction.X2 = pos.X;
            direction.Y2 = pos.Y;
            return direction;
        }

        private string ToText(MinutiaRecord record)
        {
            return String.Format("{0}, {1}, {2}, {3}",
                record.Location.X, record.Location.Y,
                record.Direction.X, record.Direction.Y);
        }

        private void TransitionTo(InputState state)
        {
            Console.WriteLine("InputState transition: {0}->{1}", m_InputState, state);
            m_InputState = state;
        }

        #endregion
    }

    public enum InputState
    {
        None = 0,
        Location,
        Direction,
    }
}
