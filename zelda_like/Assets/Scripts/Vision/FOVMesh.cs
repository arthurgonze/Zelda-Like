using System.Collections.Generic;
using UnityEngine;

namespace ZL.Vision
{
    /**
     * This class generates the FoV mesh on screen
     */
    [ExecuteInEditMode]
    public class FOVMesh : MonoBehaviour
    {
        [SerializeField] private float _meshRes = 2;

        private RaycastHit2D _hit;
        private Vector3[] _vertices;
        private int[] _triangles;
        private int _stepCount;

        // Cached References
        private FOV _fov;
        private Mesh _mesh;

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().sharedMesh;
            _fov = GetComponentInParent<FOV>();
        }

        void LateUpdate()
        {
            MakeMesh();
        }

        private void MakeMesh()
        {
            _stepCount = Mathf.RoundToInt(_fov.GetViewAngle() * _meshRes);
            float stepAngle = _fov.GetViewAngle() / _stepCount;

            List<Vector3> viewVertex = new List<Vector3>();

            _hit = new RaycastHit2D();

            for (int i = 0; i < _stepCount; i++)
            {
                float angle = _fov.transform.eulerAngles.y - (_fov.GetViewAngle() / 2) + (stepAngle * i);
                //float angle = _fov.transform.eulerAngles.y - (_fov.GetViewAngle()) + (stepAngle * i);
                Vector3 dir = _fov.DirFromAngle(angle, false);

                _hit = Physics2D.Raycast(_fov.transform.position, dir, _fov.GetViewRadius(), _fov.GetObstacleMask());

                if (_hit.collider == null)
                    viewVertex.Add(transform.position + dir.normalized * _fov.GetViewRadius());
                else
                    viewVertex.Add(transform.position + dir.normalized * _hit.distance);
            }

            int vertexCount = viewVertex.Count + 1;

            _vertices = new Vector3[vertexCount];
            _triangles = new int[(vertexCount - 2) * 3];
            _vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                _vertices[i + 1] = transform.InverseTransformPoint(viewVertex[i]);

                if (i < vertexCount - 2)
                {
                    _triangles[i * 3 + 2] = 0;
                    _triangles[i * 3 + 1] = i + 1;
                    _triangles[i * 3] = i + 2;
                }
            }

            if (_mesh)
            {
                _mesh.Clear();
                _mesh.vertices = _vertices;
                _mesh.triangles = _triangles;
                _mesh.RecalculateNormals();
            }
            else
            {
                Debug.Log("FOV Mesh não encontrada: " + gameObject.name);
                _mesh = GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}