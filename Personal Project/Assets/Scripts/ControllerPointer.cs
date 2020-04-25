using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPointer : MonoBehaviour
{

    private Vector3 _startPoint;
    private Vector3 _forward;
    private Vector3 _endPoint;
    public Transform controller;
    public LineRenderer lineRenderer;
    private Ray ray;
    private RaycastHit hit;
    private int layerMask;

    public enum PointerBehaviour
    {
        On,        //pointer always on
        Off,        //pointer always off
        OnWhenHitTarget,  //pointer only activates when hit valid target
    }

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

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

    public GameObject cursorVisual;
    public float maxLength = 10.0f;

    [SerializeField]
    private PointerBehaviour _pointerBehaviour;

    public PointerBehaviour pointerBehaviour()
    {
            
            PointerBehaviour behaviour = _pointerBehaviour;

            if (behaviour == PointerBehaviour.Off || behaviour == PointerBehaviour.OnWhenHitTarget)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.enabled = true;
            }

            return _pointerBehaviour;
        
    }



    /*
    public PointerBehavior pointerBehavior
    {
        set
        {
            _pointerBehavior = value;
            if (pointerBehavior == PointerBehavior.Off || pointerBehavior == PointerBehavior.OnWhenHitTarget)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.enabled = true;
            }
        }
        get
        {
            return _pointerBehavior;
        }
    }
    */

}
