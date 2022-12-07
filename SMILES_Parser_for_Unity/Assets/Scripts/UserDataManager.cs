using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Api;

public class UserDataManager : MonoBehaviour
{
    #region Singleton
    public static UserDataManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // TODO : should add name, id, kind of stuff
    public List<string> savedFormulaList;
    public string accessToken;
    public string refreshToken;

    public void SetLoginData(LoginResponse response)
    {
        accessToken = response.access;
        refreshToken = response.refresh;
        Debug.Log($"TEST | {response}");
    }
    
    public void AddFormula(string formula)
    {
        string match = savedFormulaList.Find(s => s == formula);
        if (string.IsNullOrEmpty(match))
        {
            savedFormulaList.Add(formula);
            Debug.Log("Successfully saved! : " + formula);
        }
        else
        {
            Debug.LogWarning("Duplicated formula found, can't add!");
        }
    }
}
