namespace Soulslism {
    abstract class InputState {

        private InputStateMachine machine;

        public void setMachine(InputStateMachine machine) {
            this.machine = machine;
        }

        virtual public InputState processInput(InputAction action) {
            return null;
        }

    }
}
