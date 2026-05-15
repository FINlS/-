using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;

    public void ActivatePortal()
    {
        Debug.Log("Загрузка новой сцены...");
        SceneManager.LoadScene(sceneToLoad);
    }
}