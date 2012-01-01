using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Dynamics;
using Jitter.DataStructures;

namespace DuckEngine.MapEditor
{
    internal class SaveStateManager
    {
        private LinkedList<StateChange> savedStates = new LinkedList<StateChange>();
        private LinkedListNode<StateChange> currentState = null;

        /*public void saveState(IEnumerable<RigidBody> bodies)
        {
            SaveState[] bodyStates = new SaveState[bodies.Count()];
            int i = 0;
            foreach (RigidBody body in bodies)
            {
                bodyStates[i++] = new SaveStates.BodyPosition(body);
            }
            saveState(new SaveStates.Composite(bodyStates));
        }*/

        /*public void saveState(RigidBody body)
        {
            saveState(new SaveStates.BodyPosition(body));
        }*/

        public SaveStateManager()
        {
            //the first state should always be null
            savedStates.AddFirst(new LinkedListNode<StateChange>(null));
            currentState = savedStates.Last;
        }

        public void saveState(StateChange state)
        {
            if (state == null)
            {
                throw new ArgumentNullException();
            }
            removeStatesAfter(currentState);
            savedStates.AddLast(state);
            currentState = savedStates.Last;
        }

        public StateChange redo()
        {
            Console.WriteLine("redo");
            if (currentState.Next == null) //if already at last state
            {
                return currentState.Value;
            }
            currentState = currentState.Next;
            currentState.Value.redo();
            return currentState.Value;
        }

        public StateChange undo()
        {
            Console.WriteLine("undo");
            if (currentState.Value == null) //if already at first state
            {
                return null;
            }
            currentState.Value.undo();
            currentState = currentState.Previous;
            return currentState.Value; //null if first state
        }
        private void removeStatesAfter(LinkedListNode<StateChange> state)
        {
            if (state != null)
            {
                state = state.Next;
                while (state != null)
                {
                    LinkedListNode<StateChange> nextState = state.Next;
                    savedStates.Remove(state);
                    state = nextState;
                }
            }
        }
    }
}
