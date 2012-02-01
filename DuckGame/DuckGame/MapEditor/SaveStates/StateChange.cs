using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using DuckEngine.MapEditor.SaveStates;

namespace DuckEngine.MapEditor
{
    class StateChange
    {
        private IEnumerable<RigidBodyState> statesBefore;
        private IEnumerable<RigidBodyState> statesAfter;
        private List<PhysicalEntity> created = new List<PhysicalEntity>();
        private List<PhysicalEntity> deleted = new List<PhysicalEntity>();
        private IEnumerable<RigidBody> bodiesBefore;
        public IEnumerable<RigidBody> Before
        {
            get { return bodiesBefore; }
            set {
                if (value == null)
                {
                    bodiesBefore = new RigidBody[0];
                }
                else
                {
                    bodiesBefore = value;
                }
                statesBefore = bodyStates(bodiesBefore);
                updateCreatedAndDeleted();
            }
        }
        private IEnumerable<RigidBody> bodiesAfter;
        public IEnumerable<RigidBody> After
        {
            get { return bodiesAfter; }
            set
            {
                if (value == null)
                {
                    bodiesAfter = new RigidBody[0];
                }
                else
                {
                    bodiesAfter = value;
                }
                statesAfter = bodyStates(bodiesAfter);
                updateCreatedAndDeleted();
            }
        }

        public StateChange(IEnumerable<RigidBody> _bodiesBefore)
        {
            Before = _bodiesBefore;
        }

        public void redo()
        {
            restoreState(statesAfter);
            create(created);
            delete(deleted);
        }

        public void undo()
        {
            restoreState(statesBefore);
            create(deleted);
            delete(created);
        }

        private void updateCreatedAndDeleted()
        {
            created.Clear();
            deleted.Clear();

            if (bodiesBefore != null && bodiesAfter != null)
            {
                IEnumerable<RigidBody> createdBodies = bodiesAfter.Except(bodiesBefore);
                IEnumerable<RigidBody> deletedBodies = bodiesBefore.Except(bodiesAfter);

                foreach (RigidBody body in createdBodies)
                {
                    if (body.Tag is PhysicalEntity)
                    {
                        created.Add((PhysicalEntity)body.Tag);
                    }
                }
                foreach (RigidBody body in deletedBodies)
                {
                    if (body.Tag is PhysicalEntity)
                    {
                        deleted.Add((PhysicalEntity)body.Tag);
                    }
                }
            }
        }

        private static void restoreState(IEnumerable<RigidBodyState> state)
        {
            foreach (RigidBodyState bodyState in state)
            {
                bodyState.restore();
            }
        }

        private static void create(IEnumerable<PhysicalEntity> created)
        {
            foreach (PhysicalEntity entity in created)
            {
                entity.EnableInterfaceCalls = true;
            }
        }

        private static void delete(IEnumerable<PhysicalEntity> deleted)
        {
            foreach (PhysicalEntity entity in deleted)
            {
                entity.EnableInterfaceCalls = false;
            }
        }

        private static IEnumerable<RigidBodyState> bodyStates(IEnumerable<RigidBody> bodies)
        {
            RigidBodyState[] bodyStates = new RigidBodyState[bodies.Count()];
            int i = 0;
            foreach (RigidBody body in bodies)
            {
                bodyStates[i++] = body.Save();
            }
            return bodyStates;
        }
    }
}
