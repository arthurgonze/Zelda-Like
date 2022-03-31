using UnityEngine;

namespace ZL.Combat
{
    public class Shield : MonoBehaviour
    {
        [SerializeField] float _rotationSpeed;
        [SerializeField] float _rotationTime;

        Transform _rotationPoint;
        float _rotationTimeCounter = 0;
        // Use this for initialization

        // Update is called once per frame
        void Update()
        {
            transform.RotateAround(_rotationPoint.position, Vector3.forward, _rotationSpeed * Time.deltaTime);
            _rotationTimeCounter += Time.deltaTime;
            if (_rotationTimeCounter > _rotationTime)
                Destroy(this.gameObject);
        }

        public void SetRotationPoint(Transform point)
        {
            _rotationPoint = point;
        }

    }
}