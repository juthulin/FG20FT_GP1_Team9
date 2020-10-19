using System.Collections;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{

    public Transform teleportPosition;
    public int allowedSecondsOutOfBounds = 5;

    int _counter = 0;
    bool _isInPlayArea = true;

    void Awake()
    {
        _counter = allowedSecondsOutOfBounds;
    }

    void OnTriggerEnter(Collider other) // player is in play area.
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayArea = true;
            _counter = allowedSecondsOutOfBounds;
            GameObject go = other.gameObject;
            StopCoroutine(CountDown(go));
            print("Returned to play area :)");
            WaypointManager.Instance.PlayerOutOfBounds(false);
            UIManager.instance.SetOutOfBoundsScreen(false);
        }
    }

    void OnTriggerExit(Collider other) // player is out of bounds.
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayArea = false;
            GameObject go = other.gameObject;
            StartCoroutine(CountDown(go));
            WaypointManager.Instance.PlayerOutOfBounds(true);
            UIManager.instance.SetOutOfBoundsScreen(true);
        }
    }

    
    IEnumerator CountDown(GameObject go)
    {
        while (_counter > 0 && !_isInPlayArea)
        {
            print("Out of bounds! Return to play area. -> " + _counter.ToString());
            UIManager.instance.UpdateOutOfBoundsTimer(_counter);
            _counter--;
            yield return new WaitForSeconds(1);
        }
    
        if (!_isInPlayArea)
        {
            print("Game over! You died from the fog.");
            if (PickupManager.Instance.playerIsHoldingPickup) PickupManager.Instance.FailDelivery();
            go.transform.position = teleportPosition.position;
        }
    }
}
