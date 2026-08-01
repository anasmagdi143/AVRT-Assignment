using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;   // add near the other usings
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GameManager : MonoBehaviour
{
    public MemorySocket[] sockets;    // drag all 6 in
    public MemoryItem[] items;        // drag all 6 in
    public Transform basketPoint;     // empty where cubes drop after memorise
    public float memorizeSeconds = 10f;

    void Start() => StartCoroutine(RunTrial());

    IEnumerator RunTrial()
    {
        Debug.Log("1. Trial started");
        var shuffled = items.OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].correctItemID = shuffled[i].itemID;
            PlaceInSocket(shuffled[i], sockets[i]);
        }
        Debug.Log("2. Setup done, waiting 10s");

        yield return new WaitForSeconds(memorizeSeconds);

        Debug.Log("3. Ejecting now");
        foreach (var s in sockets) EjectFrom(s);
        Debug.Log("4. Eject done");

        // re-enable sockets so the player can place cubes back
        foreach (var s in sockets)
            s.GetComponent<XRSocketInteractor>().enabled = true;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Score();
    }

    void Score()
    {
        Debug.Log("--- Scoring ---");
        int correct = 0;
        foreach (var s in sockets)
        {
            var item = s.CurrentItem();
            string got = item != null ? item.itemID : "EMPTY";
            bool ok = item != null && item.itemID == s.correctItemID;
            Debug.Log($"Socket {s.labelID}: wants {s.correctItemID}, has {got}, {(ok ? "OK" : "no")}");
            if (ok) correct++;
        }
        Debug.Log($"Score: {correct} / {sockets.Length}");
    }

    void PlaceInSocket(MemoryItem item, MemorySocket socket)
    {
        var rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;                        // freeze during memorise
        item.transform.position = socket.transform.position;
        item.transform.rotation = socket.transform.rotation;
    }

    int ejectIndex = 0;
    void EjectFrom(MemorySocket socket)
    {
        var item = items.First(i => i.itemID == socket.correctItemID);

        // release the cube from this socket, then move it
        var sock = socket.GetComponent<XRSocketInteractor>();
        sock.enabled = false;                    // let go of the cube

        item.GetComponent<Rigidbody>().isKinematic = true;
        item.transform.position = basketPoint.position + new Vector3(ejectIndex * 0.25f, 0f, 0f);
        ejectIndex++;
    }
}