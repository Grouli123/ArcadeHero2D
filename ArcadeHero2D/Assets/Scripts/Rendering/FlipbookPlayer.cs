using UnityEngine;

namespace ArcadeHero2D.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class FlipbookPlayer : MonoBehaviour
    {
        [SerializeField] private FlipbookClip clip;
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool randomStartFrame = false;

        SpriteRenderer _sr;
        int _frame;
        int _dir = 1;
        float _timer;
        bool _playing;
        float _speed = 1f;

        bool _oneShotActive;
        FlipbookClip _returnAfterOneShot;

        public bool IsPlaying => _playing;
        public bool IsOneShotActive => _oneShotActive;
        public FlipbookClip CurrentClip => clip;

        void Awake() => _sr = GetComponent<SpriteRenderer>();

        void OnEnable()
        {
            if (playOnEnable && clip != null) Play(clip);
        }

        public void Play(FlipbookClip newClip)
        {
            if (newClip == null || newClip.FrameCount == 0) { _playing = false; return; }

            clip = newClip;
            _playing = true;
            _dir = 1;
            _timer = 0f;
            _oneShotActive = false;
            _returnAfterOneShot = null;

            _frame = randomStartFrame ? Random.Range(0, clip.FrameCount) : 0;
            ApplyFrame();
        }

        public void Play() { if (clip != null) Play(clip); }
        public void Pause() => _playing = false;
        public void Resume() { if (clip != null) _playing = true; }
        public void Stop()
        {
            _playing = false;
            _frame = 0;
            ApplyFrame();
        }
        public void SetSpeed(float s) => _speed = Mathf.Max(0f, s);

        public void PlayOneShot(FlipbookClip oneShot, FlipbookClip returnTo)
        {
            if (oneShot == null || oneShot.FrameCount == 0) return;
            clip = oneShot;
            _returnAfterOneShot = returnTo;
            _oneShotActive = true;

            _playing = true;
            _dir = 1;
            _timer = 0f;
            _frame = 0;
            ApplyFrame();
        }

        void Update()
        {
            if (!_playing || clip == null || clip.FrameCount == 0) return;

            float dt = (clip.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * _speed;
            _timer += dt;

            float fd = clip.FrameDuration;
            while (_timer >= fd)
            {
                _timer -= fd;
                Step();
                if (!_playing) break;
            }
        }

        void Step()
        {
            int count = clip.FrameCount;

            if (clip.loopMode == FlipbookLoopMode.Loop)
            {
                _frame = (_frame + 1) % count;
                ApplyFrame();
                return;
            }

            if (clip.loopMode == FlipbookLoopMode.PingPong)
            {
                int next = _frame + _dir;
                if (next >= count || next < 0) { _dir = -_dir; next = _frame + _dir; }
                _frame = next;
                ApplyFrame();
                return;
            }

            // Once
            _frame++;
            if (_frame >= count)
            {
                _frame = count - 1;
                ApplyFrame();
                if (_oneShotActive)
                {
                    _oneShotActive = false;
                    if (_returnAfterOneShot != null)
                        Play(_returnAfterOneShot);
                    else
                        _playing = false;
                }
                else
                {
                    _playing = false;
                }
                return;
            }

            ApplyFrame();
        }

        void ApplyFrame()
        {
            if (_sr != null && clip != null && clip.FrameCount > 0)
                _sr.sprite = clip.frames[Mathf.Clamp(_frame, 0, clip.FrameCount - 1)];
        }
    }
}
