using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Practika
{
    public class Ploter
    {
        List<Line> lines = new List<Line>();
        public List<Line> Lines { get { return lines; } }
        public Point Center { get; set; }
        public float XPixelsPerUnit { get; set; }
        public float YPixelsPerUnit { get; set; }
        public Color GridColor { get; set; }
        public Color oXColor { get; set; }
        public Color oYColor { get; set; }
        public float GridWidth { get; set; }
        public float OWidth { get; set; }
        public DashStyle GridStyle { get; set; }
        public int XAxisSubDashesPerUnit { get; set; }
        public int YGridUnitsPerSquare { get; set; }
        public int XGridUnitsPerSquare { get; set; }
        public Ploter(float xPixelsPerUnit, float yPixelsPerUnit, Point center)
        {
            this.XPixelsPerUnit = xPixelsPerUnit;
            this.YPixelsPerUnit = yPixelsPerUnit;
            this.Center = center;
            this.GridWidth = 0.5f;
            this.OWidth = 1;
            GridColor = Color.FromArgb(200, 200, 200);
            oXColor = oYColor = Color.Black;
            this.YGridUnitsPerSquare = 1;
            this.XGridUnitsPerSquare = 1;
            this.XAxisSubDashesPerUnit = 1;
    }
        public void DrawGridandAxes(Graphics g, int width, int height)
        {
            using (Pen gridPen = new Pen(GridColor))
            {
                gridPen.Width = GridWidth;
                gridPen.DashStyle = GridStyle;
                int seed = (int)Math.Round(YGridUnitsPerSquare * YPixelsPerUnit);
                int i = Center.Y - seed;
                for (; i > 0; i -= seed) g.DrawLine(gridPen, 0, i, width, i);
                for (i = Center.Y + seed; i < height; i += seed) g.DrawLine(gridPen, 0, i, width, i);
                seed = (int)Math.Round(XGridUnitsPerSquare * XPixelsPerUnit);
                i = Center.X - seed;
                for (; i > 0; i -= seed) g.DrawLine(gridPen, i, 0, i, height);
                for (i = Center.X + seed; i < width; i += seed) g.DrawLine(gridPen, i, 0, i, height);
            }
            using (Pen p = new Pen(oXColor))
            {
                p.Width = OWidth;
                g.DrawLine(p, Center.X, Center.Y, width, Center.Y);
                g.DrawLine(p, Center.X, Center.Y, 0, Center.Y);
            }
            using (Pen p = new Pen(oYColor))
            {
                p.Width = OWidth;
                g.DrawLine(p, Center.X, Center.Y, Center.X, 0);
                g.DrawLine(p, Center.X, Center.Y, Center.X, height);
            }
            using (Pen xDashPen = new Pen(oXColor))
            {
                xDashPen.Width = OWidth;
                int seed = (int)Math.Ceiling((XPixelsPerUnit));
                int i = Center.X - seed;
                int dashStart = (Center.Y - (int)(OWidth / 2) - (int)((4 - OWidth) / 2));
                int dashEnd = dashStart + 3;
                Font f;
                if (seed > 5) f = new Font("Arial", seed / 5);
                else f = new Font("Arial", seed);
                int step = 0;
                for (; i > 0; i -= seed)
                {
                    step--;
                    var size = g.MeasureString(step.ToString(), f);
                    var rect = new RectangleF(new PointF(i - size.Height / 2, dashEnd + size.Height/2), size);
                    g.DrawLine(xDashPen, i, dashStart, i, dashEnd);
                    g.DrawString(step.ToString(), f, Brushes.Black, rect);
                }
                step = 0;
                for (i = Center.X + seed; i < width; i += seed)
                {
                    step++;
                    var size = g.MeasureString(step.ToString(), f);
                    var rect = new RectangleF(new PointF(i - size.Height / 2, dashEnd + size.Height / 2), size);
                    g.DrawLine(xDashPen, i, dashStart, i, dashEnd);
                    g.DrawString(step.ToString(), f, Brushes.Black, rect);
                }
            }
            using (Pen yDashPen = new Pen(oYColor))
            {
                yDashPen.Width = OWidth;
                int seed = (int)Math.Round(YPixelsPerUnit);
                int i = Center.Y - seed;
                int dashStart = (int)(Center.X - (int)(OWidth / 2) - ((4 - OWidth) / 2));
                int dashEnd = dashStart + 3;
                Font f;
                if (seed > 5) f = new Font("Arial", seed/5);
                else f = new Font("Arial", seed);
                int step = 0;
                for (; i > 0; i -= seed)
                {
                    step++;
                    var size = g.MeasureString(step.ToString(), f);
                    var rect = new RectangleF(new PointF(dashStart + size.Height / 2, i - size.Height / 2), size);
                    g.DrawLine(yDashPen, dashStart, i, dashEnd, i);
                    g.DrawString(step.ToString(), f, Brushes.Black, rect);
                }
                step = 0;
                for (i = Center.Y + seed; i < height; i += seed)
                {
                    step--;
                    var size = g.MeasureString(step.ToString(), f);
                    var rect = new RectangleF(new PointF(dashStart + size.Height / 2, i - size.Height / 2), size);
                    g.DrawLine(yDashPen, dashStart, i, dashEnd, i);
                    g.DrawString(step.ToString(), f, Brushes.Black, rect);
                }
            }
        }
        public void DrawLines(Graphics g)
        {
            g.TranslateTransform(Center.X, Center.Y);
            g.ScaleTransform(1, -1);
            foreach (Line line in lines)
            {
                if (line.Show)
                {
                    using (Pen p = new Pen(line.LineColor))
                    {
                        p.Width = line.LineWidth;
                        SmoothingMode mode = g.SmoothingMode;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        for (int i = 0; i < line.Points.Count - 1; i++)
                        {
                            try
                            {
                                g.DrawLine(p, line.Points[i].X * XPixelsPerUnit, line.Points[i].Y * YPixelsPerUnit, line.Points[i + 1].X * XPixelsPerUnit, line.Points[i + 1].Y * YPixelsPerUnit);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        g.SmoothingMode = mode;
                    }
                }
            }
            g.ResetTransform();
        }
    }

    public class Line {
        public Color LineColor { get; set; }
        public List<PointF> Points { get; set; }
        public float LineWidth { get; set; }
        public bool Show { get; set; }
        public float PointSize { get; set; }
        public Line(Color lineColor)
        {
            LineColor = lineColor;
            LineWidth = 1;
            PointSize = 1.5f;
            Points = new List<PointF>();
            Show = true;
        }
    }
}
