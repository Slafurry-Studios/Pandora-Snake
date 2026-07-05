using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class EntityBrain : MonoBehaviour
    {
        [Header("Universal References")]
        public Transform Target;
        public Animator aiAnimation;

        [Header("State Priority List (Top = Highest)")]
        [Tooltip("The brain checks this list from top to bottom. The first state whose conditions are met will run!")]
        public List<EntityState> stateList = new List<EntityState>();

        /// <summary>
        /// Any component implementing IEntityMovement works here - NPCMovement (Humanoid)
        /// or CarMovement (Cars) are both valid, and more can be added later without
        /// touching EntityBrain again.
        /// </summary>
        public IEntityMovement Movement { get; private set; }
        private EntityState currentState;

        private void Awake()
        {
            Movement = GetComponent<IEntityMovement>();
            if (Movement == null)
            {
                Debug.LogError($"{gameObject.name}: EntityBrain requires a component implementing IEntityMovement (e.g. NPCMovement or CarMovement).");
            }

            if (aiAnimation == null) aiAnimation = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            EvaluateStates();

            if (currentState != null)
            {
                currentState.UpdateState(this);
            }
        }

        private void Start() // Move logic here
        {
            if (Target == null && PlayerManager.PlayerTransform != null)
            {
                Target = PlayerManager.PlayerTransform;
            }

            // Safety fallback if PlayerManager wasn't ready
            if (Target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) Target = player.transform;
            }
        }

        private void EvaluateStates()
        {
            foreach (EntityState state in stateList)
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