namespace EfesusProfiler; 
using Godot;
using System;
using System.Collections.Generic;


public partial class MaxYChange
{
    public int maxY;
    public MaxYChange(int maxY)
    {
        this.maxY = maxY;
    }
}
public partial class ProfilerMetricsViewerGraph : Control
{

    private int maxYVal = 0;
    private int maxY = 0;
    private int marginLeft = 1; // 0 will be clipped

    private List<List<Vector2>> data = new List<List<Vector2>>();

    public event EventHandler<MaxYChange> MaxYChange;

    private Projection _projection;
    public Projection projection
    {
        get { return _projection; }
        set { _projection = value; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        //Size = new Vector2(width, 50);
        //CustomMinimumSize = new Vector2(10, height);
        //SizeFlagsVertical = (int)SizeFlags.Expand;
        //SizeFlagsHorizontal = (int)SizeFlags.Expand;
        //this.AnchorRight = 1;
        //this.AnchorBottom = 1;
        ClipContents = false;
    }

    public override void _Draw()
    {
        _DrawSystem();

        for (int i = 0; i < this.data.Count; i++)
        {
            _DrawMetricData(GraphColors.graph(i), this.data[i]);
        }
    }

    public int AddMetric()
    {
        List< Vector2> metric = new List<Vector2>();
        metric.Add(new Vector2(0, 0));
        metric.Add(new Vector2(0, 0)); // yes, necessary
        this.data.Add(metric);
        return this.data.Count - 1;
    }

    public void AddMetricPoint(int meticIndex, Vector2 p)
    {
        this.setMaxY(p);

        List<Vector2> list = this.data[meticIndex];
        if (p.Y == list[list.Count - 1].Y && p.Y == list[list.Count - 2].Y)
        {
            list.RemoveAt(list.Count - 1);
        }
        this.data[meticIndex].Add(p);
        QueueRedraw();
    }

    private void _DrawSystem()
    {
        Vector2 size = Size;
        int height = (int)size.Y;
        int width = (int)size.X;

        int xZeroLane = height;

        // y lane
        //DrawLine(new Vector2(marginLeft, 0), new Vector2(marginLeft, xZeroLane), GraphColors.system(), 1);

        // x lane
        DrawLine(new Vector2(0, xZeroLane), new Vector2(width, xZeroLane), GraphColors.system(), 1);
    }

    private void _DrawMetricData(Color color, List<Vector2> data)
    {
        Vector2 size = Size;
        int width = (int)size.X;
        for (int i = 1; i < data.Count; i++ )
        {
            Vector2 p = data[i];
            DrawLine(this.projectY(data[i-1]), this.projectY(p), color, 1);

            if (CustomMinimumSize.X < p.X)
            {
                this.CustomMinimumSize = new Vector2(
                    p.X + 100,
                    this.Size.Y
                    );
            }
        }
    }

    private void setMaxY(Vector2 p)
    {
        int newMaxY = (int)Math.Max(this.maxYVal, p.Y);
        this.maxY = (int)(newMaxY * 1.1);
        if (MaxYChange != null && newMaxY != this.maxYVal)
        {
            MaxYChange.Invoke(null, new MaxYChange(maxY));
        }
        this.maxYVal = newMaxY;
    }

    private Vector2 projectY(Vector2 p)
    {
        return _projection.project(p);
    }
}
