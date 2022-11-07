using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    public GameObject showFormulaButton;
    public RectTransform panel;
    public GameObject loadingPanel;

    public static List<string> smiles;

    private bool useAIServer = false;
    private const string AI_SERVER_HOST = "";

    public void Generate()
    {
        GenerateTask().Forget();
    }

    private async UniTaskVoid GenerateTask()
    {
        loadingPanel.SetActive(true);
        smiles = await APIClient.GetTargetGenerateFormulas(AI_SERVER_HOST);
        
        for (var i = 0; i < smiles.Count; i++)
        {
            var button = Instantiate(showFormulaButton, panel).GetComponent<FormulaButtonBehaviour>();
            button.Init(smiles[i]);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (i + 1));
        }

        await UniTask.Delay(300);
        loadingPanel.SetActive(false);
    }
}
