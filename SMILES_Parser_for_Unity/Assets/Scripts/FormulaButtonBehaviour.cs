using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public Text formulaText;

    private IWindow window;
    private string formula;

    public void Init(string formula, IWindow window)
    {
        this.window = window;

        this.formula = formula;
		
        string filtered = Regex.Replace(formula, @"[D]", "=").ToUpper();
        filtered = Regex.Replace(filtered, @"[T]", "#").ToUpper();
        formulaText.text = filtered;
    }

    public void Show()
    {
        window.OpenViewer(formula);
    }
}
