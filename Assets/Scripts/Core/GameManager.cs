using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Trvalý singleton, který si pamatuje, za koho hráč hraje (Člověk / Komár),
/// a stará se o načtení herní scény z menu.
/// Umísti tento skript na prázdný GameObject "GameManager" jen v MENU scéně.
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum CharacterType { Human, Mosquito }

    public static GameManager Instance { get; private set; }

    [Tooltip("Přesný název herní scény v Build Settings.")]
    [SerializeField] private string gameSceneName = "Game";

    public CharacterType SelectedCharacter { get; private set; } = CharacterType.Human;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Zavolej z UI tlačítka "Hrát za člověka"
    public void SelectHumanAndPlay()
    {
        SelectedCharacter = CharacterType.Human;
        SceneManager.LoadScene(gameSceneName);
    }

    // Zavolej z UI tlačítka "Hrát za komára"
    public void SelectMosquitoAndPlay()
    {
        SelectedCharacter = CharacterType.Mosquito;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
