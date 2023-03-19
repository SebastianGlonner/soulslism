using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Soulslism
{

    public partial class GameController
    {
        private float mouseSpeed = 0.25f;

        private float dragSpeed = 0.25f;
        private int dragDirection = -1;

        private float zoomSpeed = 6;

        private Camera3D camera;

        private Node3D rotationHelperX;

        private Vector3 moveCamera = new Vector3();

        private Boolean dragging;
        private Boolean rotating;

        private InputStateMachine machine;

        public GameController(Camera3D camera, Node3D rotationHelperX)
        {
            this.camera = camera;
            this.rotationHelperX = rotationHelperX;

            machine = new InputStateMachine(camera, rotationHelperX);
        }

        public void _input(InputEvent @event)
        {
            InputAction action = InputAction.IDLE;
            if (@event is InputEventMouseButton) {

                int buttonIndex = ((InputEventMouseButton) @event).ButtonIndex;
                if ( buttonIndex == (int)ButtonList.Left)
                {
                    action = InputAction.POINT_LEFT;
                } 
                else if (buttonIndex == (int)ButtonList.WheelDown)
                {
                    action = InputAction.ZOOM_OUT;
                } 
                else if (buttonIndex == (int)ButtonList.WheelUp)
                {
                    action = InputAction.ZOOM_IN;
                } 
                else if (buttonIndex == (int)ButtonList.Right) 
                {
                    action = InputAction.POINT_RIGHT;
                }

            } else if (@event is InputEventMouseMotion ) 
            {
                action = InputAction.POINT_MOVE;
            }
            
            if ( action != InputAction.IDLE )
                machine.processInput(action, @event);

        }
        public void _input2(InputEvent @event)
        {
            Boolean doMove = false;
            moveCamera.x = 0;
            moveCamera.y = 0;
            moveCamera.z = 0;
            
            if (@event is InputEventMouseButton)
            {
                InputEventMouseButton mouseEvent = (InputEventMouseButton)@event;
                if ( mouseEvent.ButtonIndex == (int)ButtonList.Left)
                {
                    if ( !mouseEvent.Pressed ) {
                        Vector3 rayFrom = camera.ProjectRayOrigin(mouseEvent.Position);
                        Vector3 rayTo = rayFrom + camera.ProjectRayNormal(mouseEvent.Position) * 1000;

                        Godot.Collections.Dictionary selection = camera.GetWorld3d().DirectSpaceState.IntersectRay(rayFrom, rayTo);
                        object collided = selection["collider"];
                        if ( collided != null ) {
                            GD.Print(collided);
                        }
                    }
                    
                    if ( !rotating )
                    {
                        if ( !dragging && mouseEvent.Pressed ) {
                            dragging = true;
                        } else if ( dragging && !mouseEvent.Pressed) {
                            dragging = false;
                        }
                    }

                } else if (mouseEvent.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    moveCamera.y = zoomSpeed * -1;
                    doMove = true;

                } else if (mouseEvent.ButtonIndex == (int)ButtonList.WheelUp)
                {
                    moveCamera.y = zoomSpeed;
                    doMove = true;

                } else if ( !dragging && mouseEvent.ButtonIndex == (int)ButtonList.Right ) {
                    if ( !rotating && mouseEvent.Pressed ) {
                        rotating = true;
                    } else if ( rotating && !mouseEvent.Pressed) {
                        rotating = false;
                    }

                }
            } else if (@event is InputEventMouseMotion && (dragging || rotating) ) {
                Vector2 mouseRelative = ((InputEventMouseMotion)@event).Relative;
                moveCamera.x = mouseRelative.x;
                moveCamera.z = mouseRelative.y;

            }
            

            if ( rotating ) {
                rotationHelperX.RotateX(Mathf.DegToRad(moveCamera.z * mouseSpeed * -1));
                Orthonormalize(rotationHelperX);
            }
            
            if ( dragging || doMove ) {
                rotationHelperX.GlobalTranslate(moveCamera * dragDirection * dragSpeed);
            }

        }

        // public void _input(InputEvent @event)
        // {
        //    if (@event is InputEventMouseButton)
        //    {
        //        InputEventMouseButton mouseEvent = (InputEventMouseButton)@event;
        //        if (mouseEvent.ButtonIndex == (int)ButtonList.Left && mouseEvent.Pressed)
        //        {
        //            Vector3 from = camera.ProjectRayOrigin(mouseEvent.Position);
        //            Vector3 to = from + camera.ProjectRayNormal(mouseEvent.Position) * 1000;

        //            PhysicsDirectSpaceState3D spaceStace = GetWorld3d().DirectSpaceState;

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

        // }

        private void Orthonormalize(Node3D node) {
            node.SetTransform(node.GetTransform().Orthonormalized());

        }

    }
}
