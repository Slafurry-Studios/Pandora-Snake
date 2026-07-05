using UnityEngine;

namespace Game.AI
{
    /// <summary>
    /// Base component script for any AI state or behavior (e.g. Chase, Attack, Land).
    /// Attach derived state scripts to the AI entity and link them into EntityBrain's State List.
    /// </summary>
    public abstract class EntityState : MonoBehaviour
    {
        /// <summary>
        /// Evaluates whether this state should currently be active.
        /// Return true if conditions are met (e.g. target in range), false otherwise.
        /// </summary>
        public abstract bool CheckConditions(EntityBrain brain);

        /// <summary>
        /// Called once the exact frame EntityBrain switches INTO this state.
        /// Use this to initialize timers, animations, or target selection.
        /// </summary>
        public abstract void EnterState(EntityBrain brain);

        /// <summary>
        /// Called every frame (in Update) while this state is active.
        /// Perform continuous movement, aiming, or firing logic here.
        /// </summary>
        public abstract void UpdateState(EntityBrain brain);

        /// <summary>
        /// Called once the exact frame EntityBrain switches OUT OF this state.
        /// Use this to reset movement, stop firing, or clean up temporary state.
        /// </summary>
        public abstract void ExitState(EntityBrain brain);
    }
}