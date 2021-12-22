using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrawSceneManager : MonoBehaviour
{
    [SerializeField] Button goNextSceneButton;

    private MouseDraw mouseDraw;
    private bool canGoNextScene = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!goNextSceneButton)
        {
            goNextSceneButton = GameObject.Find("GoNextSceneButton").GetComponent<Button>();
        }
        goNextSceneButton.onClick.AddListener(OnGoNextSceneButtonClick);
        goNextSceneButton.interactable = false;

        mouseDraw = GameObject.Find("DrawArea").GetComponent<MouseDraw>();
    }

    // Update is called once per frame
    void Update()
    {
        canGoNextScene = mouseDraw.generated;
        goNextSceneButton.interactable = canGoNextScene;
    }

    void OnGoNextSceneButtonClick()
    {
        Debug.Log("Go Next Scene.");
        SceneManager.sceneLoaded += GameSceneLoaded;
        SceneManager.LoadScene("MoveAlongBezierTest");
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        if (next.name == "MoveAlongBezierTest")
        {
            SceneManager.sceneLoaded -= GameSceneLoaded;
            Debug.Log($"Scene:'{next.name}' loaded with '{mode}' mode");

            GameSceneManager gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
            gameSceneManager.railBezier = mouseDraw.NormalizedDrawnBezier;
            gameSceneManager.InitializeMap();
        }
    }

    private void Reset()
    {
        goNextSceneButton = GameObject.Find("GoNextSceneButton").GetComponent<Button>();
    }
}
