using UnityEngine;

namespace HSM{
    public class State_Movement_Walk : State
    {
        readonly PlayerContext ctx;
        public State_Movement_Walk(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition() {
            // if not moving return to idle;
            if(Mathf.Abs(ctx.move.magnitude) <= 0.01f) return ((State_Movement)Parent).idleState;
            
            // if sprinting 
            return ctx.sprint ? ((State_Movement)Parent).sprintState : null; 
        }

        protected override void OnEnter() {
            ctx.currentPerlin = ctx.walkPerlin;
        }

        protected override void OnUpdate(float deltaTime)
        {
            //Debug.Log("Walk");
            float targetx = ctx.move.x * ctx.moveSpeed;
            float targetz = ctx.move.z * ctx.moveSpeed;
            ctx.velocity.x = targetx;
            ctx.velocity.z = targetz;
        }
    }
}
