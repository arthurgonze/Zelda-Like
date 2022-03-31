using ZL.Control;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Cinematics
{
    public class CinematicController : MonoBehaviour
    {
        [SerializeField] private List<Cinematic> _cinematics = new List<Cinematic>();

        // cached reference
        private GameObject _player;

        // Use this for initialization
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }



        public void StartCinematic(int id)
        {
            _player.GetComponent<PlayerController>().TogglePlayerControl(false);
            _cinematics[id].Play();
        }
    }
}