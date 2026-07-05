using UnityEngine;

public abstract class BaseObjectiveChannel : ScriptableObject
{
    public event System.Action<float> OnRaised;
    public bool useDonation = false ;
    public Sprite donationSprite;
    public void Raise(float amount)
    {
        OnRaised?.Invoke(amount);
    }

    public abstract void OnCompleted();
}