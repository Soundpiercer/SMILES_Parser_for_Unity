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
    private SmilesParseEngine activeSmilesViewer;
    
    private List<string> formulaList;

    private void Init()
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
        formulaList = await APIClient.GetTargetGenerateFormulas(NetworkConfig.AI_SERVER_HOST);

        for (var i = 0; i < formulaList.Count; i++)
        {
            var button = Instantiate(showFormulaButton, viewport).GetComponent<FormulaButtonBehaviour>();
            button.Init(formulaList[i], this);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (i + 1));
        }

        await UniTask.Delay(300);
        loadingPanel.SetActive(false);
    }

    #region Viewer UI
    public void OpenViewer(string formula = "")
    {
        formulaText.text = formula;
        activeSmilesViewer = Instantiate(smilesViewer).GetComponentInChildren<SmilesParseEngine>();
        activeSmilesViewer.formula = formula;
        activeSmilesViewer.Init();

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
        UserDataManager.Instance.AddFormula(activeSmilesViewer.formula);
        saveButton.SetActive(false);
    }

    public void CloseViewer()
    {
        if (activeSmilesViewer != null)
            Destroy(activeSmilesViewer.transform.parent.gameObject);

        formulaText.text = string.Empty;

        Init();
    }

    private void OnDestroy()
    {
        CloseViewer();
    }
    #endregion
}
