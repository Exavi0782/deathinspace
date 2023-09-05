using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject cameraSource;

    private void Start()
    {
        CursorOff();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else Paused();
        }
    }

    public void Resume()
    {
        cameraSource.GetComponent<CameraController>().enabled = true;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        CursorOff();
    }

    public void Paused()
    {
        cameraSource.GetComponent<CameraController>().enabled = false;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        CursorOn();
        Time.timeScale = 0f;

    }

    private void CursorOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CursorOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
