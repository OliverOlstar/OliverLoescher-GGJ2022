using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainDeath : MonoBehaviour
{
    [SerializeField] private TrainMovement train = null;
    [SerializeField] private TrainMovePiece trainPiece = null;
    private List<TrainMovePiece> connectedPieces = new List<TrainMovePiece>();

    [SerializeField] private GameObject[] destroyObjects = new GameObject[0];

    private void Start() 
    {
        transform.SetParent(null);
        
        int index = train.pieces.IndexOf(trainPiece);
        train.pieces.RemoveAt(index);

        while (train.pieces.Count - 1 > index) // Don't remove last piece (missing piece)
        {
            connectedPieces.Add(train.pieces[index]);
            train.pieces.RemoveAt(index);
        }
        
        foreach (GameObject go in destroyObjects)
        {
            Destroy(go);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Train")
        {
            Reconnect();
            Destroy(gameObject);
        }
    }

    private void Reconnect()
    {
        TrainMovePiece last = train.pieces[train.pieces.Count - 1];
        train.pieces.Remove(last);

        train.pieces.AddRange(connectedPieces);
        connectedPieces.Clear();

        train.pieces.Add(last);
    }
}
