using System;
using Godot;

public class LevelHelper
{
    private Camera camera;
    private Spatial cameraRotationHelperX;
    private Navigation navigationNode;

    private PackedScene minionScene;
    private Actor enemyCastle;

    private bool draw_path = true;

    private SpatialMaterial drawPathMaterial;

    private Spatial rootNode;

    public void SetUp(Spatial node)
    {
        this.rootNode = node;

        drawPathMaterial = new SpatialMaterial();
        drawPathMaterial.FlagsUnshaded = true;
        drawPathMaterial.FlagsUsePointSize = true;
        drawPathMaterial.AlbedoColor = new Color(1, 1, 1);

        minionScene = (PackedScene)ResourceLoader.Load("res://scenes/Actor.tscn");

        cameraRotationHelperX = node.GetNode("/root/Level/CameraRotationHelper") as Spatial;
        camera = cameraRotationHelperX.GetNode("Camera") as Camera;

        // setup initial camera position
        cameraRotationHelperX.RotateX(Mathf.Deg2Rad(-65));
        Vector3 camerInitialPosition = new Vector3();
        camerInitialPosition.y = 70;
        camerInitialPosition.z = 40;
        cameraRotationHelperX.GlobalTranslate(camerInitialPosition);

        Transform cameraTransform = camera.GetTransform();
        cameraTransform.origin = Vector3.Zero;
        camera.SetTransform(cameraTransform);

        enemyCastle = node.GetNode("EnemyCastle") as Actor;
        enemyCastle.SetTotalLife(60000);
        enemyCastle.Faction = Soulslism.Faction.Enemy;
        navigationNode = node.GetNode("Navigation") as Navigation;
    }

    public Camera getPlayerCamera()
    {
        return this.camera;
    }

    public Spatial getPlayerCameraRotationHelperX() {
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
        Actor player = minionScene.Instance() as Actor;
        player.SetProcess(false);
        player.SetPhysicsProcess(false);
        this.rootNode.AddChild(player);

        player.GlobalTranslate(at);
        player.SetTotalLife(15);

        ImmediateGeometry draw = null;
        if (drawPath)
        {
            draw = new ImmediateGeometry();
            draw.SetMaterialOverride(drawPathMaterial);
            this.rootNode.AddChild(draw);
        }

        player.Agent = new NavAgent(navigationNode, player, draw);
        player.AddTarget(new AiTarget(enemyCastle, 1));

    }

    public void DeployEnemy(Vector3 pos, bool withTarget = false, bool drawPath = false)
    {
        Actor actor = minionScene.Instance() as Actor;
        actor.SetProcess(false);
        actor.SetPhysicsProcess(false);
        this.rootNode.AddChild(actor);
        actor.Faction = Soulslism.Faction.Enemy;

        actor.GlobalTranslate(pos);
        actor.Agent = new NavAgent(navigationNode, actor);
        actor.SetTotalLife(5);
    }
}

