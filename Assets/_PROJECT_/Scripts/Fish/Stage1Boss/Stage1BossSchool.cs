using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Fish))]
public class Stage1BossSchool : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform[] _points;
    [SerializeField] private GameObject[] _members;

    private Fish _fish;
    private Transform _submarine;
    private int _currentPoint = 0;
    private Stage1Boss[] _memberBoss;
    private Vector3[] _currentOffsets;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
        for(int i = 0; i < _members.Length; i++)
        {
            _memberBoss[i] = _members[i].GetComponent<Stage1Boss>();
        }

        // 5 is a place holder for how close we want them
        _currentOffsets = PackMinVolume(_members.Length, 5.0f).ToArray();
    }

    private void OnEnable()
    {
        _fish.DestinationReached += GoNextPoint;
    }

    private void OnDisable()
    {
        _fish.DestinationReached -= GoNextPoint;
    }

    private void Update()
    {
        if (_points == null || _points.Length == 0)
        {
            return;
        }

        Transform destination = _points[_currentPoint];

        if (destination != null)
        {
            _fish.SetDestination(destination.position);
        }
    }

    // Public Methods

    // Private Methods
    private void GoNextPoint()
    {
        if (_submarine != null || _points == null || _points.Length == 0)
        {
            return;
        }

        _currentPoint = (_currentPoint + 1) % _points.Length;
    }
    
    // This is an algorithm to calculate the packing in the school
    public static List<Vector3> PackMinVolume(int sphereCount, float radius)
    {
        float d = radius * 2.0f;
        
        // Characteristic step sizes for FCC/HCP lattice structures
        float dx = d;
        float dy = d * MathF.Sqrt(3.0f) / 2.0f;
        float dz = d * MathF.Sqrt(6.0f) / 3.0f;

        // Estimate a grid size large enough to harvest coordinates from
        int side = (int)MathF.Ceiling(MathF.Pow(sphereCount, 1.0f / 3.0f)) + 3;
        int minIdx = -side;
        int maxIdx = side;

        var candidates = new List<(Vector3 pos, float distSq)>();

        // Generate a dense lattice block centered at the origin
        for (int l = minIdx; l <= maxIdx; l++)
        {
            float z = l * dz;
            float offsetX = (l % 2 != 0) ? (dx / 2.0f) : 0.0f;
            float offsetY = (l % 2 != 0) ? (dy / 3.0f) : 0.0f;

            for (int r = minIdx; r <= maxIdx; r++)
            {
                float y = r * dy + offsetY;
                float rowOffsetX = ((r % 2 != 0) ? (dx / 2.0f) : 0.0f) + offsetX;

                for (int c = minIdx; c <= maxIdx; c++)
                {
                    float x = c * dx + rowOffsetX;
                    Vector3 pos = new Vector3(x, y, z);
                    
                    candidates.Add((pos, pos.sqrMagnitude));
                }
            }
        }

        // Deterministic sort: closest to origin first to minimize outer boundary volume
        // Stabilize sorting via coordinate components to guarantee strict determinism
        return candidates
            .OrderBy(c => c.distSq)
            .ThenBy(c => c.pos.x)
            .ThenBy(c => c.pos.y)
            .ThenBy(c => c.pos.z)
            .Select(c => c.pos)
            .Take(sphereCount)
            .ToList();
    }
}