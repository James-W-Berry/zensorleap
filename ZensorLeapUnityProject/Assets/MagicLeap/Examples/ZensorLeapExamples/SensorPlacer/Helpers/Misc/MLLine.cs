// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;

namespace MagicKit
{
    ///<summary>
    /// Important settings for a line renderer
    ///</summary>
    [System.Serializable]
    public class MLLine
    {
        //----------- Public Members -----------
        public Material _material;
        public Color _startColor;
        public Color _endColor;
        public int _numSegments = 2;
        public float _startWidth = 1.0f;
        public float _endWidth = 1.0f;

        //----------- Private Members -----------

        private LineRenderer _lineRenderer;

        //----------- Public Methods -----------

        /// <summary>
        /// Return a clone of this object.
        /// </summary>
        public MLLine Clone()
        {
            MLLine newLine = new MLLine();
            newLine._material = _material;
            newLine._startColor = _startColor;
            newLine._endColor = _endColor;
            newLine._numSegments = _numSegments;
            newLine._startWidth = _startWidth;
            newLine._endWidth = _endWidth;
            return newLine;
        }

        /// <summary>
        /// Get a line renderer with the correct settings for this object.
        /// </summary>
        public LineRenderer GetLineRenderer()
        {
            if (_lineRenderer == null)
            {
                GameObject newLine = new GameObject("NewLine");
                _lineRenderer = newLine.AddComponent<LineRenderer>();
                _lineRenderer.material = _material;
                _lineRenderer.startColor = _startColor;
                _lineRenderer.endColor = _endColor;
                _lineRenderer.positionCount = _numSegments;
                _lineRenderer.startWidth = _startWidth;
                _lineRenderer.endWidth = _endWidth;
            }

            return _lineRenderer;
        }

        /// <summary>
        /// Draw a circle using the linerenderer for this object.
        /// </summary>
        public void DrawCircle(Vector3 pos, Vector3 right, Vector3 up, float radius)
        {
            if (_lineRenderer == null)
            {
                GetLineRenderer();
            }

            _lineRenderer.transform.position = pos;

            float angleStep = 360.0f / (_lineRenderer.positionCount - 1); // delta angle between each point
            for (int i = 0; i < _lineRenderer.positionCount; i++) // for all points
            {
                float radians = angleStep * i * Mathf.Deg2Rad;
                float x = Mathf.Cos(radians) * radius; // x pos on circle
                float y = Mathf.Sin(radians) * radius; // y pos on circle

                Vector3 pointPos = pos + right * x + up * y;
                _lineRenderer.SetPosition(i, pointPos);

            }
        }

        /// <summary>
        /// Calls LineRenderer.SetPosition().
        /// </summary>
        public void SetPosition(int index, Vector3 pos)
        {
            if (_lineRenderer == null)
            {
                GetLineRenderer();
            }

            _lineRenderer.SetPosition(index, pos);
        }
    }
}
