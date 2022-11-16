using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public string formula;
    public Text formulaText;

    private GenerateWindow window;

    public void Init(string formula, GenerateWindow window)
    {
        this.window = window;

        this.formula = formula;
        formulaText.text = formula;
    }

    public void Show()
    {
        SmilesParseEngine.formula = formula;
        window.OpenViewer();
        //SceneManager.LoadScene((int)Scene.ViewerScene);
    }
}
