using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Core
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private List<Room> _activeRooms;
        [SerializeField] private float _timeToDeactivateRooms = 0.5f;

        public bool IsRoomActive(Room room)
        {
            return _activeRooms.Contains(room);
        }

        /*  ____________ ___________ _______________
         * | Desativada |   Ativada |   Desativada  |
         * |------------|-----------|---------------|
         * | Ativada    |   Ativada |   Ativada     |
         * |------------|-----------|---------------|
         * | Desativada |   Ativada |   Desativada  |
         *  ____________ ___________ _______________
         */
        public bool ActiveRoom(Room activeRoom, List<Room> neighbours)
        {
            // check which room need deactivation
            foreach (Room room in _activeRooms)
            {
                if (!activeRoom.isNeighbour(room) && !room.Equals(activeRoom))
                    StartCoroutine(DeactivateRoom(room));
            }
            // check wich neighbour need to be active
            foreach (Room neighbour in neighbours)
            {
                if (IsRoomActive(neighbour)) continue;

                _activeRooms.Add(neighbour);
                neighbour.ToggleRoomActive(true);
                neighbour.SpawnEnemies();
            }
            // add the actual room to activation list
            if (!IsRoomActive(activeRoom))
            {
                _activeRooms.Add(activeRoom);
                activeRoom.SpawnEnemies();
            }

            return true;
        }

        public IEnumerator DeactivateRoom(Room room)
        {
            if (!IsRoomActive(room))
            {
                //Debug.Log("Tentando desativar sala ja desativada");
                yield return false;
            }

            yield return new WaitForSeconds(_timeToDeactivateRooms);
            room.DeactiveRoom();
            //Debug.Log("Sala "+ room.name + " Desativada");
            _activeRooms.Remove(room);
            yield return true;
        }
    }
}