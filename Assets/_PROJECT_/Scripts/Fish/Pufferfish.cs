using UnityEngine;

[RequireComponent(typeof(Fish))]
public class Pufferfish : MonoBehaviour
{
    // Fields
    [SerializeField] private float _puffSize;
    [SerializeField] private float _puffSpeed = 5f;
    
    private Fish _fish;
    private Vector3 _initialScale;
    private Vector3 _targetScale;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
        _initialScale = transform.localScale;
        _targetScale = _initialScale;
    }

    private void OnEnable()
    {
        _fish.DetectedSubmarine += Puff;
        _fish.LostSubmarine += Deflate;
    }

    private void OnDisable()
    {
        _fish.DetectedSubmarine -= Puff;
        _fish.LostSubmarine -= Deflate;
    }

    private void Update()
    {
        float t = 1f - Mathf.Exp(-_puffSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, t);
    }

    // Public Methods
    
    // Private Methods
    private void Puff(GameObject _)
    {
        _targetScale = Vector3.one * _puffSize;
    }

    private void Deflate()
    {
        _targetScale = _initialScale;
    }
}
