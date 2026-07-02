using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public event Action<float> OnStaminaPctChanged;

        [Header("Movement Settings")]
        [SerializeField] private float forwardSpeed = 5f;
        [SerializeField] private float turnSpeed = 360f; // Kecepatan berputar (derajat per detik)

        [Header("Sprint & Stamina Settings")]
        [SerializeField] private float sprintMultiplier = 1.5f;
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 20f;
        [SerializeField] private float staminaRegenRate = 15f;
        [SerializeField] private float staminaRegenDelay = 1f;

        [Header("Input")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference sprintAction;

        [Header("Objective")]
        [SerializeField] private BaseObjectiveChannel[] sprintChannel;

        private Rigidbody2D rb;
        private float targetAngle;

        private float currentStamina;
        private bool isSprinting;
        private float regenTimer;
        private bool isExhausted;

        private void Awake()
        {
            rb = GetComponentInParent<Rigidbody2D>();
            rb.gravityScale = 0f;
            targetAngle = rb.rotation;

            currentStamina = maxStamina;
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

        private void Start()
        {
            OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
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

            // Simpan state sprint SEBELUM diubah oleh HandleSprintingAndStamina,
            // supaya transisi (mulai/berhenti sprint) bisa terdeteksi dengan benar.
            bool wasSprinting = isSprinting;

            HandleSprintingAndStamina();

            if (sprintAction != null)
            {
                // Bandingkan pakai isSprinting (state final, sudah memperhitungkan
                // exhaustion & sisa stamina), bukan raw input pressed.
                if (isSprinting && !wasSprinting)
                {
                    foreach (BaseObjectiveChannel channel in sprintChannel)
                    {
                        channel.Raise(1);
                    }
                }
                else if (!isSprinting && wasSprinting)
                {
                    foreach (BaseObjectiveChannel channel in sprintChannel)
                    {
                        channel.Raise(0);
                    }
                }
            }
        }

        private void HandleSprintingAndStamina()
        {
            float previousStamina = currentStamina;

            bool sprintInputHeld = sprintAction != null && sprintAction.action.IsPressed();

            if (!sprintInputHeld)
            {
                isExhausted = false;
            }

            if (sprintInputHeld && !isExhausted && currentStamina > 0f)
            {
                isSprinting = true;
                currentStamina -= staminaDrainRate * Time.deltaTime;
                currentStamina = Mathf.Max(currentStamina, 0f);

                regenTimer = staminaRegenDelay;

                if (currentStamina <= 0f)
                {
                    isExhausted = true;
                }
            }
            else
            {
                isSprinting = false;

                if (regenTimer > 0f)
                {
                    regenTimer -= Time.deltaTime;
                }
                else if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);
                }
            }

            if (currentStamina != previousStamina)
            {
                OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
            }
        }

        public float GetStaminaNormalized()
        {
            return currentStamina / maxStamina;
        }

        public void SetStamina(float value)
        {
            float previousStamina = currentStamina;
            currentStamina = Mathf.Clamp(value, 0f, maxStamina);
            if (currentStamina != previousStamina)
            {
                OnStaminaPctChanged?.Invoke(currentStamina / maxStamina);
            }
        }


        private void FixedUpdate()
        {
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);

            float currentSpeed = forwardSpeed * (isSprinting ? sprintMultiplier : 1f);
            rb.velocity = transform.up * currentSpeed;

            rb.angularVelocity = 0f;
        }

        public float CurrentSpeed
        {
            get
            {
                return forwardSpeed * (isSprinting ? sprintMultiplier : 1f);
            }
        }

        public bool GetSprint()
        {
            return isSprinting;
        }

        public void InfiniteCardio()
        {
            maxStamina = maxStamina * 1.1f;
        }

        public void LegDay()
        {
            sprintMultiplier = sprintMultiplier * 1.15f;
        }
    }
}