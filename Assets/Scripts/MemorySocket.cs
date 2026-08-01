using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MemorySocket : MonoBehaviour
{
    public string labelID;   // A3, B5, etc.
    public string correctItemID;

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    // returns the cube in this socket, or null if empty
    public MemoryItem HeldItem()
    {
        if (socket.hasSelection == false)
        {
            return null;
        }

        var held = socket.GetOldestInteractableSelected();
        var heldObject = held as MonoBehaviour;

        if (heldObject == null)
        {
            return null;
        }

        return heldObject.GetComponent<MemoryItem>();
    }

    public bool IsCorrect()
    {
        MemoryItem item = HeldItem();

        if (item == null)
        {
            return false;
        }

        return item.itemID == correctItemID;
    }

    // off before moving cubes, on for placement
    public void SetGrabbing(bool on)
    {
        socket.enabled = on;
    }
}