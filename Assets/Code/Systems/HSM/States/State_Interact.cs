
using UnityEngine;
namespace HSM{
    public class State_Interact : State
    {
        readonly PlayerContext ctx;
        private float currentWaitTime = 0;
        protected float timeToWait = 0;
        private float staminaDrain=0;
        private float startingStamina = 0;
        private int milkIndex=0;
        private InteractionType tempInteract;

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
            tempInteract = ctx.intendedInteraction;
            if(tempInteract == InteractionType.Milk && !TryFindMilk())
            {
                tempInteract = InteractionType.Basic;
            }
            Debug.Log("Pre: "+ tempInteract);
            ctx.currentInteract.BeginInteract(out timeToWait, out staminaDrain,ref tempInteract);
            Debug.Log("Post: "+ tempInteract);
            if(ctx.brushUpgrade && (tempInteract == InteractionType.Milk || tempInteract == InteractionType.Water))
            {
                staminaDrain /= 2;
            }

            else if(ctx.trowelUpgrade && tempInteract == InteractionType.Trowel)
            {
                staminaDrain /= 2;
            }

            if(ctx.currentStamina - staminaDrain < 0)
            {
                //Debug.Log("Stamina Drain is Too much to Handle");
                OnExit();
                return;
            }
            startingStamina = ctx.currentStamina;

            

            //Debug.Log("Interacting for: " + timeToWait);
        }
        protected override void OnExit()
        {
            //Debug.Log("End Interact");
            if(ctx.currentInteract != null)
            {
                ctx.currentInteract.EndInteract(currentWaitTime, ref tempInteract);
                if(tempInteract == InteractionType.Milk && milkIndex != -1)
                {
                    ctx.invData.items.TryRemoveAt(milkIndex);
                }
            }
            tempInteract = InteractionType.Basic;
            ctx.currentInteract = null;
            currentWaitTime = 0;
            timeToWait = 0;
            staminaDrain = 0;
            startingStamina = ctx.currentStamina;
        }

        protected override void OnUpdate(float deltaTime)
        {
            currentWaitTime += deltaTime;


            // drain stamina over time if need be
            if(staminaDrain > 0 && timeToWait > 0)
            {
                float t = currentWaitTime/timeToWait;

                ctx.currentStamina = Mathf.Lerp(startingStamina,startingStamina-staminaDrain,t);
            }
            // if time to wait is less than or equal to 0 just do it instantly instead of dividing by 0
            else if(staminaDrain>0) ctx.currentStamina -= staminaDrain;
        }

        private bool TryFindMilk()
        {
            foreach(Item item in ctx.invData.items.items)
            {
                if(item!=null && item.itemData != null && item.dataId == ctx.Milk.Id)
                {
                    milkIndex = item.currentIndex.y;
                    return true;
                }
            }
            milkIndex = -1;
            return false;
        }
    }
}
