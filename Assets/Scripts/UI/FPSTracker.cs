using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    public class FPSTracker : MonoBehaviour
    {
        public UnityEvent<float> fps;

        private float[] _trackedFPS;
        private float _currentFps = 0;

        [SerializeField] private int _showPerAmountUpdates;
        private int _passedUpdates = 0;

        [SerializeField] private float _displayPerTimeFrame = 1;
        private float _currentTimeFrame = 0;

        public void DisplayOnTextField(TextMeshProUGUI text)
        {
            text.text = $"{_currentFps} fps";
        }

        protected virtual void Awake()
        {
            _trackedFPS = new float[_showPerAmountUpdates];
        }

        protected virtual void Update()
        {
            _trackedFPS[_passedUpdates] = 1f / Time.deltaTime;
            _currentTimeFrame += Time.deltaTime;
            _passedUpdates++;

            if (_passedUpdates >= _showPerAmountUpdates || _currentTimeFrame >= _displayPerTimeFrame)
                TrackFps();
        }

        private void TrackFps()
        {
            float totalFps = 0;
            for (int i = 0; i < _passedUpdates; i++)
                totalFps += _trackedFPS[i];

            _currentFps = Mathf.Round(totalFps / _passedUpdates);
            _passedUpdates = 0;
            _currentTimeFrame = 0;

            fps.Invoke(_currentFps);
        }
    }
}
