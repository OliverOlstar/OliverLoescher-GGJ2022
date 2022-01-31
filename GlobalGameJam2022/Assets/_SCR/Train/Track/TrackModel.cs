using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackModel : MonoBehaviour
{
    private GameObject currModel = null;
    [SerializeField] private GameObject forwardPrefab = null;
    [SerializeField] private GameObject backPrefab = null;
    [SerializeField] private GameObject midPrefab = null;
    [SerializeField] private GameObject defaultPrefab = null;
    [SerializeField] private GameObject missingPrefab = null;

    private bool forwardMissing = false;
    private bool backMissing = false;

    public void UpdateTrackForward(bool pMissing, bool pSkipInstantiate)
    {
        forwardMissing = pMissing;

        if (!pSkipInstantiate)
            UpdateModel();
    }

    public void UpdateTrackBack(bool pMissing)
    {
        backMissing = pMissing;

        UpdateModel();
    }

    public void UpdateTrackStraight()
    {
        forwardMissing = false;
        backMissing = false;

        InstantiateModel(defaultPrefab);
    }

    public void UpdateModel()
    {
        if (backMissing)
        {
            if (forwardMissing)
            {
                InstantiateModel(midPrefab);
            }
            else
            {
                InstantiateModel(backPrefab);
            }
        }
        else
        {
            if (forwardMissing)
            {
                InstantiateModel(forwardPrefab);
            }
            else
            {
                InstantiateModel(defaultPrefab);
            }
        }
    }

    public void SetMissingTrack(TrainTrack.TrackPoint pPoint)
    {
        InstantiateModel(missingPrefab);
        pPoint.isMissing = true;
        currModel.GetComponentInChildren<TrackMissingTrigger>().point = pPoint;
    }

    public void InstantiateModel(GameObject pPrefab)
    {
        if (currModel != null)
        {
            Destroy(currModel);
        }

        currModel = Instantiate(pPrefab, transform, false);
        currModel.transform.localPosition = Vector3.zero;
        currModel.transform.localRotation = Quaternion.identity;
    }
}
