using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class UIManager : MonoBehaviour
{
    int t;
    
    [SerializeField] float messageDuration = 1f;
    [Space]
    [SerializeField] Image healthBarRight;
    [SerializeField] Image healthBarLeft;
    [SerializeField] GameObject deliveryDeadlineParent;
    [SerializeField] Text deliveryDeadlineText;
    [SerializeField] Text pointsText;
    [SerializeField] Text deliveryFailed;
    [SerializeField] Text deliverySuccess;
    [SerializeField] Image empAvailable;
    [SerializeField] Image empChargeBar;
    [SerializeField] Image empGlow;
    [SerializeField] GameObject outOfBoundsScreen;
    [SerializeField] Text outOfBoundsTimer;
    [SerializeField] Image[] crossArray;

    public static UIManager instance;
    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        t = 0;
        empChargeBar.fillAmount = 0f;
        deliveryFailed.gameObject.SetActive(false);
        deliverySuccess.gameObject.SetActive(false);
        outOfBoundsScreen.SetActive(false);
        
        foreach (Image item in crossArray)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void SetHealthBarActive(bool playerCarryingCargo)
    {
        healthBarRight.fillAmount = playerCarryingCargo ? 1f : 0f;
        healthBarLeft.fillAmount = playerCarryingCargo ? 1f : 0f;
    }
    
    public void SetHealthBarAmount(float currentHealth, float maxHealth)
    {
        float fillAmount = (currentHealth <= 0f) ? 0f : currentHealth / maxHealth;
        healthBarRight.fillAmount = fillAmount;
        healthBarLeft.fillAmount = fillAmount;
    }

    public void SetEmpGlowActive(bool isActive)
    {
        empGlow.gameObject.SetActive(isActive);
    }

    public void SetEmpAvailableActive(bool isActive)
    {
        empAvailable.gameObject.SetActive(isActive);
    }

    public void SetEmpChargeBarAmount(float currentAmount, float maxAmount)
    {
        float fillAmount = (currentAmount <= 0f) ? 0f : currentAmount / maxAmount;
        empChargeBar.fillAmount = fillAmount;
    }

    public void UpdateFailedDeliveriesAmount()
    {
        if (t >= crossArray.Length) return;
        
        crossArray[t].gameObject.SetActive(true);
        t++;
    }

    public void SetOutOfBoundsScreen(bool isActive)
    {
        outOfBoundsScreen.SetActive(isActive);
    }

    public void UpdateOutOfBoundsTimer(int counter)
    {
        outOfBoundsTimer.text = counter.ToString();
    }

    public void SetPointsText(int pointAmount)
    {
        pointsText.text = pointAmount.ToString();
    }

    public void SetDeliveryDeadlineText(string timeLeft)
    {
        deliveryDeadlineText.text = timeLeft;
    }
    
    public void ShowDeliveryFailedMessage()
    {
        StartCoroutine(DeliveryMessageFailed());
    }
    
    public void ShowDeliverSuccessMessage()
    {
        StartCoroutine(DeliveryMessageSuccess());
    }

    public void DeliveryDeadlineVisible(bool visible)
    {
        deliveryDeadlineParent.SetActive(visible);
    }

    IEnumerator DeliveryMessageSuccess()
    {
        deliverySuccess.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        deliverySuccess.gameObject.SetActive(false);
    }
    
    IEnumerator DeliveryMessageFailed()
    {
        deliveryFailed.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        deliveryFailed.gameObject.SetActive(false);
    }
}
