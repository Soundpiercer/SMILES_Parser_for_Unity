using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public Text formulaText;

    private IWindow window;
    private string formula;

    public void Init(string formula, IWindow window)
    {
        this.window = window;

        this.formula = formula;
        formulaText.text = formula;
    }

    public void Show()
    {
        window.OpenViewer(formula);
    }
}
