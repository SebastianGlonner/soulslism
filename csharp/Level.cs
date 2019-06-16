using System;
using Godot;
using SoulslismCSharp;

public class Level : Spatial
{
    private LevelHelper levelHelper;
    private CameraController cameraController;

    public override void _Ready()
    {
        this.levelHelper = new LevelHelper();
        levelHelper.SetUp(this);

        cameraController = new CameraController(
            levelHelper.getPlayerCamera(),
            levelHelper.getPlayerCameraRotationHelperX()
        );

        setUpSmall();
    }

    private void setUpSmall()
    {
        SetProcessInput(true);

        int countPerSide = 1;

        Boolean drawPathVectors = true;

        levelHelper.DeployMany(levelHelper.DeployMinion, countPerSide, new Vector3(-50, 0, 0), drawPathVectors);
        levelHelper.DeployMany(levelHelper.DeployEnemy, countPerSide, new Vector3(30, 0, 0), drawPathVectors);

    }

    private void setUpHuge()
    {
        LevelHelper levelHelper = new LevelHelper();
        levelHelper.SetUp(this);

        SetProcessInput(true);

        int countPerSide = 50;

        Boolean drawPathVectors = true;

        levelHelper.DeployMany(levelHelper.DeployMinion, countPerSide, new Vector3(-50, 0, 0), drawPathVectors);

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

        levelHelper.DeployMany(levelHelper.DeployEnemy, 100, new Vector3(30, 0, 0), drawPathVectors);

        levelHelper.DeployMany(levelHelper.DeployEnemy, 100, new Vector3(-100, 0, 0), drawPathVectors);
        levelHelper.DeployMany(levelHelper.DeployEnemy, 100, new Vector3(90, 0, 0), drawPathVectors);

    }
    public override void _Input(InputEvent @event)
    {
        this.cameraController._input(@event);
    }
}

