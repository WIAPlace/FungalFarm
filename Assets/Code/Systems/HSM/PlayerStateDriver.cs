using System;
using System.Linq;
using JetBrains.Annotations;
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
        public LayerMask interactMask;
        
        public bool drawGizmos = true;

        public string lastPath;
        
        CharacterController controller;
        StateMachine machine;
        State root;

        [SerializeReference] private GameObject[] heldItems; 


        // Awake //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake() {
            if(ctx!=null && ctx.controller != null) controller = ctx.controller;
            
            ctx.cinCamPerlin = ctx.cinCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            ctx.cinCamController = ctx.cinCam.GetComponent<CinemachineInputAxisController>();
            //ctx.currentPerlin = ctx.idlePerlin; // set this as default
            //ctx.anim = GetComponentInChildren<Animator>();
            //ctx.renderer = GetComponent<Renderer>();
            
            ctx.currentStamina = ctx.MaxStamina;
            ctx.staminaRegen = ctx.baseStaminaRegen;
            ctx.staminaSystem.maxStamina = ctx.MaxStamina;

            root = new State_PlayerRoot(null, ctx);
            var builder = new StateMachineBuilder(root);
            machine = builder.Build();

            // Set Up Events
            input.MoveEvent += HandleMove;
            input.SprintEvent += HandleSprintEvent;
            input.SprintCancelledEvent += HandleSprintCancelledEvent;
            input.InteractEvent += HandleInteract;
            input.InteractCancelledEvent += HandleInteractCancelled;
            input.HotBarEvent += HandleHotBar;

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
            ctx.paused = false;

            // Lock currsor, will probably be moved somewhere else later
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            HandleHotBar(1);
        }

        // Destroy //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroy()
        {
            input.MoveEvent -= HandleMove;
            input.SprintEvent -= HandleSprintEvent;
            input.SprintCancelledEvent -= HandleSprintCancelledEvent;
            input.InteractEvent -= HandleInteract;
            input.InteractCancelledEvent -= HandleInteractCancelled;
            input.HotBarEvent -= HandleHotBar;
        }

        // Update //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update() {
            ctx.grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

            //Debug.Log("Pre");
            machine.Tick(Time.deltaTime);
            //Debug.Log("Post");

            var path = StatePath(machine.Root.Leaf());

            if (path != lastPath) {
                lastPath = path;
                //Debug.Log(lastPath);
            }

            
            float gravVel = ctx.velocity.y; // maintain gravity
            // Move in the direction of the controller.
            Vector3 horizontalVel = (controller.transform.right * ctx.velocity.x) + (controller.transform.forward * ctx.velocity.z);

            ctx.velocity = new Vector3(horizontalVel.x,gravVel,horizontalVel.z);

            controller.Move(ctx.velocity * Time.deltaTime);
        }

        // Misc /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDrawGizmosSelected() {
            if (!drawGizmos || groundCheck == null) return;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        static string StatePath(State s) {
            return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
        }

        // Handle Events //////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HandleMove(Vector2 moveInput) // Move
        {
            ctx.move.x = moveInput.x;
            ctx.move.z = moveInput.y;

            ctx.move.Normalize();
        }
        private void HandleSprintEvent() // Sprint
        {
            ctx.sprint = true;
        }
        private void HandleSprintCancelledEvent() // Sprint Cancelled
        {
            ctx.sprint = false;
        }
        private void HandleInteract()
        {
            if(ctx.currentInteract != null) return; // if there is already something being interacted with return
            //Debug.Log("CurrentInteract == null");
            
            Ray cinCamRay = new Ray(ctx.cinCamTransform.position,ctx.cinCamTransform.forward);
            if(Physics.Raycast(cinCamRay, out RaycastHit hitInfo, ctx.interactDistance, interactMask, QueryTriggerInteraction.Ignore))
            {
                if(hitInfo.collider.TryGetComponent<IInteractable>(out ctx.currentInteract))
                {
                    //Debug.DrawLine(ctx.cinCamTransform.position,hitInfo.point,Color.green);
                    //Debug.Log("Interactable Hit");
                }
            }
            else
            {
                //Debug.DrawLine(ctx.cinCamTransform.position,ctx.cinCamTransform.forward * ctx.interactDistance,Color.red);
            }
            
        }
        private void HandleInteractCancelled()
        {
            ctx.currentInteract=null;
        }

        private void HandleHotBar(int num)
        {
            switch (num)
            {
                case 1:
                    ctx.intendedInteraction = InteractionType.Basic;
                    SetItemActive(0);
                    break;

                case 2:
                    ctx.intendedInteraction = InteractionType.Trowel;
                    if(ctx.trowelUpgrade)SetItemActive(1);
                    else SetItemActive(0);
                    break;

                case 3:
                    ctx.intendedInteraction = InteractionType.Water;
                    if(ctx.brushUpgrade)SetItemActive(2);
                    else SetItemActive(0);
                    break;

                case 4:
                    ctx.intendedInteraction = InteractionType.Milk;
                    if(ctx.brushUpgrade)SetItemActive(2);
                    else SetItemActive(0);
                    break;

                default:
                    break;
            }
        }
        private void SetItemActive(int index)
        {
            foreach(GameObject obj in heldItems)
            {
                if(obj !=null && obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
            if(heldItems[index]!=null)heldItems[index].SetActive(true);
        }
    }

    [Serializable]
    public class PlayerContext {
        public Vector3 move;     // input direction
        public Vector3 velocity; 

        public bool grounded;
        public bool sprint;
        public bool paused;

        public bool brushUpgrade;
        public bool trowelUpgrade;
        public InteractionType intendedInteraction;

        // should probably be changed to a SO at some point
        [Header("Movement Vars")]
        public float moveSpeed = 6f;
        public float sprintSpeed = 10f;
        public float gravity = 9.81f;

        [Header("Stamina")]
        public float MaxStamina;
        public float currentStamina;
        public float baseStaminaRegen;
        public float staminaRegen;

        [Header("Other Vars")]
        public float interactDistance = 5f;     

        [Header("Camera Noise")]
        [Tooltip("X = Amplitude \nY = Frequency")] public Vector2 idlePerlin = new(.5f,.5f);
        [Tooltip("X = Amplitude \nY = Frequency")] public Vector2 walkPerlin = new(1,1);
        [Tooltip("X = Amplitude \nY = Frequency")] public Vector2 sprintPerlin = new(2,2);
        [HideInInspector] public Vector2 currentPerlin;
        public float transitionSpeed = 5f;

        //public float accel = 40f;
        //public float jumpSpeed = 7f;
        //public bool jumpPressed;

        [Header("Refrences")]
        public CharacterController controller;
        public Animator anim;
        public Renderer renderer;
        public CinemachineCamera cinCam;
        public Transform cinCamTransform => cinCam.transform;
        public CinemachineBasicMultiChannelPerlin cinCamPerlin;
        public CinemachineInputAxisController cinCamController;
        //public InputActionReference IAR;
        public IInteractable currentInteract; // if not null interact will begin
        public StaminaSystem staminaSystem;

        public ItemsDataBase invData;
        public ItemDetails Milk;

        public MushroomDetails intendedSpore = null;
    }
}

/* Putting this here so i don't forget how to do it. cause i will
    if ((targetLayers.value & (1 << collision.gameObject.layer)) != 0)
    {
                    
    }
*/