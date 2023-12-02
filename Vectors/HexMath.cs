using Elements.Geometry;


    public static class HexMath
    {
        public static readonly double ScaleY = System.Math.Sqrt(3.0) * 0.5;
        public static readonly double HalfHex = 0.5f / System.Math.Cos(System.Math.PI / 180.0 * 30.0);
        public const double TOLERANCE = 0.0000001;

        public static bool EqualsWithinTolerance(double a, double b) => System.Math.Abs(a - b) < TOLERANCE;

        public static Vector3 GetHexPosition2d(Int3 index3d) => GetHexPosition2d(Get2dHexIndex(index3d));

        public static Vector3 GetHexPosition2d(Int2 index2d)
        {
            var isOdd = index2d.YIndex % 2 != 0;

            return new Vector3(
                index2d.XIndex - (isOdd ? 0 : 0.5f),
                index2d.YIndex * ScaleY);
        }

        public static Int3 Get3dHexIndex(Int2 index2d)
        {
            var x = index2d.XIndex - (index2d.YIndex - (index2d.YIndex & 1)) / 2;
            var z = index2d.YIndex;
            var y = -x - z;
            return new Int3(x, y, z);
        }

        public static Int2 Get2dHexIndex(Int3 index3d)
        {
            var col = index3d.XIndex + (index3d.ZIndex - (index3d.ZIndex & 1)) / 2;
            var row = index3d.ZIndex;
            return new Int2(col, row);
        }

        /// <summary>
        /// Rotate a given hex around 0,0,0 by 60 degrees
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Int3 RotateHex60(Int3 index)
        {
            return new Int3(-index.YIndex, -index.ZIndex, -index.XIndex);
        }

        //public static Int3 NestMultiply(Int3 index, int amount)
        //{
        //   return index.Index3d * (amount + 1) + (index.RotateHex60() * amount);
        //}

        public static Int3[] GenerateRosetteLinear(Int3 index, int radius)
        {
            //calculate rosette size without any GC :(

            var count = 0;

            for (int q = -radius; q <= radius; q++)
            {
                int r1 = System.Math.Max(-radius, -q - radius);
                int r2 = System.Math.Min(radius, -q + radius);

                for (int r = r1; r <= r2; r++)
                {
                    count++;
                }
            }

            //Do the whole thing again this time making an array            

            var output = new Int3[count];

            count = 0;

            for (int q = -radius; q <= radius; q++)
            {
                int r1 = System.Math.Max(-radius, -q - radius);
                int r2 = System.Math.Min(radius, -q + radius);
                for (int r = r1; r <= r2; r++)
                {
                    var vec = new Int3(q, r, -q - r) + index;
                    output[count] = vec;
                    count++;
                }

            }

            return output;
        }

        private static readonly Int3[] _directions = new Int3[]
        {
            new Int3(1,-1,0),
            new Int3(0,-1,1),
            new Int3(-1,0,1),
            new Int3(-1,1,0),
            new Int3(0,1,-1),
            new Int3(1,0,-1)
        };

        public static Int3[] GenerateRosetteCircular(Int3 index, int radius)
        {
            var results = new List<Int3>() { index };

            for (int i = 1; i < radius; i++)
            {
                results.AddRange(GenerateRing(index,i));
            }

            return results.ToArray();
        }

        public static Int3[] GenerateRing(Int3 index, int radius)
        {
            var resultsCount = radius == 0 ? 1 : (radius) * 6;

            var results = new Int3[resultsCount];

            var current = index + (_directions[4] * radius);
            //var last = current;
            current += (_directions[0] * ((int)System.Math.Floor(radius * 0.5)));

            var ringStart = current;

            var i = 0;
            var count = 0;

            while (true)
            {
                for (int j = (count == 0 ? (int)System.Math.Floor(radius * 0.5f) : 0); j < radius; j++)
                {

                    //Debug.Log($"Added Cell {current}");
                    results[count] = (current);
                    current += _directions[i];
                    //Debug.DrawLine(last.ition3d, current.ition3d, Color.green * 0.5f, 100f);
                    //last = current;
                    count++;

                    if (ringStart.Equals(current))
                        return results;
                }

                i = i < 5 ? (i + 1) : 0;
            }
        }

        public static double Blerp(double a, double b, double c, Vector3 weight)
        {
            return a * weight.X + b * weight.Y + c * weight.Z;
        }

        public static Vector3 Blerp2d(Vector3 a, Vector3 b, Vector3 c, Vector3 weight)
        {
            var x = a.X * weight.X + b.X * weight.Y + c.X * weight.Z;
            var y = a.Y * weight.X + b.Y * weight.Y + c.Y * weight.Z;

            return new Vector3(x, y);
        }

        public static Vector3 Blerp3d(Vector3 a, Vector3 b, Vector3 c, Vector3 weight)
        {
            var x = a.X * weight.X + b.X * weight.Y + c.X * weight.Z;
            var y = a.Y * weight.X + b.Y * weight.Y + c.Y * weight.Z;
            var z = a.Z * weight.X + b.Z * weight.Y + c.Z * weight.Z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Choose either A, B or C based on the weight.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static T BlerpChoice<T>(T a, T b, T c, Vector3 weight)
        {
            if (weight.X >= weight.Y && weight.X >= weight.Z)
            {
                return a;
            }
            else if (weight.Y >= weight.Z && weight.Y >= weight.X)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        public static int DistanceTo(Int3 a, Int3 b)
        {
            return (System.Math.Abs(a.XIndex - b.XIndex) + System.Math.Abs(a.YIndex - b.YIndex) + System.Math.Abs(a.ZIndex - b.ZIndex)) / 2;
        }

        public static double Dot2d(Vector3 a, Vector3 b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }


    }

        public readonly struct Int2 : IEquatable<Int2>
    {
        public int XIndex { get; }
        public int YIndex { get; }

        public Int2(int x, int y)
        {
            XIndex = x;
            YIndex = y;
        }

        public override string ToString() => $"{XIndex},{YIndex}";

        public Int2 Subtract(Int2 other) => new(XIndex - other.XIndex, YIndex - other.YIndex);
        public Int2 Add(Int2 other) => new(XIndex + other.XIndex, YIndex + other.YIndex);
        public Int2 Multiply(Int2 other) => new(XIndex * other.XIndex, YIndex * other.YIndex);
        public Int2 Multiply(int other) => new(XIndex * other, YIndex * other);

        public bool Equals(Int2 other) => XIndex == other.XIndex && this.YIndex == other.YIndex;
        public override bool Equals(object obj) => obj is Int2 @int && Equals(@int);

        public static Int2 Subtract(Int2 a, Int2 b) => a.Subtract(b);
        public static Int2 Add(Int2 a, Int2 b) => a.Add(b);
        public static Int2 Multiply(Int2 a, Int2 b) => a.Multiply(b);
        public static Int2 Multiply(Int2 a, int b) => a.Multiply(b);

        public override int GetHashCode() => HashCode.Combine(XIndex, YIndex);

        public static Int2 operator +(Int2 a, Int2 b) => Add(a, b);
        public static Int2 operator -(Int2 a, Int2 b) => Subtract(a, b);
        public static Int2 operator *(Int2 a, Int2 b) => Multiply(a, b);
        public static Int2 operator *(Int2 a, int b) => Multiply(a, b);

        public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);
        public static bool operator !=(Int2 left, Int2 right) => !(left == right);
    }

        public readonly struct Int3 : IEquatable<Int3>
    {
        public int XIndex { get; }
        public int YIndex { get; }
        public int ZIndex { get; }

        public Int3(int x, int y, int z)
        {
            XIndex = x;
            YIndex = y;
            ZIndex = z;
        }

        public override string ToString() => $"{XIndex},{YIndex},{ZIndex}";

        public Int3 Subtract(Int3 other) => new(XIndex - other.XIndex, YIndex - other.YIndex, ZIndex - other.ZIndex);
        public Int3 Add(Int3 other) => new(XIndex + other.XIndex, YIndex + other.YIndex, ZIndex + other.ZIndex);
        public Int3 Multiply(Int3 other) => new(XIndex * other.XIndex, YIndex * other.YIndex, ZIndex * other.ZIndex);
        public Int3 Multiply(int other) => new(XIndex * other, YIndex * other, ZIndex * other);

        public override bool Equals(object obj) => obj is Int3 @int && Equals(@int);

        public bool Equals(Int3 other)
        {
            return XIndex == other.XIndex &&
                   YIndex == other.YIndex &&
                   ZIndex == other.ZIndex;
        }

        public override int GetHashCode() => HashCode.Combine(XIndex, YIndex, ZIndex);

        public static Int3 operator +(Int3 a, Int3 b) => a.Add(b);
        public static Int3 operator -(Int3 a, Int3 b) => a.Subtract(b);
        public static Int3 operator *(Int3 a, Int3 b) => a.Multiply(b);
        public static Int3 operator *(Int3 a, int b) => a.Multiply(b);

        public static bool operator ==(Int3 left, Int3 right) => left.Equals(right);

        public static bool operator !=(Int3 left, Int3 right) => !(left == right);
    }
