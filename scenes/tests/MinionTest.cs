using Godot;
using System;

public partial class MinionTest : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {

        Actor enemyCastle = GetNode("Minion") as Actor;
        enemyCastle.SetTotalLife(10);
    }

//    public override void _Process(float delta)
//    {
//        // Called every frame. Delta is time since last frame.
//        // Update game logic here.
//        
//    }
}
