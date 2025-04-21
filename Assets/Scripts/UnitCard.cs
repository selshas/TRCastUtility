using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(MeshFilter))]
public class UnitCard : MonoBehaviour
{
    public Texture2D texture = null;
    bool isOpen
    {
        get => _isOpen;
        set 
        {
            // Trigger Event
            _isOpen = value; 
        }
    }
    bool _isOpen = false;
    bool flippingTriggered = false;

    Rigidbody rigidbody;
    MeshRenderer meshRenderer;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = texture;
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(0, 0.01f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (flippingTriggered && rigidbody.angularVelocity.magnitude == 0)
        {
            if (rigidbody.transform.up.y > 0)
                isOpen = false;
            else
                isOpen = true;

            flippingTriggered = false;

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.ResetInertiaTensor();
        }
    }

    public void Flip()
    {
        rigidbody.useGravity = true;
        rigidbody.AddForce(new Vector3(0, 16, 0), ForceMode.Impulse);
        rigidbody.angularVelocity = new Vector3(0, 0, 20.0f);
        flippingTriggered = true;
    }
}
