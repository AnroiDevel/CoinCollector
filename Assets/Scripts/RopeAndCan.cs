using System.Collections.Generic;
using UnityEngine;

namespace CoinCollector
{
    public class RopeAndCan : MonoBehaviour
    {
        [SerializeField] private GameObject _platform;
        [SerializeField] private GameObject _can;
        [SerializeField] private int _ropeSegments = 10;
        [SerializeField] private float _segmentLength = 0.5f;
        [SerializeField] private GameObject _ropeSegmentPrefab;

        private List<GameObject> _ropeSegmentsList = new List<GameObject>();
        private LineRenderer _lineRenderer;

        private void Start()
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = _ropeSegments + 2; // +2 for the platform and can
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use a default sprite shader
            _lineRenderer.startColor = Color.black;
            _lineRenderer.endColor = Color.black;

            CreateRope();
        }

        private void Update()
        {
            UpdateLineRenderer();
        }

        private void CreateRope()
        {
            GameObject previousSegment = _platform;
            for(int i = 0; i < _ropeSegments; i++)
            {
                GameObject newSegment = Instantiate(_ropeSegmentPrefab, previousSegment.transform.position - new Vector3(0, _segmentLength, 0), Quaternion.identity);
                DistanceJoint2D joint = newSegment.GetComponent<DistanceJoint2D>();
                joint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
                joint.autoConfigureDistance = false;
                joint.distance = _segmentLength;
                joint.maxDistanceOnly = true;

                _ropeSegmentsList.Add(newSegment);
                previousSegment = newSegment;
            }

            // Connect the last segment to the can
            DistanceJoint2D canJoint = _can.AddComponent<DistanceJoint2D>();
            canJoint.connectedBody = previousSegment.GetComponent<Rigidbody2D>();
            canJoint.autoConfigureDistance = false;
            canJoint.distance = _segmentLength;
            canJoint.maxDistanceOnly = true;
        }

        private void UpdateLineRenderer()
        {
            _lineRenderer.SetPosition(0, _platform.transform.position);

            for(int i = 0; i < _ropeSegments; i++)
            {
                _lineRenderer.SetPosition(i + 1, _ropeSegmentsList[i].transform.position);
            }

            _lineRenderer.SetPosition(_ropeSegments + 1, _can.transform.position);
        }
    }
}
