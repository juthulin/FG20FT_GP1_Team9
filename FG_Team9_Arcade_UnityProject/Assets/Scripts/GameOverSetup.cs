using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverSetup : MonoBehaviour
{
    public float timeToStartJazz = 41f;
    public TMP_Text leaderboard;
    public TMP_Text currentScore;
    public TMP_InputField nameField;
    
    private void Awake()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        currentScore.text = $"Current Score: {GameManager.currentPoints}";
        HighScoreSave.UpdateLeaderBoard(ref leaderboard);
        StartCoroutine(PlayMusic());
    }

    private void Start()
    {
        MusicManager.Instance.GameOverMusic();
    }

    IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(timeToStartJazz);
        MusicManager.Instance.PlayCrossFadeMusic(MusicManager.Instance.jazzMusic, 0.1f);
    }

    public void SaveScore()
    {
        int score = GameManager.currentPoints;
        Debug.Log("Score Saved with name: " + nameField.text);
        HighScoreSave.Save(nameField.text, score);
        HighScoreSave.UpdateLeaderBoard(ref leaderboard);
    }
}
