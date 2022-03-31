using System.Collections.Generic;
using UnityEngine;

namespace ZL.Vision
{
    public class FOV : MonoBehaviour
    {
        [SerializeField] private float _viewRadius = 2;
        [SerializeField] private float _viewAngle = 60;

        [SerializeField] private LayerMask _obstacleMask, _detectionMask;// _allyMask;
        [SerializeField] private Collider2D[] _targetsInRadius;// _alliesInRadius;
        [SerializeField] private List<Transform> _visibleTargets = new List<Transform>();
        //[SerializeField] private List<Transform> _visibleAllies = new List<Transform>();
        private Vector2 closestAllyPosition;

        private void Update()
        {
            FindVisibleTargets();
        }

        void OnDrawGizmos()
        {
            // vision SPHERE
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);
        }

        void FindVisibleTargets()
        {
            _targetsInRadius = Physics2D.OverlapCircleAll(transform.position, _viewRadius, _detectionMask, -Mathf.Infinity, Mathf.Infinity);
            //_alliesInRadius = Physics2D.OverlapCircleAll(transform.position, _viewRadius, _allyMask, -Mathf.Infinity, Mathf.Infinity);

            _visibleTargets.Clear();
            //_visibleAllies.Clear();

            CheckForEnemies();
            //CheckForAllies();
        }

        private void CheckForEnemies()
        {
            for (int i = 0; i < _targetsInRadius.Length; i++)
            {
                Transform target = _targetsInRadius[i].transform;
                Vector2 dirTarget = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y);
                Vector2 dir = transform.right;

                if (Vector2.Angle(dirTarget, dir) < _viewAngle / 2)
                //if (Vector2.Angle(dirTarget, dir) < _viewAngle)
                {
                    float distanceTarget = Vector2.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, dirTarget, distanceTarget, _obstacleMask))
                        _visibleTargets.Add(target);
                }
            }
        }

        private void CheckForAllies()
        {
            //    float minDistance = -Mathf.Infinity;
            //    for (int i = 0; i < _alliesInRadius.Length; i++)
            //    {
            //        Transform ally = _alliesInRadius[i].transform;
            //        Vector2 dirAlly = new Vector2(ally.position.x - transform.position.x, ally.position.y - transform.position.y);
            //        Vector2 dir = transform.right;

            //        if (Vector2.Angle(dirAlly, dir) < _viewAngle / 2)
            //        //if (Vector2.Angle(dirTarget, dir) < _viewAngle)
            //        {
            //            float distanceTarget = Vector2.Distance(transform.position, ally.position);
            //            if (!Physics2D.Raycast(transform.position, dirAlly, distanceTarget, _obstacleMask))
            //            {
            //                _visibleAllies.Add(ally);
            //                if (distanceTarget < minDistance)
            //                    closestAllyPosition = ally.position;
            //            }
            //        }
            //    }
        }

        public Vector2 DirFromAngle(float angleDeg, bool global)
        {
            if (!global)
                angleDeg += transform.eulerAngles.z;

            float cos = Mathf.Cos(angleDeg * Mathf.Deg2Rad);
            float sin = Mathf.Sin(angleDeg * Mathf.Deg2Rad);

            return new Vector2(cos, sin);
        }

        public float GetViewAngle()
        {
            return this._viewAngle;
        }

        public float GetViewRadius()
        {
            return this._viewRadius;
        }

        public LayerMask GetObstacleMask()
        {
            return this._obstacleMask;
        }

        public List<Transform> GetVisibleTargets()
        {
            return this._visibleTargets;
        }

        public void SetViewRadius(float newRadius)
        {
            _viewRadius = newRadius;
        }

        public void SetViewAngle(float newAngle)
        {
            _viewAngle = newAngle;
        }

    }
}
