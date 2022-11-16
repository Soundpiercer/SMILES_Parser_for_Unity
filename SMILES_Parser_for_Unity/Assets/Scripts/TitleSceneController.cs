using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    public GameObject showFormulaButton;
    public RectTransform panel;
    public GameObject loadingPanel;

    [Header("Menu Frame Area")]
    public List<GameObject> menuFrameButtons;


    public static List<string> smiles;

    #region CONSTANT
    private const string AI_SERVER_HOST = "";

    private readonly Color32 BUTTON_SELECTED = new Color32(0xFF, 0xF3, 0xC2, 0xFF);
    private readonly Color32 BUTTON_NORMAL = Color.white;
    #endregion

    private void Start()
    {
        SetMenuFrameButton(0);
    }

    // Toggle Menu Frame Area Buttons.
    public void SetMenuFrameButton(int mode)
    {
        for (int i = 0; i < menuFrameButtons.Count; i++)
        {
            if (i == mode)
            {
                menuFrameButtons[i].GetComponent<Button>().interactable = false;
                menuFrameButtons[i].GetComponent<Image>().color = BUTTON_SELECTED;
            }
            else
            {
                menuFrameButtons[i].GetComponent<Button>().interactable = true;
                menuFrameButtons[i].GetComponent<Image>().color = BUTTON_NORMAL;
            }
        }
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
            var button = Instantiate(showFormulaButton, panel).GetComponent<FormulaButtonBehaviour>();
            button.Init(smiles[i]);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60 * (i + 1));
        }

        await UniTask.Delay(300);
        loadingPanel.SetActive(false);
    }
}
