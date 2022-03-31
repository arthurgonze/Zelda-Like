using Cinemachine;
using UnityEngine;

namespace ZL.Core
{
    public class CameraUtils : MonoBehaviour
    {
        private float _cameraShakingTime = 0;
        private float _cameraShakeMagnitude;
        private float _cameraShakeDuration;

        // cached references
        private CinemachineBrain _cinemachineBrain;
        private CinemachineVirtualCamera _activeCinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

        // Use this for initialization
        void Awake()
        {
            _cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_cameraShakingTime <= 0) return;

            _cameraShakingTime -= Time.deltaTime;
            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(_cameraShakeMagnitude, 0f, 1 - (_cameraShakingTime / _cameraShakeDuration));
        }

        public void ShakeCamera(float cameraShakeMagnitude, float cameraShakeDuration)
        {
            UpdateCameraReference();
            _cameraShakeMagnitude = cameraShakeMagnitude;
            _cameraShakingTime = cameraShakeDuration;
        }

        private void UpdateCameraReference()
        {
            _activeCinemachineVirtualCamera = _cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
            _cinemachineBasicMultiChannelPerlin = _activeCinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
}