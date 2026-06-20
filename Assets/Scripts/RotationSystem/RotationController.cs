using System;
using System.Collections.Generic;
using UnityEngine;

namespace HammerGolf.RotationSystem {

    /// <summary>
    /// Central singleton that owns all IRotatable objects in the scene.
    ///
    /// Responsibilities:
    ///   • Maintain a registry of every active IRotatable.
    ///   • Maintain a Collider → IRotatable lookup table built at registration time.
    ///   • Track which IRotatable (if any) the player is currently rotating.
    ///   • Read input and forward scaled, axis-masked deltas to the active rotatable.
    ///   • Fire events so the rest of the game can react without coupling.
    ///
    /// Input strategy is injected via IRotationInputProvider so you can swap
    /// mouse drag, touch, gamepad stick, etc. without touching this class.
    /// </summary>
    public sealed class RotationController : MonoBehaviour {

        // ── Singleton ────────────────────────────────────────────────────────

        public static RotationController Instance { get; private set; }

        /// <summary>Safe null-check used by RotatableRegistrar on teardown.</summary>
        public static bool HasInstance => Instance != null;

        // ── Events ───────────────────────────────────────────────────────────

        /// <summary>Fired whenever a new rotatable becomes active (arg may be null = deselected).</summary>
        public event Action<IRotatable> OnActiveRotatableChanged;

        /// <summary>Fired every frame the active rotatable is being rotated, with the applied delta.</summary>
        public event Action<IRotatable, Vector3> OnRotated;

        // ── Inspector ────────────────────────────────────────────────────────

        [Tooltip("Provide a MonoBehaviour that implements IRotationInputProvider. " +
                 "Defaults to MouseDragInputProvider if left empty.")]
        [SerializeField] private MonoBehaviour _inputProviderOverride;

        [Tooltip("Global speed multiplier applied on top of each IRotatable's own RotationSpeed.")]
        [SerializeField, Range(0.1f, 10f)] private float _globalSpeedMultiplier = 1f;

        // ── Private state ─────────────────────────────────────────────────────

        private readonly HashSet<IRotatable> _registry = new();

        /// <summary>
        /// Maps every registered Collider directly to its owning IRotatable.
        /// Built by RotatableRegistrar at registration time — zero runtime traversal.
        /// </summary>
        private readonly Dictionary<Collider, IRotatable> _colliderMap = new();

        private IRotatable _active;
        private IRotationInputProvider _input;

        // ── Unity lifecycle ───────────────────────────────────────────────────

        private void Awake() {
            if (Instance != null && Instance != this) {
                Debug.LogWarning("[RotationController] Duplicate instance destroyed.", this);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _input = _inputProviderOverride as IRotationInputProvider
                     ?? GetComponent<IRotationInputProvider>()
                     ?? gameObject.AddComponent<MouseDragInputProvider>();
        }

        private void Update() {
            if (_active == null) return;

            Vector3 rawDelta = _input.GetDelta();
            if (rawDelta == Vector3.zero) return;

            Vector3 delta = Vector3.Scale(rawDelta, _active.RotationAxis)
                            * (_active.RotationSpeed * _globalSpeedMultiplier * Time.deltaTime);

            _active.Rotate(delta);
            OnRotated?.Invoke(_active, delta);
        }

        private void OnDestroy() {
            if (Instance == this) Instance = null;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Register an IRotatable and map its colliders into the lookup table.
        /// Called by RotatableRegistrar on OnEnable.
        /// </summary>
        public void Register(IRotatable rotatable, Collider[] colliders) {
            _registry.Add(rotatable);

            if (colliders == null) return;
            foreach (var col in colliders) {
                if (col != null)
                    _colliderMap[col] = rotatable;
            }
        }

        /// <summary>
        /// Deregister an IRotatable and remove its colliders from the lookup table.
        /// Called by RotatableRegistrar on OnDisable.
        /// </summary>
        public void Deregister(IRotatable rotatable, Collider[] colliders) {
            _registry.Remove(rotatable);

            if (colliders != null) {
                foreach (var col in colliders) {
                    if (col != null)
                        _colliderMap.Remove(col);
                }
            }

            if (_active == rotatable)
                SetActive(null);
        }

        /// <summary>
        /// Resolve a hit Collider to its owning IRotatable.
        /// O(1) dictionary lookup — no hierarchy traversal.
        /// Returns null if the collider belongs to no registered rotatable.
        /// </summary>
        public bool TryResolve(Collider collider, out IRotatable rotatable) =>
            _colliderMap.TryGetValue(collider, out rotatable);

        /// <summary>
        /// Make <paramref name="rotatable"/> the active one being rotated.
        /// Pass null to deselect. The rotatable must already be registered.
        /// </summary>
        public void SetActive(IRotatable rotatable) {
            if (rotatable != null && !_registry.Contains(rotatable)) {
                Debug.LogWarning("[RotationController] SetActive called with an unregistered IRotatable.");
                return;
            }

            if (_active == rotatable) return;

            _active?.OnRotationEnd();
            _active = rotatable;
            _active?.OnRotationStart();

            OnActiveRotatableChanged?.Invoke(_active);
        }

        /// <summary>Returns a snapshot of all currently registered rotatables.</summary>
        public IReadOnlyCollection<IRotatable> GetAll() => _registry;

        /// <summary>Currently active rotatable (may be null).</summary>
        public IRotatable Active => _active;
    }
}