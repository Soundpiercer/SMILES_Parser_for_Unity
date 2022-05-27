using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleSceneController : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject loadingPanel;
    public Text loadingText;
    public GameObject infoPanelPrefab;

    public void GenerateButton()
    {
        StartCoroutine(GenerateEnumerator());
    }

    private IEnumerator GenerateEnumerator()
    {
        // Purge Infopanel
        loadingText.text = "fetching pretrained data from server...";
        foreach (Transform t in infoPanel.transform)
        {
            Destroy(t.gameObject);
        }

        loadingPanel.SetActive(true);

        yield return new WaitForSeconds(Random.Range(2.5f, 3f));

        loadingText.text = "parsing...";

        yield return new WaitForSeconds(Random.Range(1f, 1.5f));

        loadingText.text = "locating...";

        yield return new WaitForSeconds(Random.Range(1f, 1.5f));

        List<int> indexes = new List<int>(){0, 1, 2, 3, 4, 5, 6, 7, 8};
        Shuffle(indexes);

        // Generate
        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));

            InfoPanelCell cell = Instantiate(infoPanelPrefab, infoPanel.transform).GetComponent<InfoPanelCell>();
            cell.Init(indexes[i]);
        }

        loadingPanel.SetActive(false);
    }

    private System.Random random = new System.Random();

    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}