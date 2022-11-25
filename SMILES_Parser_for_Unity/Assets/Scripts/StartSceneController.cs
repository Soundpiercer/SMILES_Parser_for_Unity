using Api;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    public InputField id;
    public InputField password;
    
    public void Login()
    {
        LoginTask().Forget();
    }

    public void Register()
    {
        RegisterTask().Forget();
    }

    private async UniTaskVoid RegisterTask()
    {
        // TODO : Register UI
        var req = new RegisterRequest(
            "test", 
            "test", 
            "test", 
            "test@test.com",
            "test_first_name",
            "test_last_name"
            );
        var resp = await PrivateApiClient.RegisterRequest(req);
        LoadStartScene();
    }
    
    private async UniTaskVoid LoginTask()
    {
        var resp = await PrivateApiClient.LoginRequest(new LoginRequest(id.text, password.text));
        UserDataManager.Instance.SetLoginData(resp);
        LoadTitleScene();
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene((int)Scene.StartScene);
    }
    
    public void LoadTitleScene()
    {
        SceneManager.LoadScene((int)Scene.TitleScene);
    }
}
