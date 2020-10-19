using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] Transform[] objectsToDebug = default;

    public bool global = false;
    public bool local = true;
    [Range(1, 50)]
    public float length = 1;

    void OnDrawGizmos()
    {
        if (local)
        {
            foreach (Transform obj in objectsToDebug)
            {
                Vector3 position = obj.transform.position;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(position, position + obj.transform.up * length);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, position + obj.transform.right * length);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, position + obj.transform.forward * length);
            }
        }

        if (global)
        {
            foreach (Transform obj in objectsToDebug)
            {
                Vector3 position = transform.position;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(position, position + Vector3.up * length);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, position + Vector3.right * length);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(position, position + Vector3.forward * length);
            }
        }
    }
}