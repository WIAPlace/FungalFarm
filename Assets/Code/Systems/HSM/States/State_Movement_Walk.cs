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

        protected override void OnUpdate(float deltaTime)
        {
            Vector3 targetMove = new Vector3(ctx.move.x * ctx.moveSpeed, ctx.move.y, ctx.move.z * ctx.moveSpeed);
            ctx.velocity = targetMove;
        }
    }
}
