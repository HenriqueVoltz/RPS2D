
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class Manager : MonoBehaviour
{
    public TextMeshProUGUI Result;
    public Image AIChoice;

    public string[] Choices;
    public Sprite Rock, Paper, Scissors;

    public HeartSystem playerHeartSystem;
    public HeartSystem aiHeartSystem;
    
    [Header("Scene navigation")]
    public string winSceneName = "Mapa";     
    public string loseSceneName = "TelaInicial";    
    public float delayBeforeLoad = 1.5f;         

    bool gameOver = false;     

    void Start()
    {
        Debug.Log("Game Started");
    }

    public void Play(string myChoice)
    {
        if (Choices == null || Choices.Length == 0)
        {
            Debug.LogWarning("Choices não configurado.");
            return;
        }

        string randomChoice = Choices[Random.Range(0, Choices.Length)];
        Debug.Log($"Play() -> player:{myChoice} ai:{randomChoice}");

        switch (randomChoice)
        {
            case "Rock":
                AIChoice.sprite = Rock;
                ResolveRound(myChoice, "Rock");
                break;
            case "Paper":
                AIChoice.sprite = Paper;
                ResolveRound(myChoice, "Paper");
                break;
            case "Scissors":
                AIChoice.sprite = Scissors;
                ResolveRound(myChoice, "Scissors");
                break;
            default:
                Debug.LogWarning("Escolha da IA inválida: " + randomChoice);
                break;
        }

        CheckGameOver();
    }

    void ResolveRound(string player, string ai)
    {
        if (player == ai)
        {
            Result.text = "Draw!";
            return;
        }

        
        bool playerWins =
            (player == "Rock" && ai == "Scissors") ||
            (player == "Paper" && ai == "Rock") ||
            (player == "Scissors" && ai == "Paper");

        if (playerWins)
        {
            Result.text = "You Win!";
            if (aiHeartSystem != null) aiHeartSystem.TakeDamage(1);
            else Debug.LogWarning("aiHeartSystem não atribuído.");
        }
        else
        {
            Result.text = "You Lose!";
            if (playerHeartSystem != null) playerHeartSystem.TakeDamage(1);
            else Debug.LogWarning("playerHeartSystem não atribuído.");
        }
    }

    void CheckGameOver()
    {
        if (gameOver) return;

        if (playerHeartSystem != null && playerHeartSystem.vida <= 0)
        {
            gameOver = true;
            Result.text = "Game Over";
            StartCoroutine(LoadSceneAfterDelay(loseSceneName));
        }
        else if (aiHeartSystem != null && aiHeartSystem.vida <= 0)
        {
            gameOver = true;
            Result.text = "You Win";
            StartCoroutine(LoadSceneAfterDelay(winSceneName));
        }
    }

    IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Nome da cena vazio. Não foi possível carregar a cena.");
            yield break;
        }
        SceneManager.LoadScene(sceneName);
    }
}
