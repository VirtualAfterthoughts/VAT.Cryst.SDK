using UnityEngine;

namespace VAT.Props
{
    public class GunRepeater : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GunBolt _bolt = null;

        [SerializeField]
        private GunHammer _hammer = null;

        [SerializeField]
        private GunBarrel _barrel = null;

        [Header("Specifications")]
        [SerializeField]
        [Min(0f)]
        private float _roundsPerMinute = 800f;

        [SerializeField]
        private bool _isAutomatic = false;

        [SerializeField]
        [Min(0)]
        private int _maxBurst = 0;

        private float SecondsPerRound => 60f / _roundsPerMinute;

        private bool _isCycling = false;
        private float _cycleTime = 0f;

        private void OnEnable()
        {
            _bolt.OnStateChanged += OnStateChanged;
            _barrel.OnFire += OnFire;
        }

        private void OnDisable()
        {
            _bolt.OnStateChanged -= OnStateChanged;
            _barrel.OnFire -= OnFire;
        }

        private void OnFire()
        {
            _isCycling = true;
            _cycleTime = 0f;
        }

        private void OnStateChanged(BoltState previous, BoltState current)
        {
            if (current == BoltState.CLOSED)
            {
                _hammer.Cock();

                if (_isAutomatic && _hammer.IsActuated)
                {
                    _hammer.Release();
                }
            }
        }

        private bool _wasReturning = false;

        private void Update()
        {
            float percent = _cycleTime / SecondsPerRound;

            bool isReturning = percent >= 0.5f;

            if (isReturning)
            {
                percent = 1f - percent;
            }

            percent *= 2f;

            if (_isCycling)
            {
                _bolt.Overriden = true;

                if (isReturning && !_wasReturning && !_bolt.Locked)
                {
                    _bolt.TargetPercent = 1f;
                }

                _wasReturning = isReturning;

                if (!_bolt.Locked)
                {
                    _bolt.TargetPercent = percent;
                }

                _cycleTime += Time.deltaTime;

                if (_cycleTime > SecondsPerRound)
                {
                    _isCycling = false;
                    _cycleTime = 0f;

                    _bolt.Overriden = false;

                    if (!_bolt.Locked)
                    {
                        _bolt.ResetTarget();
                    }
                }
            }
        }
    }
}
