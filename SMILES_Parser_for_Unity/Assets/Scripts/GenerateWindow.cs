using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GenerateWindow : MonoBehaviour
{
    public GameObject pregenerateUI;
    public GameObject viewerUI;

    public GameObject showFormulaButton;
    public RectTransform viewport;
    public GameObject loadingPanel;

    public static List<string> smiles;

    public GameObject smilesViewer;
    private GameObject activeSmilesViewer;

    #region CONSTANT
    private const string AI_SERVER_HOST = "";
    #endregion

    public void Init()
    {
        pregenerateUI.SetActive(true);
        viewerUI.SetActive(false);
    }

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
            var button = Instantiate(showFormulaButton, viewport).GetComponent<FormulaButtonBehaviour>();
            button.Init(smiles[i], this);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (i + 1));
        }

        await UniTask.Delay(300);
        loadingPanel.SetActive(false);
    }

    public void OpenViewer()
    {
        activeSmilesViewer = Instantiate(smilesViewer);

        pregenerateUI.SetActive(false);
        viewerUI.SetActive(true);
    }

    public void CloseViewer()
    {
        if (activeSmilesViewer != null)
            Destroy(activeSmilesViewer);

        Init();
    }
}
