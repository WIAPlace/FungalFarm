using UnityEngine;

namespace HSM{
    public class State_Movement_Sprint : State
    {
        readonly PlayerContext ctx;
        public State_Movement_Sprint(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }
        

        protected override State GetTransition() {
            // if not moving return to idle;
            if(Mathf.Abs(ctx.move.magnitude) <= 0.01f) return ((State_Movement)Parent).idleState;
            
            // if not sprinting walk
            return !ctx.sprint ? ((State_Movement)Parent).walkState : null; 
        }

        protected override void OnEnter() {
            //ctx.cinCamPerlin.AmplitudeGain = ctx.sprintPerlin;
            ctx.currentPerlin = ctx.sprintPerlin;
        }

        protected override void OnUpdate(float deltaTime)
        {
            Vector3 targetMove = new Vector3(ctx.move.x * ctx.sprintSpeed, ctx.move.y, ctx.move.z * ctx.sprintSpeed);
            ctx.velocity = targetMove;


        }
    }
}
