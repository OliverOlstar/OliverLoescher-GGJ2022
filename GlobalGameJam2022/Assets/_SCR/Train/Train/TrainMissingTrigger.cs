using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainMissingTrigger : MonoBehaviour
{
    [SerializeField] private TrainMovement train = null;
    [SerializeField] private TrainMovePiece self = null;
    [SerializeField] private AudioSource source = null;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();

            if (player == null)
                return;

            if (player.GetItem() == null)
                return;

            if (player.GetItem().type != PlayerCarry.ItemType.TrainPiece)
                return;

            GameObject item = player.GetItem().gameObject;
            player.GetItem().Finished();
            player.StopCarrying(false);

            train.pieces.Add(item.GetComponentInParent<TrainMovePiece>());
            train.pieces.Remove(self);
            train.pieces.Add(self); // Move self to end
            Destroy(item);

            source.Play();
        }
    }
}
