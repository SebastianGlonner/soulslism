using Godot;

public class LifeLine : Spatial {

    private int percentLife = 100;
    private Vector3 localScale;
    private float maxScaleY;
    private float actualScaleY;
    private Spatial lineToScale;

    public override void _Ready()
    {
        lineToScale = GetNode("Life") as Spatial;
        maxScaleY = lineToScale.GetScale().y;
    }

    public void SetLife(int percentLife)
    {
        actualScaleY = (percentLife * maxScaleY) / 100;
        localScale = lineToScale.GetScale();
        lineToScale.SetScale(new Vector3(localScale.x, actualScaleY, localScale.z));
    }
}
