using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomBehaviour : MonoBehaviour
{
    public Atom atom;
    public int ID
    {
        get { return atom.id; }
    }

    public int buildorder;

    public void Init(Atom atom, int buildorder)
    {
        this.atom = atom;
        this.buildorder = buildorder;

        gameObject.name = "atom" + ID;

        // set element color
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        switch (atom.element)
        {
            case Element.O:
                renderer.material.color = Color.yellow;
                break;
            case Element.N:
                renderer.material.color = Color.green;
                break;
            default:
                break;
        }
    }
}
