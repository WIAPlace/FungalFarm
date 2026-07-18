using UnityEngine;

namespace HSM {
public class State_PlayerRoot : State
    {
        readonly PlayerContext ctx;
        public readonly State_Movement moveState;

        public State_PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null) {
            this.ctx = ctx;
            moveState = new State_Movement(m,this,ctx);
        }

        protected override State GetInitialState() => moveState;
    }
}
