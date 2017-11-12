using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class DraggableObject : MonoBehaviour
{

    public bool isRagdoll = false;
    public Transform DragAnchor;
    public Vector3 centerOfMass;
    private Vector3 dragAnchorPosition;
    private HingeJoint joint;
    private Rigidbody rb;

    private Vector3 dragVector;

    private GameObject DragController;

    private InteractableObject interactableObj;


    private float magnitude;

    void Awake()
    {

        DragController = new GameObject("dragController");

        //DragController = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //DragController.transform.localScale = Vector3.one * 0.1f;
        //DragController.name = "dragController";
        //DragController.GetComponent<Collider>().enabled = false;

        DragController.transform.parent = transform;
        joint = DragController.AddComponent<HingeJoint>();
        //joint.spring = 5000;
        //joint.damper = 100;
        joint.axis = Vector3.one;
        rb = DragController.GetComponent<Rigidbody>();
        rb.mass = 5;
        //rb.isKinematic = true;
        rb.useGravity = false;



        if (DragAnchor == null)
        {
            Debug.Log("You must assign a Drag Anchor - probably it's head");
        }
        else
        {
            dragAnchorPosition = transform.TransformPoint(DragAnchor.position);

            DragController.transform.position = dragAnchorPosition;
        }

        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        if (!isRagdoll)
        {

            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        }


        interactableObj = GetComponent<InteractableObject>();
    }

    // Use this for initialization
    void Start()
    {
        interactableObj.OnDragStart.AddListener(DragStart);
        interactableObj.OnDrag.AddListener(Drag);
        interactableObj.OnDragEnd.AddListener(DragEnd);

    }

    // Update is called once per frame
    void Update()
    {

        //    dragAnchorPosition = DragAnchor.position;

        //joint.anchor = dragAnchorPosition;

    }

    private void LateUpdate()
    {
        //putting drag control in here should be more stable?
        if (interactableObj.isDragging)
        {
            //draw a ray i guess
            var ray = Camera.main.ScreenPointToRay(interactableObj.lastPos);
            if (isRagdoll)
            {
                dragVector = ray.GetPoint(1.2f) - DragController.transform.position;

                dragVector = Vector3.ClampMagnitude(dragVector * 10, 2.5f);
                magnitude = dragVector.magnitude;
                DragController.GetComponent<Rigidbody>().velocity = dragVector;
            }
            else
            {
                DragController.transform.position = ray.GetPoint(1.2f);
            }


        }
        else
        {
            joint.connectedBody = null;
        }
    }

    void DragStart()
    {
        DragController.transform.position = DragAnchor.position;
        if (isRagdoll)
        {
            joint.connectedBody = DragAnchor.GetComponent<Rigidbody>();
        }
        else
        {
            joint.connectedBody = GetComponent<Rigidbody>();
        }

        //set the connected anchor to be the difference between the root and the drag anchor
        dragAnchorPosition = transform.TransformPoint(DragAnchor.position);
        //joint.connectedAnchor = dragAnchorPosition;
    }

    void Drag()
    {

    }

    void DragEnd()
    {
        joint.connectedBody = null;
        DragController.transform.position = DragAnchor.position;
    }

    public void DragTo(Vector3 position)
    {
        Debug.Log("is drag to being called?");
        DragController.transform.position = DragAnchor.position;
        if (isRagdoll)
        {
            joint.connectedBody = DragAnchor.GetComponent<Rigidbody>();
        }
        else
        {
            joint.connectedBody = GetComponent<Rigidbody>();
        }
        //set the connected anchor to be the difference between the root and the drag anchor
        dragAnchorPosition = transform.TransformPoint(DragAnchor.position);

        dragVector = position - DragController.transform.position;
        GetComponent<Rigidbody>().AddForce(dragVector * 50);

    }


}
