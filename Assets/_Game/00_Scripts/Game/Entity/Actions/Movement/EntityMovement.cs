using Game.AI;
using Slafurry.System.Audio;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IEntityMovement
{
    [Header("Steering Settings")]
    public float rayDistance = 1.5f;
    public LayerMask obstacleLayer;
    public int rayCount = 8;
    public float rayAngle = 90f;


    [Header("Animation")]
    [SerializeField] private string chaseBool = "Chase";

    [Header("Audio")]
    public string SFXCategory;
    public string movementSoundName;

    private Rigidbody2D rb;
    private EntityBrain brain;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EntityBrain>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void FaceDirection(Vector2 direction)
    {
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }

    bool wasMoving = false;


    public void SetMovement(Vector2 desiredDirection, float speed)
    {
        bool isMoving = desiredDirection != Vector2.zero && speed > 0f;

        if (isMoving)
        {
            FaceDirection(desiredDirection);
        }


        if (brain != null && brain.aiAnimation != null && !string.IsNullOrEmpty(chaseBool))
        {
            brain.aiAnimation.SetBool(chaseBool, isMoving);

            if (isMoving && !wasMoving)
            {
                Audio.PlaySFX2D(SFXCategory, movementSoundName);
            }
        }

        wasMoving = isMoving;


        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            return;
        }


        Vector2 finalDirection = Steer(desiredDirection);

        rb.velocity = finalDirection * speed;
    }


    private Vector2 Steer(Vector2 desired)
    {
        Vector2 final = desired;


        for (int i = 0; i < rayCount; i++)
        {
            float angle =
                Mathf.Lerp(
                    -rayAngle / 2,
                    rayAngle / 2,
                    (float)i / (rayCount - 1)
                );


            Vector2 dir =
                Quaternion.Euler(0, 0, angle)
                * desired;


            RaycastHit2D hit =
                Physics2D.Raycast(
                    transform.position,
                    dir,
                    rayDistance,
                    obstacleLayer
                );


            if (hit.collider != null)
            {
                Vector2 awayFromWall =
                    Vector2.Reflect(
                        desired,
                        hit.normal
                    );


                final += awayFromWall * 0.5f;
            }
        }


        return final.normalized;
    }
}