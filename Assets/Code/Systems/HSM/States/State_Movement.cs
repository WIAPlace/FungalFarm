using UnityEngine;
namespace HSM {
    public class State_Movement : State
    {
        readonly PlayerContext ctx;
        public readonly State_Movement_Idle idle;
        public readonly State_Movement_Walk walk;
        public readonly State_Movement_Sprint sprint;


        public State_Movement(StateMachine machine, State parent,  PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
