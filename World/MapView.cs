namespace Rayterra.World;

public record struct MapPosition(int X, int Y)
{
    public MapPosition(int scalar) : this(scalar, scalar) { }

    public static MapPosition operator +(MapPosition item1, MapPosition item2)
    {
        return new MapPosition(item1.X + item2.X, item1.Y + item2.Y);
    }

    public static MapPosition operator -(MapPosition item1, MapPosition item2)
    {
        return new MapPosition(item1.X - item2.X, item1.Y - item2.Y);
    }
}

public struct MapView
{
    public MapPosition Min { get; set; }
    public MapPosition Max { get; set; }

    public MapPosition Position => Min;
    public MapPosition Size => new MapPosition(Max.X - Min.X, Max.Y - Min.Y);

    public MapView(MapPosition min, MapPosition max)
    {
        Min = min;
        Max = max;
    }

    public MapView(MapPosition position, int width, int height)
    {
        Min = position;
        Max = position + new MapPosition(width, height);
    }

    public struct Enumerator
    {
        private readonly MapPosition _end;
        private MapPosition _current;

        private MapPosition _min;

        public Enumerator(MapPosition min, MapPosition max)
        {
            _current = new MapPosition(min.X - 1, min.Y);
            _end = max;
            _min = min;
        }

        public MapPosition Current => _current;

        public bool MoveNext()
        {
            _current.X++;
            if (_current.X >= _end.X)
            {
                _current.X = _min.X;
                _current.Y++;

                if (_current.Y >= _end.Y)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public Enumerator GetEnumerator() => new(Min, Max);
}
