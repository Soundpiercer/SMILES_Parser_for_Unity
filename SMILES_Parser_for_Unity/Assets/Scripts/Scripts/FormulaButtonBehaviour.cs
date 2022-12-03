using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public string formula;
    public Text formulaText;

    public void Init(string formula)
    {
        this.formula = formula;
        string filter = Regex.Replace(formula, @"[D]", "=").ToUpper();
        formulaText.text = filter;
    }

    public void Show()
    {
        SmilesParseEngine.formula = formula;
        SceneManager.LoadScene(1);
    }
}
