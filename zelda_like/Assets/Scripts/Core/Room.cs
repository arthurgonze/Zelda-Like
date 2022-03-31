using Cinemachine;
using ZL.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Core
{
    public class Room : MonoBehaviour
    {
        [SerializeField] GameObject _virtualCamera;
        [SerializeField] private bool _isHUB = false;
        [SerializeField] private bool _isActive = false;
        [SerializeField] private List<Room> _neighbours;
        [SerializeField] private GameObject[] _enemies = new GameObject[4];
        [SerializeField] private Vector2[] _enemiesPositions = new Vector2[4];
        [SerializeField] private bool _enemiesSpawned = false;
        [SerializeField] private List<GameObject> _spawnedEnemies = new List<GameObject>();
        [SerializeField] private LayerMask _roomLayer;

        private Color _lineColor;
        public void Awake()
        {
            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>().gameObject;
            _virtualCamera.SetActive(false);
            _neighbours = new List<Room>();
            _neighbours.Clear();
        }

        public void Start()
        {
            _lineColor = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
            FindNeighbours();
        }

        //public void Update()
        //{
        //    //if(_neighbours.Count == 0)
        //    //    FindNeighbours();
        //    if (this.gameObject.name.Equals("Room00"))
        //        Debug.DrawRay(transform.position, Vector2.up * 8, _lineColor);
        //}
        private void FindNeighbours()
        {
            RaycastHit2D upNeighbour = Physics2D.Raycast(transform.position, Vector2.up, 10, _roomLayer);
            //Debug.DrawRay(transform.position, Vector2.up * 8, _lineColor);
            RaycastHit2D rightNeighbour = Physics2D.Raycast(transform.position, Vector2.right, 15, _roomLayer);
            //Debug.DrawRay(transform.position, Vector2.right * 13, _lineColor);
            RaycastHit2D downNeighbour = Physics2D.Raycast(transform.position, -Vector2.up, 10, _roomLayer);
            //Debug.DrawRay(transform.position, -Vector2.up * 8, _lineColor);
            RaycastHit2D leftNeighbour = Physics2D.Raycast(transform.position, -Vector2.right, 15, _roomLayer);
            //Debug.DrawRay(transform.position, -Vector2.right * 13, _lineColor);

            if (upNeighbour.collider != null)
            {
                if (!this.gameObject.name.Equals("Room00"))
                    _neighbours.Add(upNeighbour.collider.gameObject.GetComponent<Room>());
            }

            if (rightNeighbour.collider != null)
            {
                //if (this.gameObject.name.Equals("Room00"))
                //    Debug.Log("Found an right neighbour");
                _neighbours.Add(rightNeighbour.collider.gameObject.GetComponent<Room>());
            }

            if (downNeighbour.collider != null)
            {
                //if (this.gameObject.name.Equals("Room00"))
                //    Debug.Log("Found an lower neighbour");
                _neighbours.Add(downNeighbour.collider.gameObject.GetComponent<Room>());
            }

            if (leftNeighbour.collider != null)
            {
                //if (this.gameObject.name.Equals("Room00"))
                //    Debug.Log("Found an left neighbour");
                _neighbours.Add(leftNeighbour.collider.gameObject.GetComponent<Room>());
            }
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            //Debug.Log("Ativou a Sala: " + this.name);
            EnterRoom();
            //CheckReviveMonsters();
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            ToggleRoomCamera(false);
            //Debug.Log("Desativou a Sala: " + this.name);
            //DeactiveRoom();
        }

        public void EnterRoom()
        {
            ToggleRoomActive(true);
            ToggleRoomCamera(true);

            FindObjectOfType<RoomManager>().ActiveRoom(this, _neighbours);
        }

        public void ToggleRoomActive(bool toggle)
        {
            _isActive = toggle;
        }

        public void DeactiveRoom()
        {
            ToggleRoomActive(false);
            ToggleRoomCamera(false);
            DestroyEnemies();
        }

        private void CheckReviveMonsters()
        {
            if (!_isHUB) return;

            foreach (Health health in FindObjectsOfType<Health>())
                if (health.gameObject.tag == "Enemy")
                    health.Revive();
            FindObjectOfType<SavingWrapper>().Save();
        }

        private void ToggleRoomCamera(bool toggle)
        {
            _virtualCamera.SetActive(toggle);
        }

        public bool isNeighbour(Room room)
        {
            return _neighbours.Contains(room);
        }

        public void SpawnEnemies()
        {
            if (_enemies.Length == 0 || _spawnedEnemies.Count >= _enemies.Length) return;
            int cont = 0;
            foreach (GameObject enemy in _enemies)
            {
                GameObject spawnedEnemy = Instantiate(enemy, _enemiesPositions[cont], Quaternion.identity);
                _spawnedEnemies.Add(spawnedEnemy);
                cont++;
            }
        }

        public void DestroyEnemies()
        {
            if (_enemies.Length == 0 || _spawnedEnemies.Count == 0) return;
            foreach (GameObject enemy in _spawnedEnemies)
            {
                //_spawnedEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }
            _spawnedEnemies.Clear();
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}
