using UnityEngine;
using UnityEngine.SceneManagement; // Importante para cambiar de escenas

public class MainMenuController : MonoBehaviour
{
    // Esta función la conectaremos al botón PLAY
    public void PlayGame()
    {
        // Cambiá "GameScene" por el nombre EXACTO de la escena de tu juego
        SceneManager.LoadScene("PrototipoMain"); 
    }

    // Esta función la conectaremos al botón SETTINGS
    public void OpenSettings()
    {
        // Cambiá "SettingsScene" por el nombre de tu escena de configuración
        SceneManager.LoadScene("SettingsScene");
    }

    /* El botón para el futuro (Cargar Partida)
    public void LoadGame()
    {
        Debug.Log("Próximamente: Cargar Partida");
        // Aca a futuro leeremos el SaveController antes de cambiar de escena
    }*/
    public void BackToMenu()
    {
        // Cambiá "MainMenu" por el nombre EXACTO de tu escena principal
        SceneManager.LoadScene("MainMenuScene");
    }

    // Esta funcion la conectaremos al botón EXIT
    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

        // Este código especial detecta si estás jugando adentro del editor de Unity o en el juego final exportado
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Apaga el botón Play de Unity
        #else
            Application.Quit(); // Cierra el .exe o la app cuando el juego esté compilado
        #endif
    }
}
