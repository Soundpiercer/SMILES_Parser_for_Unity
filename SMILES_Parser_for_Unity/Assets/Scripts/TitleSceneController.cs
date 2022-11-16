using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    public RectTransform viewport;

    [Header("Menu Frame Area")]
    public List<GameObject> menuFrameButtons;

    [Header("Window")]
    public List<GameObject> windows;

    #region CONSTANT
    private readonly Color32 BUTTON_SELECTED = new Color32(0xFF, 0xF3, 0xC2, 0xFF);
    private readonly Color32 BUTTON_NORMAL = Color.white;
    #endregion

    // Toggle Menu Frame Area Buttons.
    public void SetMenuFrameButton(int mode)
    {
        // flush all child windows attached in viewport.
        foreach (RectTransform t in viewport)
        {
            Destroy(t.gameObject);
        }

        // toggle buttons and open windows.
        for (int i = 0; i < menuFrameButtons.Count; i++)
        {
            if (i == mode)
            {
                menuFrameButtons[i].GetComponent<Button>().interactable = false;
                menuFrameButtons[i].GetComponent<Image>().color = BUTTON_SELECTED;

                Instantiate(windows[i], viewport);
            }
            else
            {
                menuFrameButtons[i].GetComponent<Button>().interactable = true;
                menuFrameButtons[i].GetComponent<Image>().color = BUTTON_NORMAL;
            }
        }
    }
}
