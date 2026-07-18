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


        protected override void OnUpdate(float deltaTime) 
        {
            if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
            {
                ctx.velocity.y = 0;
            }

            ctx.velocity.y += -ctx.gravity * Time.deltaTime; // apply gravity

            // Match the player body's Y rotation to the camera target's Y rotation
            // Maybe change this to be changing a value rather than changing it directly in the state itself.
            Vector3 targetRotation = new Vector3(0, ctx.cinCamTransform.eulerAngles.y, 0);
            ctx.controller.transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
}
