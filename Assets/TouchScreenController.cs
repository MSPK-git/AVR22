using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchScreenController : MonoBehaviour
{
    Collider ourCollider;

    // Start is called before the first frame update
    void Start()
    {
        ourCollider = GetComponent<BoxCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Hand")) //collision.gameObject.transform.CompareTag("HandController"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
                if (contact.thisCollider == ourCollider)
                {
                    DoButtonPress(contact.thisCollider.gameObject);
                }
            }
        }
    }

    private void DoButtonPress(GameObject gameObject)
    {
        var button = gameObject.GetComponent<Button>();

        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}
