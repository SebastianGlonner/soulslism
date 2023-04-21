using System;
using Godot;

public partial class LevelHelper
{
    private Camera3D camera;
    private Node3D cameraRotationHelperX;
    // g35 private Navigation navigationNode;

    private PackedScene minionScene;
    private Actor enemyCastle;

    private bool draw_path = true;

    private StandardMaterial3D drawPathMaterial;

    private Node3D rootNode;

    public void SetUp(Node3D node)
    {
        this.rootNode = node;

        //drawPathMaterial = new StandardMaterial3D();
        //drawPathMaterial.FlagsUnshaded = true;
        //drawPathMaterial.FlagsUsePointSize = true;
        //drawPathMaterial.AlbedoColor = new Color(1, 1, 1);

        minionScene = (PackedScene)ResourceLoader.Load("res://scenes/Actor.tscn");

        cameraRotationHelperX = node.GetNode("/root/Level/CameraRotationHelper") as Node3D;
        camera = cameraRotationHelperX.GetNode("Camera3D") as Camera3D;

        // setup initial camera position
        cameraRotationHelperX.RotateX(Mathf.DegToRad(-65));
        Vector3 camerInitialPosition = new Vector3();
        camerInitialPosition.Y = 70;
        camerInitialPosition.Z = 40;
        cameraRotationHelperX.GlobalTranslate(camerInitialPosition);

        Transform3D cameraTransform = camera.Transform;
        cameraTransform.Origin = Vector3.Zero;
        camera.Transform = cameraTransform;

        enemyCastle = node.GetNode("EnemyCastle") as Actor;
        enemyCastle.SetTotalLife(60000);
        enemyCastle.Faction = Soulslism.Faction.Enemy;
        // g35 navigationNode = node.GetNode("Navigation") as Navigation;
    }

    public Camera3D getPlayerCamera()
    {
        return this.camera;
    }

    public Node3D getPlayerCameraRotationHelperX() {
        return this.cameraRotationHelperX;
    }

    public void DeployMany(Action<Vector3, bool, bool> which, int count, Vector3 at, bool drawPath = false)
    {
        float gap = 2.5f;
        int perLine = 10;
        int row = 0;
        int line = 0;

        for ( int i = 0; i < count; i++ )
        {
            int dir = i % 2 == 1 ? -1 : 1;

            Vector3 pos = at + new Vector3(
                row * gap,
                0,
                dir * line * gap
            );

            which(pos, true, drawPath);

            line++;
            if ( line > perLine )
            {
                row++;
                line = 0;
            }
        }
    }

    public void DeployMinion(Vector3 at, bool withTarget = false, bool drawPath = false)
    {
        Actor player = minionScene.Instantiate() as Actor;
        player.SetProcess(false);
        player.SetPhysicsProcess(false);
        this.rootNode.AddChild(player);

        player.GlobalTranslate(at);
        player.SetTotalLife(15);

        ImmediateMesh draw = null;
        //if (drawPath)
        //{
        //    draw = new ImmediateMesh();
        //    draw.SurfaceSetMaterial(0, drawPathMaterial);
        //    this.rootNode.AddChild(draw);
        //}

        player.Agent = new NavAgent(rootNode.GetWorld3D(), player, draw);
        player.AddTarget(new AiTarget(enemyCastle, 1));

    }

    public void DeployEnemy(Vector3 pos, bool withTarget = false, bool drawPath = false)
    {
        Actor actor = minionScene.Instantiate() as Actor;
        actor.SetProcess(false);
        actor.SetPhysicsProcess(false);
        this.rootNode.AddChild(actor);
        actor.Faction = Soulslism.Faction.Enemy;

        actor.GlobalTranslate(pos);
        actor.Agent = new NavAgent(rootNode.GetWorld3D(), actor);
        actor.SetTotalLife(5);
    }
}

