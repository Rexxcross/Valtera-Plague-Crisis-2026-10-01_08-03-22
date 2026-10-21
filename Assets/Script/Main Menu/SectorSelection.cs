
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectorSelectionManager : MonoBehaviour
{
    public void SelectSector(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Scn_MainMenu");
    }
}
