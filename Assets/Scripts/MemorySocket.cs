using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class MemorySocket : MonoBehaviour
{
    public string labelID;   // A3, B5, etc.
    [HideInInspector] public string correctItemID;

    XRSocketInteractor socket;

    void Awake() => socket = GetComponent<XRSocketInteractor>();

    public MemoryItem HeldItem()
    {
        if (!socket.hasSelection) return null;
        var held = socket.GetOldestInteractableSelected();
        return (held as MonoBehaviour)?.GetComponent<MemoryItem>();
    }

    public bool IsCorrect()
    {
        var item = HeldItem();
        return item != null && item.itemID == correctItemID;
    }

    // off before moving cubes (or the socket drags them back), on for placement
    public void SetGrabbing(bool on) => socket.enabled = on;
}