using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyHelper
{
    private const int NumViewDirections = 15;
    private const double GoldenRatio = 1.61803398875;
    public static readonly Vector3[] Directions;

    static EnemyHelper()
    {
        Directions = new Vector3[EnemyHelper.NumViewDirections];

        float angleIncrement = Mathf.PI * 2 * (float)GoldenRatio;

        for (int i = 0; i < NumViewDirections; i++)
        {
            float t = (float) i / NumViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            Directions[i] = new Vector3(x, y, z);
        }
    }
}
