using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TrainTrack : MonoBehaviour
{
    [System.Serializable]
    public struct TrackSection
    {
        public enum Turn
        {
            Left = -1,
            Right = 1,
            None = 0
        }

        public Vector3 GetTurnVector(Vector3 pDirection)
        {
            Quaternion rot = Quaternion.AngleAxis(90 * (int)turn, Vector3.up);
            return rot * pDirection;
        }

        public Turn turn;
        [Min(1)] public int length;
        [PropertyRange(0, "MinMissing")] public int missingCount;
        private int MinMissing => Mathf.Max(0, length - 3);
    }

    [System.Serializable]
    public class TrackPoint
    {
        public Vector3 position;
        public Vector3 forward;

        public TrackModel model = null;
        public TrackPoint forwardPoint = null;
        public TrackPoint backPoint = null;

        public bool isMissing = false;

        public TrackPoint(Vector3 pPosition, Vector3 pForward, TrackModel pModel)
        {
            position = pPosition;
            forward = pForward;
            model = pModel;
        }
    }

    [SerializeField] private List<TrackSection> track = new List<TrackSection>();
    [SerializeField] private List<TrackPoint> trackPoints = new List<TrackPoint>();

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabStraight = null;
    [SerializeField] private GameObject prefabCorner = null;
    [SerializeField] private GameObject prefabCornerInverse = null;

    private void Start() 
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;
        for (int i = 0; i < track.Count; i++)
        {
            int trackPointsCount = trackPoints.Count;

            GameObject m;
            if (track[i].turn != TrackSection.Turn.None)
            {
                direction = track[i].GetTurnVector(direction);
                GameObject prefab = track[i].turn == TrackSection.Turn.Left ? prefabCorner : prefabCornerInverse;
                m = InstantiateTrack(prefab, position, direction);
            }
            else
            {
                m = InstantiateTrack(prefabStraight, position, direction);
            }
            trackPoints.Add(new TrackPoint(position, direction, m.GetComponent<TrackModel>()));

            int missingCount = track[i].missingCount;
            for (int z = 1; z < track[i].length; z++)
            {
                Vector3 pos = position + direction * (z);
                m = InstantiateTrack(prefabStraight, pos, direction);
                TrackModel tm = m.GetComponent<TrackModel>();
                TrackPoint p = new TrackPoint(pos, direction, tm);
                trackPoints.Add(p);
                
                if (tm == null)
                    continue;

                if (z == 1 || z == track[i].length - 1)
                    continue;

                if (missingCount == 0)
                    continue;

                int possibleMissing = (track[i].length - 2) - z;
                if (possibleMissing > missingCount && Random.value > 0.5f)
                    continue;

                missingCount--;
                tm.SetMissingTrack(p);
            }

            for (int z = 0; z < track[i].length; z++)
            {
                TrackPoint p = trackPoints[z + trackPointsCount];

                if (p.model == null)
                    continue;

                if (z == track[i].length - 1)
                {
                    if (p.isMissing == false)
                        p.model.UpdateTrackBack(trackPoints[z + trackPointsCount - 1].isMissing);

                    p.forwardPoint = trackPoints[z + trackPointsCount - 1];
                }
                else if (z == 0)
                {
                    if (p.isMissing == false)
                        p.model.UpdateTrackForward(trackPoints[z + trackPointsCount + 1].isMissing, false);

                    p.backPoint = trackPoints[z + trackPointsCount + 1];
                }
                else
                {
                    if (p.isMissing == false)
                    {
                        p.model.UpdateTrackForward(trackPoints[z + trackPointsCount + 1].isMissing, true);
                        p.model.UpdateTrackBack(trackPoints[z + trackPointsCount - 1].isMissing);
                    }
                    p.forwardPoint = trackPoints[z + trackPointsCount - 1];
                    p.backPoint = trackPoints[z + trackPointsCount + 1];
                }
            }
            
            position += direction * track[i].length;
        }
    }

    public float GetProgressClamped(float pProgress, out bool pClamped)
    {
        pClamped = false;
        int p = Mathf.FloorToInt(pProgress);
        if (p >= trackPoints.Count - 1 || p < 0.0f)
        {
            return pProgress;
        }

        if (trackPoints[p + 1].isMissing)
        {
            pClamped = true;
            return p;
        }
        
        return pProgress;
    }

    public bool TryGetPoint(float pProgress, out Vector3 pPosition, out Vector3 pForward)
    {
        int p = Mathf.FloorToInt(pProgress);
        if (p >= trackPoints.Count - 1 || p < 0.0f)
        {
            pPosition = Vector3.zero;
            pForward = Vector3.forward;
            return false;
        }

        pPosition = Vector3.Lerp(trackPoints[p].position, trackPoints[p + 1].position, pProgress - p);
        pForward = Vector3.Slerp(trackPoints[p].forward, trackPoints[p + 1].forward, 0);
        return true;
    }

    private GameObject InstantiateTrack(GameObject pPrefab, Vector3 pPosition, Vector3 pDirection)
    {
        GameObject go = Instantiate(pPrefab, transform);
        go.transform.position = pPosition;
        go.transform.forward = pDirection;
        return go;
    }

    private void OnDrawGizmos() 
    {
        Vector3 offset = Vector3.up * 0.5f;
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;
        Gizmos.DrawSphere(position + offset, 0.1f);
        for (int i = 0; i < track.Count; i++)
        {
            Vector3 lastPosition = position;

            direction = track[i].GetTurnVector(direction);
            position += direction * track[i].length;

            float p = (float)track[i].missingCount / (float)track[i].length;
            Gizmos.color = new Color(p, 0.0f, 1 - p, 1.0f);
            Gizmos.DrawSphere(position + offset, 0.1f);
            Gizmos.DrawLine(lastPosition + offset, position + offset);
        }

        if (trackPoints == null || trackPoints.Count < 1)
            return;

        offset = Vector3.up * 0.3f;
        Gizmos.DrawSphere(trackPoints[0].position + offset, 0.1f);
        for (int i = 1; i < trackPoints.Count; i++)
        {
            Gizmos.DrawSphere(trackPoints[i].position + offset, 0.1f);
            Gizmos.DrawLine(trackPoints[i - 1].position + offset, trackPoints[i].position + offset);
        }
    }
}
