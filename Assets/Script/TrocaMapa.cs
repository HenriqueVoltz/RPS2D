using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocaMapa : MonoBehaviour
{
    public void Fase1(){
        SceneManager.LoadScene("Fase1");
    }

    public void Fase2()
    {
        SceneManager.LoadScene("Fase2");
    }
}
