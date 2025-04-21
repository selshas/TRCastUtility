using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardTableControl : MonoBehaviour
{
    Vector2 cursorPos_screenSpace;

    Ray cursorRay;
    Vector3 cursorRay_dir;
    Rigidbody rigidbody_currentPick = null;

    Vector3 cursorPos_prev;
    Vector3 cursorPos_curr;
    Vector3 cursorPos_delta;

    UnityEngine.UI.GraphicRaycaster grRaycaster_mainCanvas;

    GameObject gameObject_icon_cardTable;

    const float cardFloatingDistance = 0.2f;

    List<UnityEngine.EventSystems.RaycastResult> list_grRaycastResults = new List<UnityEngine.EventSystems.RaycastResult>();

    // Start is called before the first frame update
    void Awake()
    {
        cursorPos_screenSpace = Mouse.current.position.ReadValue();
        cursorRay = Camera.main.ScreenPointToRay(cursorPos_screenSpace);
        cursorRay_dir = cursorRay.direction.normalized;
        float dist_c2p = (transform.position.y - Camera.main.transform.position.y);
        dist_c2p -= Mathf.Sign(dist_c2p) * cardFloatingDistance;
        Vector3 cursorPos_onTable = cursorRay_dir * (dist_c2p / cursorRay_dir.y);

        cursorPos_prev = cursorPos_curr = cursorPos_onTable + Camera.main.transform.position;

        grRaycaster_mainCanvas = GameObject.Find("Canvas").GetComponent<UnityEngine.UI.GraphicRaycaster>();
        gameObject_icon_cardTable = GameObject.Find("Canvas/Root/ToolStatusIcons/Icon_CardTable");
    }

    private void OnEnable()
    {
        gameObject_icon_cardTable.SetActive(true);
    }
    private void OnDisable()
    {
        gameObject_icon_cardTable.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        cursorPos_screenSpace = Mouse.current.position.ReadValue();
        cursorRay = Camera.main.ScreenPointToRay(cursorPos_screenSpace);
        cursorRay_dir = cursorRay.direction.normalized;
        float dist_c2p = (transform.position.y - Camera.main.transform.position.y);
        dist_c2p -= Mathf.Sign(dist_c2p) * cardFloatingDistance;
        Vector3 cursorPos_onTable = cursorRay_dir * (dist_c2p / cursorRay_dir.y);

        cursorPos_prev = cursorPos_curr;
        cursorPos_curr = cursorPos_onTable + Camera.main.transform.position;
        cursorPos_delta = cursorPos_curr - cursorPos_prev;


        if (rigidbody_currentPick == null)
        {
            Rigidbody rigidbody_pointOver = PickCard();
            if (rigidbody_pointOver == null) return;
            
            ApplicationSetup.list_raycastResults.Add(new UnityEngine.EventSystems.RaycastResult());

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (rigidbody_pointOver == null) return;

                rigidbody_currentPick = rigidbody_pointOver;

                rigidbody_currentPick.useGravity = false;
                rigidbody_currentPick.transform.position -= cursorRay_dir * cardFloatingDistance;
                rigidbody_currentPick.linearVelocity = Vector3.zero;
                rigidbody_currentPick.ResetInertiaTensor();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                rigidbody_pointOver.GetComponent<UnitCard>().Flip();
            }
            else if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                if (rigidbody_pointOver.useGravity)
                {
                    rigidbody_pointOver.transform.position -= cursorRay_dir * cardFloatingDistance * 6;
                    rigidbody_pointOver.linearVelocity = Vector3.zero;
                    rigidbody_pointOver.ResetInertiaTensor();
                    rigidbody_pointOver.useGravity = false;
                }
                else
                {
                    rigidbody_pointOver.transform.position += cursorRay_dir * cardFloatingDistance * 5.95f;
                    rigidbody_pointOver.useGravity = true;
                }
            }
        }
        else
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    rigidbody_currentPick.gameObject.SetActive(false);
                    rigidbody_currentPick.linearVelocity = Vector3.zero;
                    rigidbody_currentPick.rotation = Quaternion.Euler(0, 0, 0);
                    rigidbody_currentPick.ResetInertiaTensor();
                    rigidbody_currentPick = null;
                }
                else
                    rigidbody_currentPick.position += cursorPos_delta;
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                rigidbody_currentPick.useGravity = true;
                rigidbody_currentPick = null;
            }
        }
    }


    Rigidbody PickCard()
    {
        UnityEngine.EventSystems.PointerEventData ped = new UnityEngine.EventSystems.PointerEventData(null);
        ped.position = cursorPos_screenSpace;
        
        grRaycaster_mainCanvas.Raycast(ped, list_grRaycastResults);
        if (list_grRaycastResults.Count > 0)
        {
            list_grRaycastResults.Clear();
            return null;
        }
        list_grRaycastResults.Clear();
        
        RaycastHit[] hits = Physics.RaycastAll(cursorRay);

        Rigidbody rigidbody_front = null;
        float rigidbody_front_distance = 0;
        for (int i = 0; i < hits.Length; i++)
        {
            Rigidbody rigidbody = hits[i].rigidbody;
            if (rigidbody == null) continue;

            if (rigidbody_front == null || (rigidbody_front_distance > hits[i].distance))
            {
                rigidbody_front = rigidbody;
                rigidbody_front_distance = hits[i].distance;
            }
        }

        

        UnitCard card = rigidbody_front.GetComponent<UnitCard>();
        if (card == null) return null;

        return rigidbody_front;   
    }
}
