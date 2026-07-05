using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerAim : MonoBehaviour
    {
        [Header("Aim Settings")]
        [Tooltip("Assign the indicator or weapon object that will orbit the center.")]
        [SerializeField] private Transform aimIndicator;

        [Tooltip("The center point of the orbit. Usually the player's center. If empty, uses this object's transform.")]
        [SerializeField] private Transform orbitCenter;

        [Tooltip("Distance from the center point to the aim indicator.")]
        [SerializeField] private float orbitRadius = 1.5f;

        [Tooltip("Should the indicator also rotate to face the aim direction?")]
        [SerializeField] private bool rotateIndicator = true;

        [Tooltip("Offset rotation in degrees if the indicator's sprite doesn't point correctly. Example: -90 if sprite faces UP.")]
        [SerializeField] private float rotationOffset = -90f;

        private Camera mainCamera;
        private Vector3 currentAimDirection;

        /// <summary>Current normalized aim direction, read by PlayerShoot.</summary>
        public Vector3 CurrentAimDirection => currentAimDirection;

        /// <summary>World position of the aim indicator, used as bullet spawn point.</summary>
        public Vector3 AimIndicatorPosition => aimIndicator != null ? aimIndicator.position : transform.position;

        public Transform AimIndicator => aimIndicator;
        public Transform OrbitCenter => orbitCenter;

        private void Awake()
        {
            mainCamera = Camera.main;

            if (orbitCenter == null)
            {
                orbitCenter = transform;
            }

            if (aimIndicator == null)
            {
                aimIndicator = transform;
            }
        }

        private void Update()
        {
            AimAtCursor();
        }

        private void AimAtCursor()
        {
            if (mainCamera == null || aimIndicator == null || orbitCenter == null) return;
            if (Mouse.current == null) return;
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.nearClipPlane));
            mouseWorldPos.z = orbitCenter.position.z;

            Vector3 aimDirection = (mouseWorldPos - orbitCenter.position).normalized;
            currentAimDirection = aimDirection;

            if (aimDirection != Vector3.zero)
            {
                aimIndicator.position = orbitCenter.position + aimDirection * orbitRadius;

                if (rotateIndicator)
                {
                    float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                    aimIndicator.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
                }
            }
        }
    }
}