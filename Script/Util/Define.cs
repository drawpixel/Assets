using System.Collections;

public enum UnityLayer
{
	Default = 0,
	TransparentFX = 1,
	IgnoreRayCast = 2,
	Water = 4,
	UI = 5,

	LockRaycast = 11,
	LockRaycastRude = 12,
	Destroyable = 13,
}

public enum DamageType
{
    Fire,
    Energy,
    All,
}

public struct Int2D
{
    public static Int2D zero = new Int2D(0, 0);

    public int X;
    public int Y;

    public Int2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Int2D s1, Int2D s2)
    {
        return s1.X == s2.X && s1.Y == s2.Y;
    }
    public static bool operator !=(Int2D s1, Int2D s2)
    {
        return s1.X != s2.X && s1.Y != s2.Y;
    }
    public static Int2D operator +(Int2D s1, Int2D s2)
    {
        return new Int2D(s1.X + s2.X, s1.Y + s2.Y);
    }
    public static Int2D operator -(Int2D s1, Int2D s2)
    {
        return new Int2D(s1.X - s2.X, s1.Y - s2.Y);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Int2D))
            return false;
        Int2D d2 = (Int2D)obj;
        return this == d2;
    }
    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
    public override string ToString()
    {
        return X.ToString() + " " + Y.ToString();
    }
}