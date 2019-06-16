using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace SoulslismCSharp
{

    class CameraController
    {
        private float mouseSpeed = 0.25f;

        private float dragSpeed = 0.25f;
        private int dragDirection = -1;

        private float zoomSpeed = 6;

        private Camera camera;

        private Spatial rotationHelperX;

        private Vector3 moveCamera = new Vector3();

        private Boolean dragging;
        private Boolean rotating;

        public CameraController(Camera camera, Spatial rotationHelperX)
        {
            this.camera = camera;
            this.rotationHelperX = rotationHelperX;

        }

        public void _input(InputEvent @event)
        {

            Boolean doMove = false;
            moveCamera.x = 0;
            moveCamera.y = 0;
            moveCamera.z = 0;

           if (@event is InputEventMouseButton)
           {
               InputEventMouseButton mouseEvent = (InputEventMouseButton)@event;
               if ( !rotating && mouseEvent.ButtonIndex == (int)ButtonList.Left)
               {
                    if ( !dragging && mouseEvent.Pressed ) {
                        dragging = true;
                    } else if ( dragging && !mouseEvent.Pressed) {
                        dragging = false;
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
                rotationHelperX.RotateX(Mathf.Deg2Rad(moveCamera.z * mouseSpeed * -1));
                Orthonormalize(rotationHelperX);
                // camera.RotateY(Mathf.Deg2Rad(moveCamera.x * mouseSpeed));
                // Orthonormalize(camera);                


        // rotation_helper.rotate_x(deg2rad(event.relative.y * MOUSE_SENSITIVITY))
        // self.rotate_y(deg2rad(event.relative.x * MOUSE_SENSITIVITY * -1))

        // var camera_rot = rotation_helper.rotation_degrees
        // camera_rot.x = clamp(camera_rot.x, -70, 70)
        // rotation_helper.rotation_degrees = camera_rot

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

        // }

        private void Orthonormalize(Spatial node) {
            node.SetTransform(node.GetTransform().Orthonormalized());

        }

    }
}
