using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityUtils;

namespace HSM {
    public class PlayerStateDriver : MonoBehaviour {
        public PlayerContext ctx = new PlayerContext();
        public InputReader input;

        public Transform groundCheck;

        public float groundRadius = 0.2f;
        public LayerMask groundMask;
        public bool drawGizmos = true;
        string lastPath;

        CharacterController controller;
        StateMachine machine;
        State root;

        void Awake() {
            if(ctx!=null && ctx.controller != null) controller = ctx.controller;
            
            //ctx.anim = GetComponentInChildren<Animator>();
            //ctx.renderer = GetComponent<Renderer>();

            root = new State_PlayerRoot(null, ctx);
            var builder = new StateMachineBuilder(root);
            machine = builder.Build();

            // Set Up Events
            input.MoveEvent += HandleMove;
            input.SprintEvent += HandleSprintEvent;
            input.SprintCancelledEvent += HandleSprintCancelledEvent;

            // fallback: create a groundCheck just below the collider's bounds
            if (groundCheck == null) {
                var col = GetComponent<Collider>();
                var t = new GameObject("groundCheck").transform;
                t.SetParent(transform, false);
                var y = col ? (-col.bounds.extents.y + 0.01f) : -0.5f;
                t.localPosition = new Vector3(0, y, 0);
                groundCheck = t;
            }
            ctx.sprint = false;

            // Lock currsor, will probably be moved somewhere else later
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnDestroy()
        {
            input.MoveEvent -= HandleMove;
            input.SprintEvent -= HandleSprintEvent;
            input.SprintCancelledEvent -= HandleSprintCancelledEvent;
        }

        void Update() {
            ctx.grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

            machine.Tick(Time.deltaTime);

            var path = StatePath(machine.Root.Leaf());

            if (path != lastPath) {
                //Debug.Log("State", path);
                lastPath = path;
            }

            // Move in the direction of the controller.
            ctx.velocity = (controller.transform.right * ctx.velocity.x) + (controller.transform.forward * ctx.velocity.z);

            controller.Move(ctx.velocity * Time.deltaTime);
        }

        void OnDrawGizmosSelected() {
            if (!drawGizmos || groundCheck == null) return;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        static string StatePath(State s) {
            return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
        }

        // Handle Events ///////////////////////////////////////////////////////////////////////////
        private void HandleMove(Vector2 moveInput)
        {
            ctx.move.x = moveInput.x;
            ctx.move.z = moveInput.y;

            ctx.move.Normalize();
        }
        private void HandleSprintEvent()
        {
            ctx.sprint = true;
        }
        private void HandleSprintCancelledEvent()
        {
            ctx.sprint = false;
        }
    }

    [Serializable]
    public class PlayerContext {
        public Vector3 move;     // input direction
        public Vector3 velocity; 
        public bool grounded;
        public bool sprint;
        public float moveSpeed = 6f;
        public float sprintSpeed = 10f;
        public float accel = 40f;
        public float gravity = 9.81f;
        //public float jumpSpeed = 7f;
        //public bool jumpPressed;
        public Animator anim;
        public CharacterController controller;
        public Renderer renderer;
        public CinemachineCamera cinCam;
        public Transform cinCamTransform => cinCam.transform;
        
    }
}