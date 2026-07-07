using UnityEngine;

/// <summary>
/// Volitelný pomocný skript pro menu scénu. Není nutný - klidně napoj tlačítka
/// rovnou na GameManager.SelectHumanAndPlay() / SelectMosquitoAndPlay().
/// Tenhle skript se hodí, pokud chceš mít v menu ještě popisky/preview.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public void OnClickPlayAsHuman()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SelectHumanAndPlay();
    }

    public void OnClickPlayAsMosquito()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SelectMosquitoAndPlay();
    }

    public void OnClickQuit()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }
}
