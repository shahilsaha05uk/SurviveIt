using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class SceneManagement : MonoBehaviour
{
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject[] panels;

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string volume = "BackgroundVolumeParam";
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI valueTxt;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(SliderValue);
        masterSlider.value = PlayerPrefs.GetFloat(volume, masterSlider.value);

    }
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volume, masterSlider.value);

    }
    private void SliderValue(float value)
    {
        mixer.SetFloat(volume, value);
        int normValue = Mathf.FloorToInt(Mathf.InverseLerp(masterSlider.minValue, masterSlider.maxValue, value)*100);
        valueTxt.text = normValue.ToString();
    }

    public void SceneLoader(string sceneName, LoadSceneMode loadMode)
    {
        //Scene s = SceneManager.GetActiveScene();
        //SceneManager.UnloadSceneAsync(s);
        SceneManager.LoadScene(sceneName, loadMode);

    }

    public void OnNewGameButtonClick()
    {
        SceneLoader("Game", LoadSceneMode.Single);
    }    
    public void OnOptionsButtonClick()
    {
        SceneLoader("Options", LoadSceneMode.Single);
    }
    public void OnStatsButtonClick()
    {
        LoadPanel("Stats");

    }
    public void OnBackButtonClick()
    {
        SceneLoader("MainMenu", LoadSceneMode.Single);
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    void LoadPanel(string panelName)
    {
        for(int i=0; i< panels.Length;i++)
        {
            if(panels[i].name == panelName)
            {
                panels[i].SetActive(true);
            }
            else
            {
                panels[i].SetActive(false);
            }
        }
    }
}
