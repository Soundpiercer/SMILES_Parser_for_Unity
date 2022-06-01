using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    private List<Node<T>> neighbors;
    private List<float> weights;

    public T Data { get; set; }
    public bool marked { get; set; }

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

public class Graph<T>
{
    private List<Node<T>> nodeList;
    public int NodesCount
    {
        get { return nodeList.Count; }
    }

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

    public Node<T> GetNode(int index)
    {
        return nodeList[index];
    }

    public void AddEdge(int fromIndex, int toIndex, float weight = 0, bool oneway = true)
    {
        AddEdge(nodeList[fromIndex], nodeList[toIndex], weight, oneway);
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

public enum Element
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

public class Atom
{
    public Element element;
    public int id;

    public Atom()
    {
        element = Element.Default;
        id = 0;
    }

    public Atom(Element element, int id = 0)
    {
        this.element = element;
        this.id = id;
    }

    public Atom(char value, int id = 0)
    {
        if (Enum.TryParse(value.ToString(), out Element result))
        {
            element = result;
            this.id = id;
        }
    }

    public Atom(string value, int id = 0)
    {
        if (Enum.TryParse(value, out Element result))
        {
            element = result;
            this.id = id;
        }
    }

    public Atom(int value, int id = 0)
    {
        if (Enum.TryParse(value.ToString(), out Element result))
        {
            element = result;
            this.id = id;
        }
    }
}

public class Ring
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