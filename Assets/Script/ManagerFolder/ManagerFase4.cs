
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;


public class ManagerFase4 : MonoBehaviour
{
    public TextMeshProUGUI Result;
    public Image AIChoice;

    // Lista de escolhas possíveis usada como fallback
    public string[] Choices;
    public Sprite Rock, Paper, Scissors, Lizard, Spock;

    public HeartSystem playerHeartSystem;
    public HeartSystem aiHeartSystem;
    
    [Header("Scene navigation")]
    public string winSceneName = "Mapa";     
    public string loseSceneName = "TelaInicial";    
    public float delayBeforeLoad = 1.5f;         

    bool gameOver = false;

    [Header("AI Sequence (deterministic)")]
    [Tooltip("Sequência determinística que a IA usará. Preencha 25 entradas para exatamente 25 cartas.")]
    public string[] AISequence = new string[] {
        "Spock", "Rock", "Lizard", "Paper", "Scissors",
        "Rock", "Spock", "Paper", "Lizard", "Scissors",
        "Rock", "Paper", "Spock", "Lizard", "Scissors",
        "Rock", "Paper", "Lizard", "Spock", "Scissors",
        "Rock", "Paper", "Lizard", "Spock", "Scissors"
    };

    [Tooltip("Se verdadeiro, a sequência reinicia após atingir o fim. Se falso, a IA apenas executa até 'maxPlays' e ignora chamadas extras.")]
    public bool loopSequence = false;
    [Tooltip("Número máximo de jogadas que a IA fará (use 25 para seu caso).")]
    public int maxPlays = 25;
    [Tooltip("Quando verdadeiro, a IA não aceitará mais plays após atingir 'maxPlays'.")]
    public bool enforceMaxPlays = true;

    int aiSequenceIndex = 0;
    int aiPlaysCount = 0;

    void Start()
    {
        Debug.Log("Game Started");

        // Validações úteis
        if (AISequence == null || AISequence.Length == 0)
            Debug.LogWarning("AISequence não configurada. A IA usará fallback aleatório a partir de 'Choices'.");
        else if (AISequence.Length < maxPlays)
            Debug.LogWarning($"AISequence tem menos entradas ({AISequence.Length}) do que maxPlays ({maxPlays}). As jogadas adicionais usarão a última entrada ou fallback dependendo das flags.");

        if (AIChoice == null)
            Debug.LogWarning("AIChoice (Image) não está atribuído no Inspector. A IA não poderá mostrar a sprite.");

        if (Rock == null) Debug.LogWarning("Sprite 'Rock' não atribuído no Inspector.");
        if (Paper == null) Debug.LogWarning("Sprite 'Paper' não atribuído no Inspector.");
        if (Scissors == null) Debug.LogWarning("Sprite 'Scissors' não atribuído no Inspector.");
        if (Lizard == null) Debug.LogWarning("Sprite 'Lizard' não atribuído no Inspector.");
        if (Spock == null) Debug.LogWarning("Sprite 'Spock' não atribuído no Inspector.");
    }

    /// <summary>
    /// Chamada quando o jogador faz a jogada. A IA responderá com a próxima carta da sequência determinística até 'maxPlays'.
    /// </summary>
    public void Play(string myChoice)
    {
        if (Choices == null || Choices.Length == 0)
        {
            Debug.LogWarning("Choices não configurado.");
            return;
        }

        // Bloqueia chamadas extras quando for exigido exatamente 'maxPlays'
        if (enforceMaxPlays && aiPlaysCount >= maxPlays)
        {
            Debug.Log("IA já executou o número máximo de jogadas (" + maxPlays + "). Ignorando Play().");
            return;
        }

        string aiChosen = null;

        // Usa a sequência determinística quando fornecida
        if (AISequence != null && AISequence.Length > 0)
        {
            if (aiSequenceIndex >= AISequence.Length)
            {
                if (loopSequence)
                    aiSequenceIndex = 0;
                else
                    aiSequenceIndex = AISequence.Length - 1; // permanece na última entrada
            }

            aiChosen = AISequence[aiSequenceIndex];
            aiSequenceIndex++;
            aiPlaysCount++;
        }
        else
        {
            // Fallback aleatório (só se não houver sequência configurada)
            aiChosen = Choices[Random.Range(0, Choices.Length)];
        }

        Debug.Log($"Play() -> player:{myChoice} ai:{aiChosen} (aiIndex={aiSequenceIndex - 1}, plays={aiPlaysCount})");

        // Normaliza e aplica sprite/resolve
        string aiKey = (aiChosen ?? "").Trim().ToLowerInvariant();
        Sprite chosenSprite = GetSpriteByKey(aiKey);
        string canonicalName = GetCanonicalName(aiKey);

        if (AIChoice != null && chosenSprite != null)
        {
            AIChoice.sprite = chosenSprite;
        }
        else if (AIChoice == null)
        {
            Debug.LogWarning("AIChoice é nulo; não é possível definir a sprite.");
        }
        else
        {
            Debug.LogWarning("Sprite não encontrada para a escolha da IA: '" + aiChosen + "'. Verifique os campos do Inspector.");
        }

        if (!string.IsNullOrEmpty(canonicalName))
            ResolveRound(myChoice, canonicalName);
        else
            Debug.LogWarning("Escolha da IA inválida (não mapeada para um nome canônico): " + aiChosen);

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
            (player == "Scissors" && ai == "Paper") ||
            (player == "Rock" && ai == "Lizard") ||
            (player == "Lizard" && ai == "Spock") ||
            (player == "Spock" && ai == "Scissors") ||
            (player == "Scissors" && ai == "Lizard") ||
            (player == "Lizard" && ai == "Paper") ||
            (player == "Paper" && ai == "Spock") ||
            (player == "Spock" && ai == "Rock");

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

    // Retorna a sprite correspondente para uma chave em lower-case (ex: "rock")
    Sprite GetSpriteByKey(string key)
    {
        switch (key)
        {
            case "rock": return Rock;
            case "paper": return Paper;
            case "scissors": return Scissors;
            case "lizard": return Lizard;
            case "spock": return Spock;
            default: return null;
        }
    }

    // Retorna o nome canônico (capitalizado) usado pelo ResolveRound
    string GetCanonicalName(string key)
    {
        switch (key)
        {
            case "rock": return "Rock";
            case "paper": return "Paper";
            case "scissors": return "Scissors";
            case "lizard": return "Lizard";
            case "spock": return "Spock";
            default: return null;
        }
    }

    [ContextMenu("Reset AI Sequence")]
    public void ResetAISequence()
    {
        aiSequenceIndex = 0;
        aiPlaysCount = 0;
        Debug.Log("AI sequence reset: index=0, plays=0");
    }
}
