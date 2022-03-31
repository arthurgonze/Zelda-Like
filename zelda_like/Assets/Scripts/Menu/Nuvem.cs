using UnityEngine;

namespace ZL.Menu
{
    public class Nuvem : MonoBehaviour
    {
        [SerializeField] private float _xSpeed;
        [SerializeField] private float _dir = 1f;

        private RectTransform objectTransform;
        private Vector3 _initialPos;
        private float _limitXPosition;

        // Start is called before the first frame update
        void Start()
        {
            objectTransform = this.GetComponent<RectTransform>();
            _initialPos = objectTransform.localPosition;
            _limitXPosition = -_initialPos.x;
            //Debug.Log("Name: "+ this.gameObject.name + ", Transform pos: " + this.transform.position + ", Initial Pos: " + objectTransform.position + ", LocalInitial Pos: " + objectTransform.localPosition);
        }

        // Update is called once per frame
        void Update()
        {
            float step = _xSpeed * _dir;
            float newX = objectTransform.localPosition.x + step + Time.deltaTime;

            objectTransform.localPosition = new Vector3(newX, objectTransform.localPosition.y, objectTransform.localPosition.z);

            if ((_dir > 0 && (objectTransform.localPosition.x > _limitXPosition)) || (_dir < 0 && (objectTransform.localPosition.x < _limitXPosition)))
                objectTransform.localPosition = _initialPos;
        }
    }
}
