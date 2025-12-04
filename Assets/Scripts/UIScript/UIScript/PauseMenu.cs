using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuButton; 

    public void Pause()
    {
        Debug.Log("PAUSE CALLED");
        pauseMenu.SetActive(true);
        pauseMenuButton.SetActive(false);
        Time.timeScale = 0;
    }


    public void Continue()
    {
        pauseMenu.SetActive(false);
        pauseMenuButton.SetActive(true);
        Time.timeScale = 1;
    }

    public void Option()
    {
        //Option UI SetActive(true)
    }

    public void Quit()
    {
        //Change scene to Main Menu
    }
}
