using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Play Button Object")]
    [SerializeField] Button playButton = default;
    [Header("Keymap Button Object")]
    [SerializeField] Button keymapButton = default;
    [Header("Leaderboard Button Object")]
    [SerializeField] Button leaderBoardButton = default;
    [Header("Credits Button Object")]
    [SerializeField] Button creditsButton = default;
    [Header("Button to close down the game")]
    [SerializeField] Button quitButton = default;
    [Space]
    [Header("Menu Screens")]
    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject keymapScreen;
    [SerializeField] GameObject leaderboardScreen;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] TMP_Text leaderboard;
    
    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonPress);
        keymapButton.onClick.AddListener(OnKeymapButtonPress);
        leaderBoardButton.onClick.AddListener(OnLeaderBoardButtonPress);
        creditsButton.onClick.AddListener(OnCreditsButtonPress);
        quitButton.onClick.AddListener(OnQuitButtonPress);
        
        mainMenuScreen.SetActive(true);
        keymapScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    void OnPlayButtonPress()
    {
        SceneManager.LoadScene(1);
    }

    void OnKeymapButtonPress()
    {
        mainMenuScreen.SetActive(false);
        keymapScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }
    
    void OnLeaderBoardButtonPress()
    {
        mainMenuScreen.SetActive(false);
        keymapScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        creditsScreen.SetActive(false);
        HighScoreSave.UpdateLeaderBoard(ref leaderboard);
    }

    void OnCreditsButtonPress()
    {
        mainMenuScreen.SetActive(false);
        keymapScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void OnMainMenuButtonPress()
    {
        mainMenuScreen.SetActive(true);
        keymapScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }
    
    void OnQuitButtonPress()
    {
        Application.Quit();
        print("QuitButton pressed. (Won't quit the game in the editor, but will work in the build).");
    }
}
