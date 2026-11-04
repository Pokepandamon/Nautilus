using System;
using UnityEngine;

public class HarpoonTip : MonoBehaviour
{
    public event Action<Fish> HitFish;

    private void OnTriggerEnter(Collider other) => TryHit(other);
    private void OnCollisionEnter(Collision collision) => TryHit(collision.collider);

    private void TryHit(Collider other)
    {
        Fish fish = other.GetComponentInParent<Fish>();
        if (fish != null)
            HitFish?.Invoke(fish);
    }
}