using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    private Canvas saveFilesCanvas;

    private void Start()
    {
        saveFilesCanvas = FindSaveFilesCanvas();
        if (saveFilesCanvas != null)
        {
            SaveFilesMenuView.Hide(saveFilesCanvas);
        }

        Button loadGameButton = FindButton("Load_Game_Button");
        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OpenLoadGame);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName == gameSceneName)
        {
            NewGame();
            return;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame()
    {
        SaveSystem.DeleteAll();
        SaveSession.BeginNewGame();
        Time.timeScale = 1;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenLoadGame()
    {
        saveFilesCanvas ??= FindSaveFilesCanvas();
        if (saveFilesCanvas == null)
        {
            Debug.LogError("Could not find the Save_Files canvas in Main_Menu.");
            return;
        }

        SaveSession.ClearSelection();
        SaveFilesMenuView.Configure(
            saveFilesCanvas,
            SaveSession.SelectSlot,
            LoadSelectedSlot,
            null,
            DeleteSelectedSlot,
            CloseLoadGame,
            false);
        SaveFilesMenuView.Show(saveFilesCanvas);
    }

    private void LoadSelectedSlot()
    {
        if (SaveSession.SelectedSlot < 1)
        {
            return;
        }

        SaveGameData data = SaveSystem.Load(SaveSession.SelectedSlot);
        if (data == null)
        {
            return;
        }

        SaveSession.PrepareLoad(data);
        Time.timeScale = 1;
        SceneManager.LoadScene(string.IsNullOrWhiteSpace(data.sceneName) ? gameSceneName : data.sceneName);
    }

    private void DeleteSelectedSlot()
    {
        if (SaveSession.SelectedSlot < 1)
        {
            return;
        }

        SaveSystem.Delete(SaveSession.SelectedSlot);
        SaveSession.ClearSelection();
        SaveFilesMenuView.RefreshSlots(saveFilesCanvas);
    }

    private void CloseLoadGame()
    {
        SaveFilesMenuView.Hide(saveFilesCanvas);
    }

    private Canvas FindSaveFilesCanvas()
    {
        foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (canvas.name == "Save_Files" && canvas.gameObject.scene.isLoaded)
            {
                return canvas;
            }
        }

        return null;
    }

    private Button FindButton(string buttonName)
    {
        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (button.name == buttonName && button.gameObject.scene.isLoaded)
            {
                return button;
            }
        }

        return null;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
