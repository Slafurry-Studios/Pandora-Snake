using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference sprintAction;

        [Header("Objective")]
        [SerializeField] private BaseObjectiveChannel[] sprintChannel;

        private float forwardSpeed;
        private float turnSpeed = 1000f;
        private float sprintMultiplier;

        private Rigidbody2D rb;
        private PlayerStamina stamina;
        private float targetAngle;
        private bool isSprinting;
        private bool initialized;

        public void Initialize()
        {
            Player player = GetComponentInParent<Player>();
            stamina = player.PlayerStamina;

            rb = player.RigidBody2D;
            rb.gravityScale = 0f;
            targetAngle = rb.rotation;

            forwardSpeed = player.PlayerData.PlayerSpeed;
            sprintMultiplier = player.PlayerData.PlayerMovementData.PlayerSpeedMultiplier;

            initialized = true;
        }

        private void OnEnable()
        {
            if (moveAction != null) moveAction.action.Enable();
            if (sprintAction != null) sprintAction.action.Enable();
        }

        private void OnDisable()
        {
            if (moveAction != null) moveAction.action.Disable();
            if (sprintAction != null) sprintAction.action.Disable();
        }

        private void Update()
        {
            if (moveAction != null)
            {
                Vector2 input = moveAction.action.ReadValue<Vector2>();

                if (input.sqrMagnitude > 0.01f)
                {
                    if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    {
                        input.y = 0;
                        input.x = Mathf.Sign(input.x);
                    }
                    else
                    {
                        input.x = 0;
                        input.y = Mathf.Sign(input.y);
                    }

                    targetAngle = Vector2.SignedAngle(Vector2.up, input);
                }
            }

            bool wasSprinting = isSprinting;
            HandleSprint();

            if (sprintAction != null)
            {
                if (isSprinting && !wasSprinting)
                {
                    foreach (var channel in sprintChannel) channel.Raise(1);
                }
                else if (!isSprinting && wasSprinting)
                {
                    foreach (var channel in sprintChannel) channel.Raise(0);
                }
            }
        }

        private void HandleSprint()
        {
            if (!initialized) return;

            bool sprintInputHeld = sprintAction != null && sprintAction.action.IsPressed();

            if (!sprintInputHeld)
                stamina.ResetExhaustion();

            if (sprintInputHeld && stamina.CanSprint)
            {
                isSprinting = true;
                stamina.Drain(Time.deltaTime);
            }
            else
            {
                isSprinting = false;
                stamina.Regen(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (!initialized) return;

            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);

            float currentSpeed = forwardSpeed * (isSprinting ? sprintMultiplier : 1f);
            rb.velocity = transform.up * currentSpeed;

            rb.angularVelocity = 0f;
        }

        public float CurrentSpeed => forwardSpeed * (isSprinting ? sprintMultiplier : 1f);

        public bool GetSprint() => isSprinting;

        public void LegDay()
        {
            sprintMultiplier *= 1.15f;
        }
    }
}