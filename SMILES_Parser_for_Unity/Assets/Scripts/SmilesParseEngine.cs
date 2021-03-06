using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmilesParseEngine : MonoBehaviour
{
    public GameObject smilesObjectPrefab;
    public GameObject smilesEdgePrefab;
    public Transform smilesObjectRoot;
    public Text formulaText;

    public static string formula = string.Empty;
    private Graph<Atom> Graph = new Graph<Atom>();

    private void Start()
    {
        Parse();
    }

    private void Parse()
    {
        // ★ Step 1. Detect Rings and separate it from smiles string
        Dictionary<int, Ring> rings = new Dictionary<int, Ring>();
        int j = 0;
        for (int i = 0; i < formula.Length; i++)
        {
            char c = formula[i];
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

        string molecules = Regex.Replace(formula, @"[\d-]", string.Empty);
        // Debug.Log(molecules);


        // ★ Step 2. Build Graph
        for (int i = 0; i < molecules.Length; i++)
        {
            Graph.AddNode(new Atom(molecules[i], i));
        }

        for (int i = 0; i < Graph.NodesCount; i++)
        {
            if (i != Graph.NodesCount - 1)
                Graph.AddEdge(i, i + 1, 1);

            if (IsRingEnd(i, rings))
            {
                Graph.AddEdge(i, GetRing(i, rings).startAddress, 1);
            }
        }

        // ★ Step 3. Build Structure
        List<AtomBehaviour> atoms = CreateAtoms(Graph);
        Ring targetRing = new Ring();
        foreach (AtomBehaviour atom in atoms)
        {
            if (IsRingEnd(atom.ID, rings))
            {
                targetRing = GetRing(atom.ID, rings);
            }
        }

        List<AtomBehaviour> ringAtoms = atoms.FindAll(atom => atom.ID >= targetRing.startAddress && atom.ID <= targetRing.endAddress)
            .OrderBy(atom => atom.buildorder)
            .ToList();

        // Relocate atoms affected by rings
        Vector3 directionAfterRingified = Vector3.right;
        if (ringAtoms.Count > 0)
        {
            directionAfterRingified = Ringify(ringAtoms);

            List<AtomBehaviour> afterRingAtoms = atoms.FindAll(atom => atom.ID > targetRing.endAddress)
                .OrderBy(atom => atom.buildorder)
                .ToList();

            if (afterRingAtoms.Count > 0)
                RelocateAtoms(afterRingAtoms, ringAtoms.Last(), directionAfterRingified);
        }

        // Create Edges
        CreateEdges(Graph, atoms);

        // Relocate SMILES GameObject Root
        Vector3 position = Vector3.zero;
        int childCount = 0;
        foreach (Transform t in smilesObjectRoot)
        {
            position += t.transform.position;
            childCount++;
        }

        smilesObjectRoot.position = -position / childCount;

        // ★ Step 4. Finish. Show Formula Text
        formulaText.text = formula;
    }

    #region Step 3
    private List<AtomBehaviour> CreateAtoms(Graph<Atom> graph)
    {
        List<AtomBehaviour> result = new List<AtomBehaviour>();

        Stack<Node<Atom>> stack = new Stack<Node<Atom>>();
        AtomBehaviour previous;
        int buildorder = 0;

        // Init Root
        Node<Atom> rootNode = graph.GetNode(0);      
        stack.Push(rootNode);
        
        AtomBehaviour root = Instantiate(smilesObjectPrefab, smilesObjectRoot)
            .AddComponent<AtomBehaviour>();
        root.Init(rootNode.Data, buildorder);
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

                    AtomBehaviour neighborObject = Instantiate(smilesObjectPrefab, previous.transform.position + (Vector3.right * 2), Quaternion.identity, smilesObjectRoot)
                        .AddComponent<AtomBehaviour>();
                    neighborObject.Init(neighbor.Data, ++buildorder);
                    result.Add(neighborObject);
                    previous = neighborObject;
                }
            }
        }

        return result;
    }

    private Vector3 Ringify(List<AtomBehaviour> ringAtoms)
    {
        Vector3 direction = Vector3.right * 2;

        for (int i = 1; i < ringAtoms.Count; i++)
        {
            ringAtoms[i].transform.position = ringAtoms[i - 1].transform.position + Vector3.right * 2;

            ringAtoms[i].transform.RotateAround(
                ringAtoms[i - 1].transform.position, Vector3.forward, 360f / ringAtoms.Count * i);
        }

        for (int i = 1; i < ringAtoms.Count; i++)
        {
            ringAtoms[i].transform.RotateAround(
                ringAtoms[0].transform.position, Vector3.forward, -360f / ringAtoms.Count * 2);
        }

        float degree = 360f / ringAtoms.Count * (ringAtoms.Count - 4) * Mathf.Deg2Rad;
        direction.x = Mathf.Cos(degree) * 2;
        direction.y = Mathf.Sin(degree) * 2;

        Debug.Log(degree * Mathf.Rad2Deg + " " + direction);
        return direction;
    }

    private void RelocateAtoms(List<AtomBehaviour> afterRingAtoms, AtomBehaviour ringEnd, Vector3 direction)
    {
        for (int i = 0; i < afterRingAtoms.Count; i++)
        {
            afterRingAtoms[i].transform.position = ringEnd.transform.position + direction * (i + 1);
        }
    }

    private void CreateEdges(Graph<Atom> graph, List<AtomBehaviour> atoms)
    {
        foreach (AtomBehaviour atom in atoms)
        {
            Node<Atom> atomNode = graph.GetNode(atom.ID);
            if (atomNode.Neighbors.Count > 0)
            {
                AtomBehaviour from = atoms.Find(a => a.ID == atomNode.Data.id);

                List<Node<Atom>> neighbors = atomNode.Neighbors;
                foreach (Node<Atom> node in neighbors)
                {
                    AtomBehaviour to = atoms.Find(a => a.ID == node.Data.id);

                    EdgeBehaviour edgeObject = Instantiate(smilesEdgePrefab, smilesObjectRoot)
                        .AddComponent<EdgeBehaviour>();
                    edgeObject.Init(from, to);
                }
            }
        }
    }
    #endregion

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

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
