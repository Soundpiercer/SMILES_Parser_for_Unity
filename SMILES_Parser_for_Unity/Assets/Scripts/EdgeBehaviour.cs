using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBehaviour : MonoBehaviour
{
    public AtomBehaviour from;
    public AtomBehaviour to;

    private readonly Color default_color = new Color32(0x44, 0x44, 0x44, 0xFF);

    public void Init(AtomBehaviour from, AtomBehaviour to)
    {
        this.from = from;
        this.to = to;
        gameObject.name = string.Format("edge {0}, {1}", from.ID, to.ID);

        foreach (Transform t in transform)
        {
            t.GetComponent<MeshRenderer>().material.color = default_color;
        }

        gameObject.transform.position =
            (from.transform.position + to.transform.position) / 2f;
        // gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f); default rotation

        float angle = Vector3.SignedAngle(to.transform.position - from.transform.position, Vector3.right, -Vector3.forward);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f + angle);
    }
}
