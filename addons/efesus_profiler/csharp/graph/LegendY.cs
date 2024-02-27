namespace EfesusProfiler;
using Godot;
using System;
using System.Collections.Generic;

public partial class LegendY : Control
{
    private int maxY;
    private Vector2 staticVector2 = new Vector2(0, 0);
    private int legendYLabelCount = 4;
    private int labelHeight = 14; 
    private List<Label> yLabels = new List<Label>();

    private Projection _projection;
    public Projection projection
    {
        set { _projection = value; }
    }

    private Color _color;
    public Color color
    {
        set { _color = value; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this._PrepareLegendY();
    }

    public override void _Draw()
    {
        this._UpdateLegendY(maxY);
    }

    private void _PrepareLegendY()
    {

        for (int i = 0; i < legendYLabelCount; i++)
        {
            Label label = ControlUtil.NewAttachedLabel("" + i, this);
            label.AnchorRight = 1;
            label.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            label.HorizontalAlignment = HorizontalAlignment.Right;
            yLabels.Add(label);

        }

        yLabels[0].OffsetTop = 0;
    }

    private void _UpdateLegendY(int maxY)
    {
        _UpdateLabel(yLabels[0], maxY);
        yLabels[0].OffsetTop = 0;

        for (int i = 1; i < legendYLabelCount - 1; i++)
        {
            int val = (int)(maxY / legendYLabelCount - 1) * i;
            _UpdateLabel(yLabels[i], val);
        }

        _UpdateLabel(yLabels[legendYLabelCount - 1], 0);

        // y lane
        float x = getWidth();
        DrawLine(new Vector2(x, 0), new Vector2(x, _projection.getMaxHeight()), GraphColors.system(), 1);
    }

    private void _UpdateLabel(Label label, int val)
    {
        label.Text = "" + val;
        label.OffsetTop = CalcLabelYPosition(val);
        DrawLabelLine(val);
    }

    public void maxYChanged(int maxY)
    {
        this.maxY = maxY;
        QueueRedraw();
    }

    private float CalcLabelYPosition(float y)
    {
        staticVector2.Y = y;
        float newY = _projection.project(staticVector2).Y;
        newY -= labelHeight;
        return newY;
    }

    private void DrawLabelLine(float y)
    {
        y = _projection.project(new Vector2(0, y)).Y;
        DrawLine(new Vector2(0, y), new Vector2(getWidth(), y), _color, 1);

    }

    private float getWidth()
    {
        return CustomMinimumSize.X;
    }
}
