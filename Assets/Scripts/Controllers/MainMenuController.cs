using UnityEngine;
using UnityEngine.SceneManagement; // Importante para cambiar de escenas

public class MainMenuController : MonoBehaviour
{
    // Esta función la conectaremos al botón PLAY
    public void PlayGame()
    {
        
        SceneManager.LoadScene("PrototipoMain"); 
    }

    // Esta función la conectaremos al botón SETTINGS
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // Esta funcion la conectaremos al botón EXIT
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Apaga el botón Play de Unity
        #else
            Application.Quit(); 
        #endif
    }
}
