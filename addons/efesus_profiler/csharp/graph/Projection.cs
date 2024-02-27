namespace EfesusProfiler;

using Godot;
using System;


public partial class Projection
{
    private float maxHeight;
    private float maxValue;

    public Projection(float maxHeight, float maxValue)
    {
        this.maxHeight = maxHeight;
        this.maxValue = maxValue;
    }

    public float getMaxHeight()
    {
        return maxHeight;
    }

    public Vector2 project(Vector2 p)
    {
        float yPercent = (100 * p.Y) / maxValue;
        float yProjected = (yPercent * maxHeight) / 100;

        return new Vector2(p.X, Math.Max(maxHeight - yProjected, 0));
    }
}

