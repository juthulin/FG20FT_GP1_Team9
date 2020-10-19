using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenuButton : MonoBehaviour
{
    Button mainMenuButton;
    
    void Awake()
    {
        mainMenuButton = GetComponent<Button>();
        mainMenuButton.onClick.AddListener(OnMenuButtonPress);
    }

    void OnMenuButtonPress()
    {
        SceneManager.LoadScene(0);
    }
}
