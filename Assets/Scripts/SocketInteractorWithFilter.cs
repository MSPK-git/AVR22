using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractorWithFilter : XRSocketInteractor
{
    public XRSocketInteractor interactorSocketCondition = null; // Säule
    public XRGrabInteractable interactableInSocketCondition = null; // Bicycle

    public string filterTag = string.Empty;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (filterTag.Length > 0 && !interactable.transform.CompareTag(filterTag))
        {
            return false;
        }

        return base.CanHover(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        if (interactablesSelected.Count == 0)
        {
            //Debug.Log("CanSelect " + interactable.transform.name + " with tag " + interactable.transform.tag);
        }

        if (base.CanSelect(interactable))
        {
            if (filterTag.Length > 0 && !interactable.transform.CompareTag(filterTag))
            {
                Debug.Log("CanSelect " + interactable.transform.name + " with tag " + interactable.transform.tag + " FALSE");
                return false;
            }

            if (interactorSocketCondition != null && interactableInSocketCondition != null)
            {
                // If we're already monuted to the interactable it's okay.
                if (interactablesSelected.Contains(interactable))
                {
                    return true;
                }

                // Check if interactable is socketed in the interactor
                if (interactorSocketCondition.interactablesSelected.Contains(interactableInSocketCondition))
                {
                    return true;
                }
            } else
            {
                return true;
            }
        }
        return false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        foreach (var interactables in interactablesSelected)
        {
            var sockets = interactables.transform.GetComponentsInChildren<SocketInteractorWithFilter>();
            foreach (var socket in sockets)
            {
                socket.NotifySelect(args.interactableObject);
            }
        }

        base.OnSelectEntered(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        foreach (var interactables in interactablesSelected)
        {
            var sockets = interactables.transform.GetComponentsInChildren<SocketInteractorWithFilter>();
            foreach (var socket in sockets)
            {
                socket.NotifyExit(args.interactableObject);
            }
        }

        base.OnSelectExiting(args);
    }

    protected void NotifySelect(IXRSelectInteractable interactableObject)
    {
        // Interactable added to socket condition, we can now accept bike parts
        if (interactableObject.Equals(interactableInSocketCondition))
        {
            foreach (var interactable in this.interactablesSelected)
            {
                var myInteractable = interactable.transform.GetComponent<OffsetInteractable>();
                if (myInteractable != null)
                {
                    myInteractable.preventHandGrab = false;
                }
            }
        }
    }

    protected void NotifyExit(IXRSelectInteractable interactableObject)
    {
        // Interactable removed from socket condition, no more socketing for us
        if (interactableObject.Equals(interactableInSocketCondition))
        {
            foreach (var interactable in this.interactablesSelected)
            {
                var myInteractable = interactable.transform.GetComponent<OffsetInteractable>();
                if (myInteractable != null)
                {
                    myInteractable.preventHandGrab = true;
                }
            }
        }
    }
}