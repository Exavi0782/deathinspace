using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.Audio;

public class OptionsChange : MonoBehaviour
{
    [SerializeField]  RenderPipelineAsset[] qualityLevels;
    [Header("Quality dropdown")]
    [SerializeField]  TMP_Dropdown qualityDropdown;
    [Header("resolution dropdown")]
    [SerializeField]  TMP_Dropdown resolutionDropdown;

    [Header("Audio mixer")]
    [SerializeField]  AudioMixer audioMixer;

    Resolution[] resolutions;
   
    void Start()
    {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for(int index = 0;index < resolutions.Length; index++)
        {
            string option = resolutions[index].width + "x" + resolutions[index].height;
            options.Add(option);

            if (resolutions[index].width == Screen.currentResolution.width &&
                resolutions[index].height == Screen.currentResolution.height)
                    currentResolutionIndex = index;

        }
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue(); 
        resolutionDropdown.AddOptions(options);
    }

    public void Changelevel(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullscreen(bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex) 
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
