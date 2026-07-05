using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float frequency = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    public void SetStartPosition(Vector3 newStart)
    {
        startPos = newStart;
    }
}