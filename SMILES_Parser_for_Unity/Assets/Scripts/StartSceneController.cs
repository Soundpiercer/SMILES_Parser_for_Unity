using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    public void TouchToStart()
    {
        SceneManager.LoadScene((int)Scene.TitleScene);
    }
}
