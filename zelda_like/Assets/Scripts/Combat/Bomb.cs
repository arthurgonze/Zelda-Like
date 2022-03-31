using ZL.Core;
using UnityEngine;

namespace ZL.Combat
{
    public class Bomb : MonoBehaviour
    {
        //[SerializeField] float _intialRadius = 0.25f;
        //[SerializeField] float _finalRadius = 0.5f;
        [SerializeField] float _damage = 10f;
        //[SerializeField] float _explosionSpeed = 2f;
        // Use this for initialization

        //private Vector2 _initialScale;
        //private Vector2 _finalScale;
        void Start()
        {
            //this.transform.localScale.Set(_intialRadius, _intialRadius, _intialRadius);
            //_initialScale = new Vector2(_intialRadius, _intialRadius);
            //_finalScale = new Vector2(_finalRadius, _finalRadius);
        }

        // Update is called once per frame
        void Update()
        {
            //if(this.transform.localScale.x < _finalRadius-0.1f)
            //{
            //    this.transform.localScale = Vector2.Lerp(this.transform.localScale, _finalScale, Time.deltaTime* _explosionSpeed);
            //}
            //else
            //{
            //    Destroy(this.gameObject);
            //}
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
                other.gameObject.GetComponent<Health>().TakeDamage(_damage);
        }

        public void DestroyBomb()
        {
            Destroy(this.gameObject);
        }
    }
}