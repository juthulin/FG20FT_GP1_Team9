using System.Collections;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{

    public Transform teleportPosition;
    public int allowedSecondsOutOfBounds = 5;

    int counter = 0;
    bool isInPlayArea = true;

    void Awake()
    {
        counter = allowedSecondsOutOfBounds;
    }

    void OnTriggerEnter(Collider other) // player is in play area.
    {
        if (other.CompareTag("Player"))
        {
            isInPlayArea = true;
            counter = allowedSecondsOutOfBounds;
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
            isInPlayArea = false;
            GameObject go = other.gameObject;
            StartCoroutine(CountDown(go));
            WaypointManager.Instance.PlayerOutOfBounds(true);
            UIManager.instance.SetOutOfBoundsScreen(true);
        }
    }

    
    IEnumerator CountDown(GameObject go)
    {
        while (counter > 0 && !isInPlayArea)
        {
            print("Out of bounds! Return to play area. -> " + counter.ToString());
            UIManager.instance.UpdateOutOfBoundsTimer(counter);
            counter--;
            yield return new WaitForSeconds(1);
        }
    
        if (!isInPlayArea)
        {
            print("Game over! You died from the fog.");
            if (PickupManager.Instance.playerIsHoldingPickup) PickupManager.Instance.FailDelivery();
            go.transform.position = teleportPosition.position;
        }
    }
}
