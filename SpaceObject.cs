using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem
{
    public static class PolarRect
    {
        public static double GetA(double x, double y) => Math.Atan2(y, x); //given x, y get angle
        public static double GetR(double x, double y) => Math.Sqrt(x * x + y * y); //given x, y get radius
        public static double GetX(double r, double a) => r * Math.Cos(a); //given radius, angle get x
        public static double GetY(double r, double a) => r * Math.Sin(a); //given radius, angle get y
    }

    public static class Boundaries
    {
        public static double Left = 0; //variables
        public static double MaxLeft = 0;
        public static double Top = 0;
        public static double MaxTop = 0;
        public static double Right = 0;
        public static double MaxRight = 0;
        public static double Bottom = 0;
        public static double MaxBottom = 0;
        public static double Width = 0;
        public static double Height = 0;
        public static int ScreenWidth = 0;
        public static int ScreenHeight = 0;

        public static void ShiftHorizontal(double amt)
        {
            Left += amt; //move left or right
            Right += amt;
        }
        public static void ShiftVertical(double amt)
        {
            Top += amt; //move up or down
            Bottom += amt;
        }
        public static void ShiftLeft(double amt) => ShiftHorizontal(-amt);
        public static void ShiftRight(double amt) => ShiftHorizontal(amt);
        public static void ShiftUp(double amt) => ShiftVertical(-amt);
        public static void ShiftDown(double amt) => ShiftVertical(amt);
        public static void ShiftLeft() => ShiftLeft(Width / 10);
        public static void ShiftRight() => ShiftRight(Width / 10);
        public static void ShiftUp() => ShiftUp(Height / 10);
        public static void ShiftDown() => ShiftDown(Height / 10);
        public static void ResetAll()
        {
            Left = MaxLeft; //reset all values to the original
            Right = MaxRight;
            Top = MaxTop;
            Bottom = MaxBottom;
            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Bottom - Top);
        }

        public static void SetCenter(double x, double y)
        {
            Left = x - Width / 2; //set the positions from the specified center point
            Right = x + Width / 2;
            Top = y - Height / 2;
            Bottom = y + Height / 2;
        }
        public static void SetCenter(PointD c) => SetCenter(c.X, c.Y);
        public static PointD GetCenter()
        {
            double cx = (Right - Left) / 2 + Left; //compute center x
            double cy = (Bottom - Top) / 2 + Top; //compute center y
            return new PointD(cx, cy); //return center x, center y
        }
        private static PointD GetRadius()
        {
            Width = Math.Abs(Right - Left); //compute width
            Height = Math.Abs(Bottom - Top); //compute height
            double rx = (Right - Left) / 2; //radius x = width / 2
            double ry = (Bottom - Top) / 2; //radius y = height / 2
            return new PointD(rx, ry); //return radius x, radius y
        }
        public static void ZoomIn()
        {
            PointD center = GetCenter(); //get center position
            PointD radius = GetRadius(); //get radius size
            Left = center.X - radius.X / 2; //recompute left
            Right = center.X + radius.X / 2; //recompute right
            Top = center.Y - radius.Y / 2; //recompute top
            Bottom = center.Y + radius.Y / 2; //recompute bottom
            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Bottom - Top);
        }
        public static void ZoomOut()
        {
            PointD center = GetCenter(); //get center position
            PointD radius = GetRadius(); //get radius size
            Left = center.X - radius.X * 2; //recompute left
            Right = center.X + radius.X * 2; //recompute right
            Top = center.Y - radius.Y * 2; //recompute top
            Bottom = center.Y + radius.Y * 2; //recompute bottom
            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Bottom - Top);
        }
        public static Rectangle GetScreenPos(PointD pos, double size)
        {
            if (Width == 0) Width = Math.Abs(Right - Left); //if needed, compute width
            if (Height == 0) Height = Math.Abs(Bottom - Top); //if needed, compute height
            Rectangle rc = new Rectangle(); //make a rectangle to return values
            rc.X = (int)((pos.X - Left) / Width * ScreenWidth); //compute x position
            rc.Y = (int)((pos.Y - Top) /Height * ScreenHeight); //compute y position
            rc.Width = (int)(size / Width * ScreenWidth); //compute width
            rc.Height = (int)(size / Height * ScreenHeight); //compute height
            return rc; //return rectangle
        }
        public static PointD FromScreenPos(Point pos)
        {
            if (Width == 0) Width = Math.Abs(Right - Left); //if needed, compute width
            if (Height == 0) Height = Math.Abs(Bottom - Top); //if needed, compute height
            double x = (double)pos.X / ScreenWidth * Width + Left; //convert x value
            double y = (double)pos.Y / ScreenHeight * Height + Top; //convert y value
            return new(x, y); //return the result
        }
    }

    public class SpaceObject
    {
        const int minwidth = 3;
        const int minheight = 3;
        readonly string name; //name of object
        readonly double size; //size of object (km diameter)
        readonly double mass; //mass of object (kg)
        readonly double radius; //distance from orbitee (km)
        readonly double speed; //orbital speed (km/s)
        readonly double direction; //direction of travel (radians, 0 = orbitting)
        readonly Image picture; //picture of object
        readonly int orbits; //orbits which (-1 = none)
        double angle; //angle of orbit
        PointD position; //position of object
        PointD parent; //position of parent

        public SpaceObject(string name, double mass, double size, double radius, double speed, double direction, Image? picture, int orbits)
        {
            this.name = name; //copy parameters
            this.mass = mass;
            this.size = size;
            this.radius = radius;
            this.speed = speed;
            this.direction = direction;
            this.picture = MakePicture(picture);
            this.orbits = orbits;
            angle = 0; //start with angle of 0
            position = PointD.Empty; //start with position of 0,0
            parent = PointD.Empty; //start with no parent position
            if (Boundaries.MaxLeft == 0) Boundaries.MaxLeft = -1e10; //set boundaries to full view
            if (Boundaries.MaxRight == 0) Boundaries.MaxRight = 1e10;
            if (Boundaries.MaxTop == 0) Boundaries.MaxTop = -1e10;
            if (Boundaries.MaxBottom == 0) Boundaries.MaxBottom = 1e10;
            if (Boundaries.Left == 0) Boundaries.Left = -1e10;
            if (Boundaries.Right == 0) Boundaries.Right = 1e10;
            if (Boundaries.Top == 0) Boundaries.Top = -1e10;
            if (Boundaries.Bottom == 0) Boundaries.Bottom = 1e10;
            if (Boundaries.Width == 0) Boundaries.Width = 2e10;
            if (Boundaries.Height == 0) Boundaries.Height = 2e10;
        }
        private Image MakePicture(Image? image)
        {
            if (image != null) return image; //if image is already good, just return it
            Bitmap bmp = new(100, 100); //set up a picture
            using Graphics g = Graphics.FromImage(bmp); //with graphics object
            {
                using SolidBrush b = new(Color.Gray); //and using a gray brush
                {
                    g.FillEllipse(b, 0, 0, 100, 100); //draw a picture
                }
            }
            return bmp; //return our picture
        }
        public int GetOrbits() => orbits;
        public string GetName() => name;
        public PointD GetPos() => position;
        public void SetParent(PointD pos) => parent = pos;
        public void SetParent(double x, double y) => parent = new(x, y);
        public void SetPosition(PointD pos) => position = pos;
        public void SetPosition(double x, double y) => position = new(x, y);
        public void ComputePosition()
        {
            if (orbits != -1)
            {
                position.X = parent.X + PolarRect.GetX(radius, angle);  //Set horizontal position
                position.Y = parent.Y + PolarRect.GetY(radius, angle);  //Set vertical position
            }
            else
            {
                position.X += PolarRect.GetX(speed, direction); //add movement in horizontal plane
                position.Y += PolarRect.GetY(speed, direction); //add movement in vertical plane
            }
        }
        public void Move()
        {
            angle += 0.001 * speed; //add to angle according to speed
            ComputePosition(); //compute position
        }
        public override string ToString()
        {
            string ret = "";
            ret += name + ": Position = (" + GetPos().X + ", " + GetPos().Y + "); Diameter = " + size + "; ";
            ret += "Orbits = " + orbits + " at a distance of " + radius + "; Translated Position = (";
            ret += TranslateX() + ", " + TranslateY() + ") with size (" + TranslateWidth() + ", " + TranslateHeight() + ").";
            return ret;
        }
        public Rectangle TranslatePosition() => Boundaries.GetScreenPos(position, size);
        public int TranslateX() => TranslatePosition().X;
        public int TranslateY() => TranslatePosition().Y;
        public int TranslateWidth() => TranslatePosition().Width;
        public int TranslateHeight() => TranslatePosition().Height;
        public void Paint(Graphics g)
        {
            Rectangle dest = TranslatePosition(); //get rectangle for destination
            if (dest.Width < minwidth) dest.Width = minwidth; //apply minimum width
            if (dest.Height < minheight) dest.Height = minheight; //apply minimum height
            if ((dest.X < 0) || (dest.X > Boundaries.ScreenWidth) || (dest.Y < 0) || (dest.Y > Boundaries.ScreenHeight)) //if outside bounds
                return; //just quit
            Rectangle dst = new(dest.X - dest.Width / 2, dest.Y - dest.Height / 2, dest.Width, dest.Height); //make new destination rect
            Rectangle src = new(0, 0, picture.Width, picture.Height); //source rect
            g.DrawImage(picture, dst, src, GraphicsUnit.Pixel); //draw image
        }
    }



    /// <summary>
    /// PointD structure - like a PointF but using doubles instead of floats
    /// I did not really need to make this and had no particular reason to do so except that I wanted to do it as I had not made
    /// something like this before.
    /// </summary>
    [Serializable]
    public struct PointD : IEquatable<PointD>
    {
        /// <summary>
        /// Creates a new instance of the <see cref='PointD'/> class with member data left uninitialized.
        /// </summary>
        public static readonly PointD Empty;
        private double x; // Do not rename (binary serialization)
        private double y; // Do not rename (binary serialization)

        /// <summary>
        /// Initializes a new instance of the <see cref='PointD'/> class with the specified coordinates.
        /// </summary>
        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='PointD'/> struct from the specified
        /// <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        public PointD(Vector2 vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='PointD'/> struct from the specified
        /// <see cref="System.Drawing.PointF"/>
        /// </summary>
        public PointD(PointF point)
        {
            x = point.X;
            y = point.Y;
        }

        /// <summary>
        /// Creates a new <see cref="System.Drawing.PointF"/> from this <see cref="PointD"/>
        /// </summary>
        /// <returns></returns>
        public readonly PointF ToPointF() => new((float)x, (float)y);

        /// <summary>
        /// Creates a new <see cref="System.Numerics.Vector2"/> from this <see cref="PointD"/>.
        /// </summary>
        public readonly Vector2 ToVector2() => new((float)x, (float)y);

        /// <summary>
        /// Gets a value indicating whether this <see cref='PointD'/> is empty.
        /// </summary>
        [Browsable(false)]
        public readonly bool IsEmpty => x == 0.0 && y == 0.0;

        /// <summary>
        /// Gets the x-coordinate of this <see cref='PointD'/>.
        /// </summary>
        public double X
        {
            readonly get => x;
            set => x = value;
        }

        /// <summary>
        /// Gets the y-coordinate of this <see cref='PointD'/>.
        /// </summary>
        public double Y
        {
            readonly get => y;
            set => y = value;
        }

        /// <summary>
        /// Converts the specified <see cref="PointD"/> to a <see cref="System.Numerics.Vector2"/>.
        /// </summary>
        public static explicit operator Vector2(PointD point) => point.ToVector2();

        /// <summary>
        /// Converts the specified <see cref="System.Numerics.Vector2"/> to a <see cref="PointD"/>.
        /// </summary>
        public static explicit operator PointD(Vector2 vector) => new(vector);

        /// <summary>
        /// Converts the specified <see cref="PointD"/> to a <see cref="System.Drawing.PointF"/>
        /// </summary>
        public static explicit operator PointF(PointD point) => point.ToPointF();

        /// <summary>
        /// Converts the specified <see cref="System.Drawing.PointF"/> to a <see cref="PointD"/>
        /// </summary>
        public static explicit operator PointD(PointF point) => new(point);

        /// <summary>
        /// Translates a <see cref='PointD'/> by a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD operator +(PointD pt, Size sz) => Add(pt, sz);

        /// <summary>
        /// Translates a <see cref='PointD'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD operator -(PointD pt, Size sz) => Subtract(pt, sz);

        /// <summary>
        /// Translates a <see cref='PointD'/> by a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD operator +(PointD pt, SizeF sz) => Add(pt, sz);

        /// <summary>
        /// Translates a <see cref='PointD'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD operator -(PointD pt, SizeF sz) => Subtract(pt, sz);

        /// <summary>
        /// Compares two <see cref='PointD'/> objects. The result specifies whether the values of the
        /// <see cref='PointD.X'/> and <see cref='PointD.Y'/> properties of the two
        /// <see cref='PointD'/> objects are equal.
        /// </summary>
        public static bool operator ==(PointD left, PointD right) => left.X == right.X && left.Y == right.Y;

        /// <summary>
        /// Compares two <see cref='PointD'/> objects. The result specifies whether the values of the
        /// <see cref='PointD.X'/> or <see cref='PointD.Y'/> properties of the two
        /// <see cref='PointD'/> objects are unequal.
        /// </summary>
        public static bool operator !=(PointD left, PointD right) => !(left == right);

        /// <summary>
        /// Translates a <see cref='PointD'/> by a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD Add(PointD pt, Size sz) => new(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// Translates a <see cref='PointD'/> by the negative of a given <see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD Subtract(PointD pt, Size sz) => new(pt.X - sz.Width, pt.Y - sz.Height);

        /// <summary>
        /// Translates a <see cref='PointD'/> by a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD Add(PointD pt, SizeF sz) => new(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// Translates a <see cref='PointD'/> by the negative of a given <see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD Subtract(PointD pt, SizeF sz) => new(pt.X - sz.Width, pt.Y - sz.Height);

        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is PointD pt && Equals(pt);

        public readonly bool Equals(PointD other) => this == other;

        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());

        public override readonly string ToString() => $"{{X={x}, Y={y}}}";
    }
}
