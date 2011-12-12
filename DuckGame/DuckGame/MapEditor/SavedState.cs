using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Dynamics;
using Jitter.DataStructures;

namespace DuckEngine.MapEditor
{
    internal struct SavedBodyState
    {
        public readonly JVector savedPosition;
        public readonly JMatrix savedOrientation;
        public readonly RigidBody belongedTo;

        public SavedBodyState(RigidBody body)
        {
            savedPosition = body.Position;
            savedOrientation = body.Orientation;
            belongedTo = body;
        }

        public void restore()
        {
            belongedTo.Position = savedPosition;
            belongedTo.Orientation = savedOrientation;
        }
    }
    internal class SavedState
    {
        private LinkedList<SavedBodyState[]> savedStates = new LinkedList<SavedBodyState[]>();
        private LinkedListNode<SavedBodyState[]> currentState = null;

        public void saveState(ReadOnlyHashset<RigidBody> rigidBodies)
        {
            SavedBodyState[] bodyStates = new SavedBodyState[rigidBodies.Count];
            int i = 0;
            foreach (RigidBody body in rigidBodies)
            {
                bodyStates[i++] = new SavedBodyState(body);
            }
            if (currentState != null)
            {
                while (currentState.Next != null)
                {
                    savedStates.Remove(currentState.Next);
                }
            }
            savedStates.AddLast(bodyStates);
            currentState = savedStates.Last;
        }

        public void redo()
        {
            Console.WriteLine("redo");
            if (currentState != null && currentState.Next != null)
            {
                currentState = currentState.Next;
                restoreCurrentState();
            }
        }

        public void undo()
        {
            Console.WriteLine("undo");
            if (currentState != null && currentState.Previous != null) {
                currentState = currentState.Previous;
                restoreCurrentState();
            }
        }

        private void restoreCurrentState()
        {
            foreach (SavedBodyState state in currentState.Value)
            {
                state.restore();
            }
        }
    }
}
