using UnityEngine;

public class MagicCastState : StateBase
{
    public MagicCastState(PlayerStateMachine stateMachine) : base(stateMachine) { }


    public override void Update()
    {
       
    }

    public override void HandleEvent(Event evt)
    {
        switch (evt)
        {
            case Event.DEAD:
                break;
        }
    }
}
