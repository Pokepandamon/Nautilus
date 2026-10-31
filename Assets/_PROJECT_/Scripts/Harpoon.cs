
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    // Fields
    [SerializeField] private SubUpgrades _subUpgrades;
    
    // The part of the harpoon that actually moves
    [SerializeField] private GameObject _harpoon;
    
    // How long the harpoon is out for
    [SerializeField] private float _harpoonTime;
    
    // Harpoon Distances
    [SerializeField] private float[] _harpoonUpgrades;
    
    //
    [SerializeField] private GameObject _ropeSource;

    private float _initialTime;
    private bool _reachedEnd;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _initialTime = Time.fixedTime;
    }
    
    private void Update()
    {
        if (!_reachedEnd)
        {
            _harpoon.transform.Translate(2 * _harpoonUpgrades[_subUpgrades.Harpoon] * Time.deltaTime / _harpoonTime, 0, 0);
            //_harpoon.transform.position = transform.position + new Vector3(Mathf.Lerp(0, _harpoonUpgrades[_subUpgrades.Harpoon], 2 * (Time.fixedTime - _initialTime) / _harpoonTime), _harpoon.transform.position.y, _harpoon.transform.position.z);
        }
        else
        {
            _harpoon.transform.Translate(-2 * _harpoonUpgrades[_subUpgrades.Harpoon] * Time.deltaTime / _harpoonTime, 0, 0);
            if (Time.fixedTime - _initialTime >= _harpoonTime)
            {
                Destroy(gameObject);
            }
            //_harpoon.transform.position = new Vector3(Mathf.Lerp(_harpoonUpgrades[_subUpgrades.Harpoon], 0, 2 * ((Time.fixedTime - _initialTime) / _harpoonTime - 1)), _harpoon.transform.position.y, _harpoon.transform.position.z) + transform.position;
        }
        _reachedEnd = 2 * (Time.fixedTime - _initialTime) / _harpoonTime > 1;
    }
    
    // Public Methods
    public GameObject GetRopeSource()
    {
        return _ropeSource;
    }
    
    // Private Methods
    
}
