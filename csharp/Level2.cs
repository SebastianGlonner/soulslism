using System;
using Godot;
using Soulslism;

public partial class Level2 : Node3D
{
    private LevelHelper levelHelper;
    private GameController cameraController;

    public override void _Ready()
    {
        this.levelHelper = new LevelHelper();
        levelHelper.SetUp(this);

        cameraController = new GameController(
            levelHelper.getPlayerCamera(),
            levelHelper.getPlayerCameraRotationHelperX()
        );

        setUpHuge();
    }

    private void setUpHuge()
    {
        LevelHelper levelHelper = this.levelHelper;

        SetProcessInput(true);

        int countPerSide = 50;

        Boolean drawPathVectors = true;

        levelHelper.DeployMany(levelHelper.DeployMinion, countPerSide, new Vector3(-90, 0, 0), drawPathVectors);
        levelHelper.DeployMany(levelHelper.DeployMinion, countPerSide, new Vector3(-50, 0, 0), drawPathVectors);

        setupEnemies(drawPathVectors);
    }

    private void setupEnemies(Boolean drawPathVectors)
    {

        levelHelper.DeployEnemy(new Vector3(-15, 0, -10));
        levelHelper.DeployEnemy(new Vector3(-15, 0, -9));
        levelHelper.DeployEnemy(new Vector3(-15, 0, -8));
        levelHelper.DeployEnemy(new Vector3(-16, 0, 3));
        levelHelper.DeployEnemy(new Vector3(-13, 0, 3));

        levelHelper.DeployEnemy(new Vector3(-5, 0, 8));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 12));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 16));
        levelHelper.DeployEnemy(new Vector3(-5, 0, 22));

        levelHelper.DeployMany(levelHelper.DeployEnemy, 100, new Vector3(30, 0, 0), drawPathVectors);

        levelHelper.DeployMany(levelHelper.DeployEnemy, 100, new Vector3(90, 0, 0), drawPathVectors);

    }

    public override void _Input(InputEvent @event)
    {
        // this.cameraController._input(@event);
    }

    private void _on_EnemyCastle_input_event(Node camera, InputEvent @event, Vector3 click_position, Vector3 click_normal, int shape_idx) {
        if ( @event is InputEventMouseButton )
            GD.Print("clicked enemy castel");  
    }
}

