using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Dipanggil saat tombol START ditekan
    public void PlayGame()
    {
        // Masuk ke Scene Level 1
        // Pastikan nama scene kamu nanti "Level1" persis
        SceneManager.LoadScene("Level1");
    }

    // Dipanggil saat tombol EXIT ditekan
    public void QuitGame()
    {
        Debug.Log("Keluar dari Game!");
        Application.Quit();
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}
