using System;
using Godot;

namespace Soulslism {
    class InputStateMachine {
        
        private InputState currentState = null;

        private Camera3D camera;

        private Node3D rotationHelperX;

        private InputStateIdle isIdle;

        public InputStateMachine(Camera3D camera, Node3D rotationHelperX)
        {
            this.camera = camera;
            this.rotationHelperX = rotationHelperX;

            this.isIdle = initState<InputStateIdle>(typeof(InputStateIdle));
            
            this.currentState = this.isIdle;
        }

        private T initState<T> (Type t) where T: InputState {
            T state = (T) Activator.CreateInstance(t);
            state.setMachine(this);
            return state;
        }

        public void processInput(InputAction action, InputEvent @event) {
            this.currentState.processInput(action);
        }
    }

    class InputStateIdle : InputState
    {
        public override InputState processInput(InputAction action)
        {
            GD.Print("processInput override");
            if ( action == InputAction.DRAG_SCREEN_START ) {
                return null;
            }

            return null;
        }
    }
}
