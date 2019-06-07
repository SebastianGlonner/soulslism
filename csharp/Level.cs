using System;
using Godot;

public class Level : Spatial
{

    private Camera camera;

    private Navigation navigationNode;

    private PackedScene minionScene;
    private Actor enemyCastle;

    private bool draw_path = true;

    private SpatialMaterial drawPathMaterial;

    public override void _Ready()
    {
        LevelHelper levelHelper = new LevelHelper();
        levelHelper.SetUp(this);

        SetProcessInput(true);

        int countPerSide = 50;

        levelHelper.DeployMany(levelHelper.DeployMinion, countPerSide, new Vector3(-50, 0, 0), false);

        //DeployMinion(new Vector3(-55, 0, 0));
        //DeployMinion(new Vector3(-54, 0, 0));

        //for (int i = 1; i < 10; i++)
        //{
        //    DeployMinion(new Vector3(-55, 0, i));
        //}

        levelHelper.DeployEnemy(new Vector3(-15, 0, -10));
        levelHelper.DeployEnemy(new Vector3(-15, 0, -9));
        levelHelper.DeployEnemy(new Vector3(-15, 0, -8));
        levelHelper.DeployEnemy(new Vector3(-16, 0, 3));
        levelHelper.DeployEnemy(new Vector3(-13, 0, 3));

        levelHelper.DeployEnemy(new Vector3(-5, 0, 8));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 12));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 16));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 22));

        levelHelper.DeployMany(levelHelper.DeployEnemy, countPerSide, new Vector3(30, 0, 0), false);
    }

    public void _on_Button_pressed() {
        NativeScript lib = (NativeScript)ResourceLoader.Load("res://gdnative/navAgent/bin/simple.gdns");
        Godot.Object libInstance = lib.New();

        GD.Print("test");
        GD.Print(libInstance.GetClass());

        Label lbl = this.GetNode("/root/Level/Label") as Label;
        lbl.Text = (String)libInstance.Call("get_data");

    }

    //public override void _Input(InputEvent @event)
    //{
    //    if (@event is InputEventMouseButton)
    //    {
    //        InputEventMouseButton mouseEvent = (InputEventMouseButton)@event;
    //        if (mouseEvent.ButtonIndex == 1 && mouseEvent.Pressed)
    //        {
    //            Vector3 from = camera.ProjectRayOrigin(mouseEvent.Position);
    //            Vector3 to = from + camera.ProjectRayNormal(mouseEvent.Position) * 1000;

    //            PhysicsDirectSpaceState spaceStace = GetWorld().DirectSpaceState;

    //            object collision;
    //            Dictionary colliding = spaceStace.IntersectRay(from, to);
    //            if ( colliding.TryGetValue("position", out collision) )
    //            {
    //                Logging.Log("Update Target on click:");
    //                Logging.Log(colliding);
    //                player.AddTarget(new AiTarget((Vector3) collision, 9));

    //            }

    //        }
    //    }
    //}

}

