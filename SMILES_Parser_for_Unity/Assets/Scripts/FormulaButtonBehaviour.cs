using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public Text formulaText;

    private GenerateWindow window;
    private string formula;

    public void Init(string formula, GenerateWindow window)
    {
        this.window = window;

        this.formula = formula;
        formulaText.text = formula;
    }

    public void Show()
    {
        SmilesParseEngine.formula = formula;
        window.OpenViewer(formula);
        //SceneManager.LoadScene((int)Scene.ViewerScene);
    }
}
