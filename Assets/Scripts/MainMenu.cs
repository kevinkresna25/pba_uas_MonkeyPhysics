using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Dipanggil saat tombol START ditekan
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene("TutorialLevel");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void QuitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }
}
