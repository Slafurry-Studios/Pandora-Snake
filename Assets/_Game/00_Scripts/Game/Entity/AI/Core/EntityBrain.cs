using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
    public class EntityBrain : MonoBehaviour
    {
        public Transform Target { get; private set; }
        public Animator Animator { get; private set; }
        public IEntityMovement EntityMovement { get; private set; }
        public IEntityShoot EntityShoot { get; private set; }

        private List<EntityState> states;
        private EntityState currentState;
        private bool initialized;

        public void Initialize(List<EntityState> states, IEntityMovement entityMovement, IEntityShoot entityShoot, Animator animator)
        {
            EntityMovement = entityMovement;
            EntityShoot = entityShoot;

            this.states = states;

            if (EntityMovement == null)
            {
                Debug.LogError($"{gameObject.name}: EntityBrain requires a component implementing IEntityMovement (e.g. NPCMovement or CarMovement).");
            }

            if (Animator == null) Animator = animator;

            if (Target == null && PlayerManager.PlayerTransform != null)
            {
                Target = PlayerManager.PlayerTransform;
            }

            if (Target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) Target = player.transform;
            }

            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;

            EvaluateStates();

            if (currentState != null)
            {
                currentState.UpdateState(this);
            }
        }

        private void EvaluateStates()
        {
            foreach (EntityState state in states)
            {
                if (state.CheckConditions(this))
                {
                    if (currentState != state)
                    {
                        ChangeState(state);
                    }

                    return;
                }
            }
        }

        private void ChangeState(EntityState newState)
        {
            if (currentState != null) currentState.ExitState(this);
            currentState = newState;
            if (currentState != null) currentState.EnterState(this);
        }
    }
}