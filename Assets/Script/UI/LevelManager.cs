using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LevelManager : MonoBehaviour
{
    public GameObject levelMenu;
    public GameObject skillSelectionPanel;
    public GameObject startGame;
    public int selectedLevel;

    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image progressBar;
    private float target;
    private void Awake()
    {
        levelMenu.SetActive(true);
        skillSelectionPanel.SetActive(false);
        LoadSavedLevel(); 
    }

    public void SelectLevel(int lvlId)
    {
        Debug.Log("Selected level: " + lvlId);
        selectedLevel = lvlId;
        SaveSelectedLevel();
        levelMenu.SetActive(false);
        skillSelectionPanel.SetActive(true);
    }

    public async void LoadLevel(int lvlId)
    {

        skillSelectionPanel.SetActive(false);
        string lvlName = "Level" + lvlId;

        progressBar.fillAmount = 0;
        var scene = SceneManager.LoadSceneAsync(lvlName);
        scene.allowSceneActivation = false;
        loadingCanvas.SetActive(true);
        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(2000);
        scene.allowSceneActivation = true;
        loadingCanvas.SetActive(false);
    }
    private void SaveSelectedLevel()
    {
        PlayerPrefs.SetInt("SelectedLevel", selectedLevel);
        PlayerPrefs.Save();
    }

    public void LoadSavedLevel()
    {
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
    }

    public void updateProgressBar()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
    }


    
}
