using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchScreenController : MonoBehaviour
{
    HashSet<GameObject> pressedButtons = new HashSet<GameObject>();
    Collider ourCollider;
    bool isTouching;

    // Start is called before the first frame update
    void Start()
    {
        ourCollider = GetComponent<BoxCollider>();
        isTouching = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // "Right Hand Presence Phys (UnityEngine.GameObject)"
        if (collision.gameObject.name.Contains("Hand")) //collision.gameObject.transform.CompareTag("HandController"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
                if (contact.thisCollider == ourCollider)
                {
                    if (!isTouching)
                    {
                        isTouching = true;

                        OnButtonPressed(contact.thisCollider.gameObject);
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        /*foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider == ourCollider)
            {
                isTouching = false;
            }
        }*/
        isTouching = false;
    }

    private void OnButtonPressed(GameObject gameObject)
    {
        var button = gameObject.GetComponent<Button>();

        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}
