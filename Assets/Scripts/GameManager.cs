using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Drag scene objects, not prefabs")]
    public MemorySocket[] sockets;
    public MemoryItem[] cubes;
    public Transform basketSpot;

    [Header("Settings")]
    public float memoriseSeconds = 10f;
    public float rowSpacing = 0.25f;

    void Start() => StartCoroutine(RunRound());

    IEnumerator RunRound()
    {
        SetAnswerKeyAndShowCubes();
        yield return new WaitForSeconds(memoriseSeconds);
        MoveCubesToRow();
    }

    void SetAnswerKeyAndShowCubes()
    {
        var shuffled = cubes.OrderBy(_ => Random.value).ToArray();

        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].correctItemID = shuffled[i].itemID;

            var cube = shuffled[i];
            cube.GetComponent<Rigidbody>().isKinematic = true;
            cube.transform.position = sockets[i].transform.position;
            cube.transform.rotation = sockets[i].transform.rotation;
        }
    }

    void MoveCubesToRow()
    {
        foreach (var s in sockets) s.SetGrabbing(false);

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].GetComponent<Rigidbody>().isKinematic = true;
            cubes[i].transform.position =
                basketSpot.position + new Vector3(i * rowSpacing, 0f, 0f);
        }

        foreach (var s in sockets) s.SetGrabbing(true);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Score();
    }

    void Score()
    {
        int correct = sockets.Count(s => s.IsCorrect());
        Debug.Log($"Score: {correct} / {sockets.Length}");
    }
}