using UnityEngine;
using UnityEngine.UI;

public class HeartSystem : MonoBehaviour
{
    public int vida;
    public int vidaMaxima;

    public Image[] hearts;
    public Sprite Cheio;
    public Sprite Vazio;

    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        vida = Mathf.Clamp(vida - amount, 0, vidaMaxima);
        UpdateUI();
        if (vida == 0) Debug.Log(gameObject.name + " morreu");
    }

    public void Heal(int amount)
    {
        vida = Mathf.Clamp(vida + amount, 0, vidaMaxima);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hearts == null || hearts.Length == 0) return;

        int max = Mathf.Clamp(vidaMaxima, 0, hearts.Length);
        int current = Mathf.Clamp(vida, 0, max);

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = (i < max);
            hearts[i].sprite = (i < current) ? Cheio : Vazio;
        }
    }

    void OnValidate()
    {
        UpdateUI();
    }
}