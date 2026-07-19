
using UnityEngine;
namespace HSM{
    public class State_Interact : State
    {
        readonly PlayerContext ctx;
        private float currentWaitTime = 0;
        private float timeToWait = 0;

        public State_Interact(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;   
        }

        protected override State GetTransition()
        {
            if(ctx.currentInteract == null ) return ((State_PlayerRoot)Parent).moveState; // if there is not interactable exit
            return currentWaitTime >= timeToWait ? ((State_PlayerRoot)Parent).moveState : null; // if interact time is over exit
        }

        protected override void OnEnter()
        {
            ctx.currentInteract.BeginInteract(out timeToWait);
            Debug.Log("Interacting for: " + timeToWait);
        }
        protected override void OnExit()
        {
            //Debug.Log("End Interact");
            if(ctx.currentInteract != null)
            {
                ctx.currentInteract.EndInteract(currentWaitTime);
            }
            ctx.currentInteract = null;
            currentWaitTime = 0;
            timeToWait = 0;
        }

        protected override void OnUpdate(float deltaTime)
        {
            currentWaitTime += deltaTime;
        }
    }
}
