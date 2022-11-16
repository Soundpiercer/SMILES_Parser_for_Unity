using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FormulaButtonBehaviour : MonoBehaviour
{
    public string formula;
    public Text formulaText;

    public void Init(string formula)
    {
        this.formula = formula;
        formulaText.text = formula;
    }

    public void Show()
    {
        SmilesParseEngine.formula = formula;
        SceneManager.LoadScene((int)Scene.ViewerScene);
    }
}
