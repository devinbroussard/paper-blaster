using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour
{
    //A link to the book's animator
    [SerializeField] private Animator _bookAnimator;
    //The UI canvas
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Vector3 _firstPos;
    [SerializeField] private Vector3 _secondPos;
    private bool _animating;

    //Main menu switch  to play scene
    public void Play()
    {
        _animating = true;
        Time.timeScale = 1;
        _bookAnimator.SetTrigger("Open");
        _canvas.SetActive(false);
        RoutineBehaviour.Instance.StartNewTimedAction(args => SceneManager.LoadScene("play_scene"), 
        TimedActionCountType.SCALEDTIME, 1.25f);
    }

    //Quit the game
    public void Quit()
    {
        Application.Quit();
    }

    //Switch to main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("main_menu_scene");
    }

    //Updating the camera zoom
    private void Update()
    {
        if(_animating == true)
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _secondPos, 0.03f);
    }
}