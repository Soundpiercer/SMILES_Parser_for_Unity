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
    }
}
