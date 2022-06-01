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
    private List<Graph<Atom>> graphs = new List<Graph<Atom>>();
    private Graph<Atom> Graph
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
        graphs.Add(new Graph<Atom>());
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
            Graph.AddNode(new Atom(molecules[i], i));
        }

        for (int i = 0; i < Graph.NodesCount - 1; i++)
        {
            Graph.AddEdge(i, i + 1, 1);

            if (IsRingEnd(i, rings))
            {
                Graph.AddEdge(i, GetRing(i, rings).startAddress, 1);
            }
        }

        // ★ Step 3. Build Structure
        List<AtomBehaviour> buildorder = DepthFirstSearchInstantiate(Graph);
        Ringify(buildorder);
    }

    private List<AtomBehaviour> DepthFirstSearchInstantiate(Graph<Atom> graph)
    {
        List<AtomBehaviour> result = new List<AtomBehaviour>();

        Stack<Node<Atom>> stack = new Stack<Node<Atom>>();
        AtomBehaviour previous;

        // Init Root
        Node<Atom> rootNode = graph.GetNode(0);      
        stack.Push(rootNode);
        
        AtomBehaviour root = Instantiate(smilesObjectPrefab)
            .AddComponent<AtomBehaviour>();
        root.Init(rootNode.Data);
        result.Add(root);
        previous = root;

        rootNode.marked = true;

        // DFS
        while (stack.Count > 0)
        {
            Node<Atom> node = stack.Pop();
            for (int i = 0; i < node.Neighbors.Count; i++)
            {
                var neighbor = node.Neighbors[i];
                if (neighbor.marked == false)
                {
                    neighbor.marked = true;
                    stack.Push(neighbor);

                    AtomBehaviour neighborObject = Instantiate(smilesObjectPrefab, previous.transform.position + (Vector3.right * 2), Quaternion.identity)
                        .AddComponent<AtomBehaviour>();
                    neighborObject.Init(neighbor.Data);
                    result.Add(neighborObject);
                    previous = neighborObject;
                }

                //Debug.Log(node.Data.id + ", " + neighbor.Data.id);
            }
        }

        return result;
    }

    private void Ringify(List<AtomBehaviour> buildorder)
    {
        foreach (AtomBehaviour atom in buildorder)
        {

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
