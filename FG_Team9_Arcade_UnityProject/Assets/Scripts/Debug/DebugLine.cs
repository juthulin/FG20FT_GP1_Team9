using UnityEngine;

// This script will draw a line from this.transform.position to a target, the length of the line can also be set to a specified amount
public class DebugLine : MonoBehaviour
{
    public Transform target;
    public Color debugColor = Color.white;
    public bool clampLength = true;
    public float lineLengthIfUnclamped = 100f;
    
    void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        if (clampLength)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
        else
        {
            Gizmos.DrawLine(transform.position,
                transform.position + (target.position - transform.position).normalized * lineLengthIfUnclamped);
        }
        Gizmos.color = Color.white;
    }
}
