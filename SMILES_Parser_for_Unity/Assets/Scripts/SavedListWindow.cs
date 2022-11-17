using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SavedListWindow : MonoBehaviour, IWindow
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
    public GameObject smilesViewer;
    private SmilesParseEngine activeSmilesViewer;

    private void Start()
    {
        Init();
        ShowSavedFormulaCells();
    }

    private void Init()
    {
        pregenerateUI.SetActive(true);
        viewerUI.SetActive(false);
    }

    private void ShowSavedFormulaCells()
    {
        loadingPanel.SetActive(true);

        for (int i = 0; i < UserDataManager.Instance.savedFormulaList.Count; i++)
        {
            var button = Instantiate(showFormulaButton, viewport).GetComponent<FormulaButtonBehaviour>();
            button.Init(UserDataManager.Instance.savedFormulaList[i], this);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (i + 1));
        }

        loadingPanel.SetActive(false);
    }

    #region Viewer UI
    public void OpenViewer(string formula = "")
    {
        formulaText.text = formula;
        activeSmilesViewer = Instantiate(smilesViewer).GetComponentInChildren<SmilesParseEngine>();
        activeSmilesViewer.formula = formula;

        pregenerateUI.SetActive(false);
        viewerUI.SetActive(true);
    }

    public void ToggleDetailInfoPanel()
    {
        detailInfoPanel.SetActive(!detailInfoPanel.activeSelf);
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
