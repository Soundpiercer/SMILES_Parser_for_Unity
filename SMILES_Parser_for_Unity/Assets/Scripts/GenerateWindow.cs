using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GenerateWindow : MonoBehaviour, IWindow
{
    public GameObject pregenerateUI;
    public GameObject viewerUI;

    [Header("Pre-Generate UI")]
    public RectTransform viewport;
    public GameObject loadingPanel;
    public GameObject showFormulaButton;

    [Header("Viewer UI")]
    public Text formulaText;
    public GameObject detailInfoPanel;
    public GameObject saveButton;
    public GameObject smilesViewer;
    private GameObject activeSmilesViewer;
    
    public static List<string> smiles;

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

    public void OpenViewer(string formula = "")
    {
        formulaText.text = formula;
        activeSmilesViewer = Instantiate(smilesViewer);

        pregenerateUI.SetActive(false);
        viewerUI.SetActive(true);
        saveButton.SetActive(true);
    }

    public void ToggleDetailInfoPanel()
    {
        detailInfoPanel.SetActive(!detailInfoPanel.activeSelf);
    }

    public void Save()
    {
        UserDataManager.Instance.AddFormula(SmilesParseEngine.formula);
        saveButton.SetActive(false);
    }

    public void CloseViewer()
    {
        if (activeSmilesViewer != null)
            Destroy(activeSmilesViewer);

        formulaText.text = string.Empty;

        Init();
    }

    private void OnDestroy()
    {
        CloseViewer();
    }
}
