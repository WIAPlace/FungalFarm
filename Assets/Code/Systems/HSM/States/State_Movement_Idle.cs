using HSM;
using UnityEngine;
namespace HSM {
    public class State_Movement_Idle : State
    {
        readonly PlayerContext ctx;

        public State_Movement_Idle(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition() {
            return Mathf.Abs(ctx.move.magnitude) > 0.01f ? ((State_Movement)Parent).walkState : null;
        }

        protected override void OnEnter() {
            ctx.velocity = Vector3.zero;
            ctx.currentPerlin = ctx.idlePerlin;
        }
        
    }
}
