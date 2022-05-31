using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class SmilesParseEngine : MonoBehaviour
{
    public GameObject smilesObjectPrefab;

    private List<string> smiles = new List<string>();
    private List<Graph<SmilesObject>> graphs = new List<Graph<SmilesObject>>();
    private Graph<SmilesObject> graph
    {
        get { return graphs.Count == 0 ? null : graphs[0]; }
    }

    private void Start()
    {
        ReadText();
        Parse(0);
    }

    private void ReadText()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("40smiles");

        using (StringReader sr = new StringReader(textAsset.text))
        {
            while (sr.Peek() >= 0)
            {
                smiles.Add(sr.ReadLine());
            }
        }
    }

    private void Parse(int idx)
    {
        string source = smiles[idx];
        graphs.Add(new Graph<SmilesObject>());
        Debug.Log(source);

        // ★ Step 1. Detect Rings and separate it from smiles string
        Dictionary<int, Ring> rings = new Dictionary<int, Ring>();
        int j = 0;
        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];
            if (Regex.IsMatch(c.ToString(), @"[\d-]"))
            {
                int id = int.Parse(c.ToString());
                if (!rings.ContainsKey(id))
                {
                    j -= 1;

                    Ring ring = new Ring();
                    rings.Add(id, ring);

                    ring.id = id;
                    ring.startAddress = j; // - rings.Count;
                    rings[id] = ring;
                }
                else
                {
                    if (!rings[id].IsValid)
                    {
                        j -= 1;

                        Ring ring = rings[id];
                        ring.endAddress = j; // - rings.Count;
                        rings[id] = ring;
                    }
                }
            }

            j++;
        }

        /*
        foreach (KeyValuePair<int, Ring> pair in rings)
        {
            Debug.Log(pair.Value.startAddress);
            Debug.Log(pair.Value.endAddress);
            Debug.Log(pair.Value.Length);
            Debug.Log(pair.Value.IsValid);
        }
        */

        string molecules = Regex.Replace(source, @"[\d-]", string.Empty);
        // Debug.Log(molecules);


        // ★ Step 2. Build Graph
        for (int i = 0; i < molecules.Length; i++)
        {
            graph.AddNode(new SmilesObject(molecules[i], i));
        }

        for (int i = 0; i < graph.NodesCount - 1; i++)
        {
            graph.AddEdge(i, i + 1, 1);

            if (IsRingEnd(i, rings))
            {
                graph.AddEdge(i, GetRing(i, rings).startAddress, 1);
            }
        }

        // ★ Step 3. Build Structure
        DepthFirstSearch();
    }

    private void DepthFirstSearch()
    {
        Node<SmilesObject> root = graph.GetNode(0);
        Stack<Node<SmilesObject>> stack = new Stack<Node<SmilesObject>>();
        stack.Push(root);
        GameObject rootObject = Instantiate(smilesObjectPrefab);

        root.marked = true;

        while (stack.Count > 0)
        {
            Node<SmilesObject> node = stack.Pop();
            for (int i = 0; i < node.Neighbors.Count; i++)
            {
                var neighbor = node.Neighbors[i];
                if (neighbor.marked == false)
                {
                    neighbor.marked = true;
                    stack.Push(neighbor);
                    Instantiate(smilesObjectPrefab, rootObject.transform.position + (Vector3.right * neighbor.Data.id), Quaternion.identity);
                }

                Debug.Log(node.Data.id + ", " + neighbor.Data.id);
            }
        }
    }

    private bool IsRingEnd(int index, Dictionary<int, Ring> rings)
    {
        KeyValuePair<int, Ring> pair = rings.FirstOrDefault(i => i.Value.endAddress == index);

        if (pair.Value != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Ring GetRing(int index, Dictionary<int, Ring> rings)
    {
        Ring ring = rings.First(i => i.Value.endAddress == index).Value;
        return ring;
    }
}
