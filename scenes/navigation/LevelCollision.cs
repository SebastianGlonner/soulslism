using System;
using Godot;

public class LevelCollision : Spatial
{

    private Camera camera;

    private Navigation navigationNode;

    private PackedScene minionScene;
    private Actor enemyCastle;

    private bool draw_path = true;

    private SpatialMaterial drawPathMaterial;

    public override void _Ready()
    {
        drawPathMaterial = new SpatialMaterial();
        drawPathMaterial.FlagsUnshaded = true;
        drawPathMaterial.FlagsUsePointSize = true;
        drawPathMaterial.AlbedoColor = new Color(1, 1, 1);

        minionScene = (PackedScene)ResourceLoader.Load("res://scenes/Actor.tscn");
        SetProcessInput(true);

        camera = GetNode("/root/Level/Camera") as Camera;
        enemyCastle = GetNode("EnemyCastle") as Actor;
        enemyCastle.SetTotalLife(60000);
        navigationNode = GetNode("Navigation") as Navigation;

        int countPerSide = 50;

        DeployMany(DeployMinion, countPerSide, new Vector3(-80, 0, 0), false);

        //DeployMinion(new Vector3(-55, 0, 0));
        //DeployMinion(new Vector3(-54, 0, 0));

        //for (int i = 1; i < 10; i++)
        //{
        //    DeployMinion(new Vector3(-55, 0, i));
        //}

        DeployMinion(new Vector3(-9, 0, -5), false);
        DeployMinion(new Vector3(-9, 0, -7), false);

        DeployMany(DeployEnemy, countPerSide, new Vector3(50, 0, 0), false);
    }

    private void DeployMany(Action<Vector3, bool> which, int count, Vector3 at, bool drawPath = false)
    {
        float gap = .5f;
        int perLine = 10;
        int row = 0;
        int line = 0;

        for ( int i = 0; i < count; i++ )
        {
            int dir = i % 2 == 1 ? -1 : 1;

            Vector3 pos = at + new Vector3(
                row * gap,
                0,
                dir * line
            );

            which(pos, true);

            line++;
            if ( line > perLine )
            {
                row++;
                line = 0;
            }
        }
    }

    private void DeployMinion(Vector3 at, bool withTarget)
    {
        Actor player = minionScene.Instance() as Actor;
        AddChild(player);

        player.GlobalTranslate(at);
        player.SetTotalLife(15);

        ImmediateGeometry draw = null;
        if ( false )
        {
            draw = new ImmediateGeometry();
            draw.SetMaterialOverride(drawPathMaterial);
            AddChild(draw);
        }

        player.Agent = new NavAgent(navigationNode, player, draw);

        if ( withTarget )
            player.AddTarget(new AiTarget(enemyCastle, 1));

    }

    private void DeployEnemy(Vector3 pos, bool withTarget)
    {
        Actor actor = minionScene.Instance() as Actor;
        AddChild(actor);
        actor.Faction = Soulslism.Faction.Enemy;

        actor.GlobalTranslate(pos);
        actor.Agent = new NavAgent(navigationNode, actor);
        actor.SetTotalLife(5);
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

