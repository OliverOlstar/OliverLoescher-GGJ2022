using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMissingTrigger : MonoBehaviour
{
    [HideInInspector] public TrainTrack.TrackPoint point = null;
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            PlayerCarry player = other.GetComponent<PlayerCarry>();

            if (player == null)
                return;

            if (player.GetItem() == null)
                return;

            if (player.GetItem().type != PlayerCarry.ItemType.Log)
                return;

            GameObject item = player.GetItem().gameObject;
            player.StopCarrying(false);
            Destroy(item);

            if (point.backPoint != null && point.backPoint.model != null && !point.backPoint.isMissing)
                point.backPoint.model.UpdateTrackBack(false);
                
            if (point.forwardPoint != null && point.forwardPoint.model != null && !point.forwardPoint.isMissing)
                point.forwardPoint.model.UpdateTrackForward(false, false);

            point.model.UpdateTrackStraight();
            point.isMissing = false;
        }
    }
}
