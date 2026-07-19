using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
namespace HSM {
    public class State_Movement : State
    {
        readonly PlayerContext ctx;
        public readonly State_Movement_Idle idleState;
        public readonly State_Movement_Walk walkState;
        public readonly State_Movement_Sprint sprintState;


        public State_Movement(StateMachine machine, State parent,  PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
            idleState = new State_Movement_Idle(machine,this,ctx);
            walkState = new State_Movement_Walk(machine,this,ctx);
            sprintState = new State_Movement_Sprint(machine,this,ctx);
        }

        protected override State GetInitialState() => idleState;

        protected override State GetTransition()
        {
            // if interact is not null switch to interact state.
            return ctx.currentInteract != null ? ((State_PlayerRoot)Parent).interactState : null;
        }


        protected override void OnUpdate(float deltaTime) 
        {
            if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
            {
                ctx.velocity.y = 0;
            }
            //Debug.Log("Move");
            ctx.velocity.y += -ctx.gravity * deltaTime; // apply gravity

            // Match the player body's Y rotation to the camera target's Y rotation
            // Maybe change this to be changing a value rather than changing it directly in the state itself.
            Vector3 targetRotation = new Vector3(0, ctx.cinCamTransform.eulerAngles.y, 0);
            ctx.controller.transform.rotation = Quaternion.Euler(targetRotation);

            ChangePerlin(deltaTime);
        }

        protected override void OnEnter()
        {
            // Change this to Sensitivity Later on.
            ctx.cinCamController.Controllers[0].Input.Gain = 1;
            ctx.cinCamController.Controllers[1].Input.Gain = -1;

            
        }
        protected override void OnExit()
        {
            ctx.cinCamPerlin.AmplitudeGain = ctx.idlePerlin.x;
            ctx.cinCamPerlin.FrequencyGain = ctx.idlePerlin.y;

            // Disable camera controlls by making inputs = 0;
            foreach (var controllerAxis in ctx.cinCamController.Controllers)
            {
                controllerAxis.Input.Gain = 0;
            }


            ctx.velocity = Vector3.zero;
            // make sure perlin is back to what it should be if we wernt moving.
            
            //base.OnExit();
        }
        

        private void ChangePerlin(float deltaTime)
        {
            // Multi Channel Perlin 
            if(ctx.cinCamPerlin.AmplitudeGain != ctx.currentPerlin.x){ // updates the multi channel perlin to the perfered amount (Amplitude)
                ctx.cinCamPerlin.AmplitudeGain = Mathf.Lerp(ctx.cinCamPerlin.AmplitudeGain, ctx.currentPerlin.x, deltaTime * ctx.transitionSpeed);
            }
            if(ctx.cinCamPerlin.FrequencyGain != ctx.currentPerlin.y){ // updates the multi channel perlin to the perfered amount (Frequency)
                ctx.cinCamPerlin.FrequencyGain = Mathf.Lerp(ctx.cinCamPerlin.AmplitudeGain, ctx.currentPerlin.y, deltaTime * ctx.transitionSpeed);
            }
            // will need to find out a way to make perlin go to idle when exit movement
        }
    }
}
