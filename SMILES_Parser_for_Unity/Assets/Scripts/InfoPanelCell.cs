using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelCell : MonoBehaviour
{
    public List<GameObject> molecules = new List<GameObject>();
    public Text qedText;

    public void Init(int index)
    {
        float flipf = Random.Range(0f, 2f);
        bool flip = (int)flipf % 2 == 1;

        if (flip)
        {
            molecules[index].transform.Find("rotation").localRotation = Quaternion.Euler(0, 0, 180);
            molecules[index].transform.Find("rotation").localPosition += new Vector3(0, 40);
        }
        molecules[index].SetActive(true);

        qedText.text = Random.Range(0.94f, 0.97f).ToString();
    }
}
