using System;
using GXPEngine;

public struct Vec2
{
    public float x;
    public float y;

    public Vec2(float pX = 0, float pY = 0)
    {
        x = pX;
        y = pY;
    }

    public static Vec2 operator +(Vec2 left, Vec2 right)
    {
        return new Vec2(left.x + right.x, left.y + right.y);
    }

    public static Vec2 operator *(Vec2 vec, float scale)
    {
        return new Vec2(vec.x * scale, vec.y * scale);
    }

    public static Vec2 operator -(Vec2 left, Vec2 right)
    {
        return new Vec2(left.x - right.x, left.y - right.y);
    }

    public float Length()
    {
        return Mathf.Sqrt((x * x) + (y * y));
    }

    public Vec2 Normalize()
    {
        float length = Length();
        if (length == 0)
            return new Vec2(0, 0);

        return new Vec2(x / length, y / length);
    }

    public Vec2 SetXY(float hor, float ver)
    {
        return new Vec2(hor, ver);
    }

    public static float DegToRad(float convDeg)
    {
        convDeg = convDeg * Mathf.PI / 180;
        return convDeg;
    }

    public static float RadToDeg(float convRad)
    {
        convRad = convRad * 180 / Mathf.PI;
        return convRad;
    }

    public static Vec2 GetUnitVectorDeg(float deg, float length = 5)
    {
        deg = DegToRad(deg);
        return GetUnitVectorRad(deg, length);
    }

    public static Vec2 GetUnitVectorRad(float rad, float length = 5)
    {
        float getX = Mathf.Cos(rad) * length;
        float getY = Mathf.Sin(rad) * length;
        return new Vec2(getX, getY);
    }

    public Vec2 SetAngleRadiants(float radiants)
    {
        float length = Length();
        this = GetUnitVectorRad(radiants, length);
        return this;
    }

    public Vec2 SetAngleDegrees(float degrees)
    {
        return SetAngleRadiants(DegToRad(degrees));
    }

    public float GetAngleRadiants()
    {
        return (Mathf.Atan2(y, x));
    }

    public float GetAngleDegrees()
    {
        return (RadToDeg(GetAngleRadiants()));
    }

    public void RotateRadians(float angle)
    {
        float sinAngle = Mathf.Sin(angle);
        float cosAngle = Mathf.Cos(angle);
        this = new Vec2(x * cosAngle - y * sinAngle, x * sinAngle + y * cosAngle);
    }

    public void RotateDegrees()
    {
        float angle = GetAngleDegrees();
        RotateRadians(DegToRad(angle));
    }

    public Vec2 RotateAroundRadians(float angle, Vec2 targetOrigin)
    {
        this -= targetOrigin;
        RotateRadians(angle);
        return this += targetOrigin;
    }

    public Vec2 RotateAroundDegrees(Vec2 targetOrigin)
    {
        float angle = GetAngleDegrees();
        return RotateAroundRadians(DegToRad(angle), targetOrigin);
    }

    public override string ToString()
    {
        return String.Format("({0},{1})", x, y);
    }
}