using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPointer : MonoBehaviour
{
    public Transform controller;
    public LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("UI");
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(controller.position, controller.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            lineRenderer.SetPosition(0, controller.position);
            lineRenderer.SetPosition(1, hit.transform.position);
        }
        else
        {
            lineRenderer.SetPosition(0, controller.position);
            lineRenderer.SetPosition(1, controller.TransformDirection(Vector3.forward));
        }
    }
}
