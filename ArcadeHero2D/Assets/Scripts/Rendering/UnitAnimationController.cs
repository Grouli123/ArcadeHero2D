using ArcadeHero2D.Domain.Base;
using ArcadeHero2D.Domain.Contracts;
using UnityEngine;

namespace ArcadeHero2D.Rendering
{
    [RequireComponent(typeof(FlipbookPlayer))]
    public sealed class UnitAnimationController : MonoBehaviour
    {
        [Header("Clips")]
        [SerializeField] private FlipbookClip idleLoop;
        [SerializeField] private FlipbookClip moveLoop;
        [SerializeField] private FlipbookClip attackOnce; 
        [SerializeField] private FlipbookClip hurtOnce;   
        [SerializeField] private FlipbookClip deathOnce;  

        [Header("Auto-bind (optional)")]
        [SerializeField] private UnitBase unit;           
        [SerializeField] private FlipbookPlayer player;   

        [Header("Tuning")]
        [SerializeField] private float minHurtInterval = 0.12f; 

        enum Overlay { None = 0, Attack = 1, Hurt = 2, Death = 3 }

        private bool _moving;
        private Overlay _overlay = Overlay.None;
        private float _overlayTimer;
        private float _lastHurtTime = -999f;
        private bool _dead;

        private IHealth _health;
        private int _lastHP = -1;

        private void Awake()
        {
            if (!player) player = GetComponent<FlipbookPlayer>();
            if (!unit)   unit   = GetComponentInParent<UnitBase>();
            BindHealth(unit != null ? unit.Health : null);
            PlayBase();
        }

        private void OnEnable()  => Subscribe();
        private void OnDisable() => Unsubscribe();

        private void Subscribe()
        {
            if (_health != null)
            {
                _health.OnChanged += OnHealthChanged; 
                _health.OnDied    += OnDied;
            }
        }
        private void Unsubscribe()
        {
            if (_health != null)
            {
                _health.OnChanged -= OnHealthChanged;
                _health.OnDied    -= OnDied;
            }
        }

        public void BindHealth(IHealth health)
        {
            if (_health == health) return;
            Unsubscribe();
            _health = health;
            _lastHP = _health != null ? _health.Current : -1;
            Subscribe();
        }

        private void Update()
        {
            if (_overlay != Overlay.None)
            {
                _overlayTimer -= Time.deltaTime;
                if (_overlayTimer <= 0f && !_dead)
                {
                    _overlay = Overlay.None;
                    PlayBase();
                }
            }
        }

        private void OnHealthChanged(int current, int max)
        {
            if (_dead) return;

            if (_lastHP >= 0 && current < _lastHP)
            {
                if (Time.time - _lastHurtTime >= minHurtInterval)
                {
                    _lastHurtTime = Time.time;
                    RequestHurt();
                }
            }
            _lastHP = current;

            if (current <= 0) OnDied(); 
        }

        private void OnDied()
        {
            if (_dead) return;
            _dead = true;
            PlayOverlay(Overlay.Death, deathOnce, 0.6f, returnToBase:false);
        }

        public void SetMoving(bool moving)
        {
            if (_dead) return;
            if (_moving == moving) return;
            _moving = moving;

            if (_overlay == Overlay.None)
                PlayBase();
        }

        public void RequestAttack()
        {
            if (_dead || attackOnce == null) return;
            if (_overlay == Overlay.Hurt || _overlay == Overlay.Death) return;
            PlayOverlay(Overlay.Attack, attackOnce, 0.35f, returnToBase:true);
        }

        public void RequestHurt()
        {
            if (_dead || hurtOnce == null) return;
            if (_overlay == Overlay.Death || _overlay == Overlay.Hurt) return;
            PlayOverlay(Overlay.Hurt, hurtOnce, 0.35f, returnToBase:true);
        }

        public float PlayDeath() 
        {
            if (_dead) return 0f;
            _dead = true;
            return PlayOverlay(Overlay.Death, deathOnce, 0.6f, returnToBase:false);
        }

        public float GetDeathDuration()
        {
            if (deathOnce == null || deathOnce.FrameCount == 0) return 0.6f;
            return deathOnce.FrameCount * deathOnce.FrameDuration;
        }

        private void PlayBase()
        {
            var baseClip = (_moving ? moveLoop : idleLoop) ?? idleLoop;
            if (baseClip != null) player.Play(baseClip);
        }

        private float PlayOverlay(Overlay ov, FlipbookClip clip, float fallbackDur, bool returnToBase)
        {
            float dur = (clip != null && clip.FrameCount > 0) ? clip.FrameCount * clip.FrameDuration : fallbackDur;
            _overlay = ov;
            _overlayTimer = dur;

            if (clip != null)
            {
                if (returnToBase)
                {
                    var baseClip = (_moving ? moveLoop : idleLoop) ?? idleLoop;
                    player.PlayOneShot(clip, baseClip);
                }
                else
                {
                    player.Play(clip); 
                }
            }
            return dur;
        }
    }
}