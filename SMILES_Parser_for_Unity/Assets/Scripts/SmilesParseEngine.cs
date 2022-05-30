using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Atom
{
    Default = 0,
    H = 1,
    C = 6,
    N = 7,
    O = 8,
    F = 9,
    //Na = 11,
    //Cl = 17,
    K = 19,
    //Ca = 20,
}

public class SmilesObject
{
    public Atom atom;

    public SmilesObject()
    {
        atom = Atom.Default;
    }

    public SmilesObject(Atom atom)
    {
        this.atom = atom;
    }

    public SmilesObject(string value)
    {
        if (Enum.TryParse(value, out Atom result))
        {
            atom = result;
        }
    }

    public SmilesObject(int value)
    {
        if (Enum.TryParse(value.ToString(), out Atom result))
        {
            atom = result;
        }
    }
}

public class Node<T>
{
    private List<Node<T>> neighbors;
    private List<float> weights;

    public T Data { get; set; }

    public Node()
    {
    }

    public Node(T value)
    {
        Data = value;
    }

    public List<Node<T>> Neighbors
    {
        get
        {
            neighbors = neighbors ?? new List<Node<T>>();
            return neighbors;
        }
    }

    public List<float> Weights
    {
        get
        {
            weights = weights ?? new List<float>();
            return weights;
        }
    }
}

public struct Ring
{
    public int id;

    public int startAddress;
    public int endAddress;
    public int Length
    {
        get { return endAddress - startAddress + 1; }
    }

    public bool IsValid
    {
        get { return endAddress != 0 && Length > 2; }
    }
}

// Graph 클래스
public class Graph<T>
{
    private List<Node<T>> nodeList;

    public Graph()
    {
        nodeList = new List<Node<T>>();
    }

    public Node<T> AddNode(T data)
    {
        Node<T> n = new Node<T>(data);
        nodeList.Add(n);
        return n;
    }

    public Node<T> AddNode(Node<T> node)
    {
        nodeList.Add(node);
        return node;
    }

    public void AddEdge(Node<T> from, Node<T> to, float weight = 0, bool oneway = true)
    {
        from.Neighbors.Add(to);
        from.Weights.Add(weight);

        if (!oneway)
        {
            to.Neighbors.Add(from);
            to.Weights.Add(weight);
        }
    }

    public void DebugPrintLinks()
    {
        foreach (Node<T> graphNode in nodeList)
        {
            foreach (var n in graphNode.Neighbors)
            {
                string s = graphNode.Data + " - " + n.Data;
                Debug.Log(s);
            }
        }
    }
}

public class SmilesParseEngine : MonoBehaviour
{
    private List<string> smiles = new List<string>();
    private List<Graph<SmilesObject>> molecules = new List<Graph<SmilesObject>>();

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
        Debug.Log(source);

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

        foreach (KeyValuePair<int, Ring> pair in rings)
        {
            Debug.Log(pair.Value.startAddress);
            Debug.Log(pair.Value.endAddress);
            Debug.Log(pair.Value.Length);
            Debug.Log(pair.Value.IsValid);
        }    

        string molecules = Regex.Replace(source, @"[\d-]", string.Empty);
        Debug.Log(molecules);

        /*
        Graph<SmilesObject> molecule = new Graph<SmilesObject>();
        string s = smiles[0];

        for (int i = 0; i < s.Length; i++)
        {
            SmilesObject atom = new SmilesObject(s[i].ToString());
            molecule.AddNode(atom);
        }
        */
    }
}
