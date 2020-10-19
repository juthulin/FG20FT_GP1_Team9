using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-52)]
public class GameManager : MonoBehaviour
{
    public static GameObject Player { get; private set; }
    public static Camera PlayerCamera { get; private set; } 
    public static GameManager Instance { get; private set; }
    
    public static int currentPoints;
    
    [Tooltip("The target gameObject in the player which the initial direction will be used and the bullet will line up with this ray")]
    public Transform playerTarget;
    public Transform projectilePool;

    [NonSerialized] public Transform newPlayerTarget;

    private void OnValidate()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        PlayerCamera = Camera.main;
        
        GameObject[] playerTaggedObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in playerTaggedObjects)
        {
            if (obj.transform.parent == null)
            {
                Player = obj;
            }
        }
        Assert.IsNotNull(Player, "Could not find an object with tag \"Player\", make sure a player exists in the scene and that it is tagged with tag \"Player\"");
    }

    public static void OnGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainSceneUpdated");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        UIManager.instance.SetPointsText(currentPoints);
    }
}
