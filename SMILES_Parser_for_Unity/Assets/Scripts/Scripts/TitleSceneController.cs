using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class TitleSceneController : MonoBehaviour
{
    public GameObject showFormulaButton;
    public RectTransform panel;
    public GameObject loadingPanel;

    public static List<string> smiles = new List<string>();

    private bool useAIServer = false;
    private const string AI_SERVER_HOST = "";

    public void Generate()
    {
        smiles = new List<string>();

        StartCoroutine(GenerateEnumerator());
    }

    private IEnumerator GenerateEnumerator()
    {
        string rawText = string.Empty;
        List<string> rawSmiles = new List<string>();

        loadingPanel.SetActive(true);

        // fetch raw Text Data
        if (useAIServer)
        {
            WWWForm form = new WWWForm();

            UnityWebRequest request = UnityWebRequest.Post(AI_SERVER_HOST, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                rawText = request.downloadHandler.text;
            }
            else
            {
                Debug.LogError(request.error);
            }
        }
        else
        {
            TextAsset textAsset = Resources.Load<TextAsset>("40smiles");
            rawText = textAsset.text;
        }

        using (StringReader sr = new StringReader(rawText))
        {
            while (sr.Peek() >= 0)
            {
                rawSmiles.Add(sr.ReadLine());
            }
        }

        // randomly pick 8 smiles string
        List<int> randomIndexes = new List<int>();
        while (randomIndexes.Count < 8)
        {
            int index = Random.Range(0, rawSmiles.Count - 1);

            if (randomIndexes.Contains(index)) continue;
            randomIndexes.Add(index);
        }

        foreach (int i in randomIndexes)
        {
            string filtered = Regex.Replace(rawSmiles[i], @"[()23#\[\]]", string.Empty).ToUpper();
            filtered = Regex.Replace(filtered, @"[=]", "D").ToUpper();
            smiles.Add(filtered);
        }

        // Instantiate Buttons
        for (int i = 0; i < smiles.Count; i++)
        {
            FormulaButtonBehaviour button = Instantiate(showFormulaButton, panel).GetComponent<FormulaButtonBehaviour>();
            
            button.Init(smiles[i]);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -60 * (i + 1), 0);
        }

        yield return new WaitForSeconds(0.3f);
        loadingPanel.SetActive(false);
    }
}
