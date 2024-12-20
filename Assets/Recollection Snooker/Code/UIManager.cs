using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }


    #region References

    [SerializeField] GameObject[] heartSprites;

    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject LosePanel;

    [SerializeField] GameObject okButton;

    #endregion

    #region Public Methods

    public void HeartLoss(int currentPlayerHP)
    {
        heartSprites[currentPlayerHP].SetActive(false);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MetaPanel(bool thePlayerWins)
    {
        if (thePlayerWins)
        {
            winPanel.SetActive(true);
        }
        else
        {
            LosePanel.SetActive(true);
        }
    }

    public void ToggleOkButton()
    {
        okButton.SetActive(!okButton.activeSelf);
    }

    #endregion
}
