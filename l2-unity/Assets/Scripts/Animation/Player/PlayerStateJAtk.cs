using System;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class PlayerStateJAtk : StateMachineBehaviour
{
    private float _startTime = -1;
    private float _endTime = -1;

    public string parameterName;
    private AnimationCurve _animationCurve;
    private bool _isSwitchIdle = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
  
        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);

        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        _animationCurve = new AnimationCurve();
        _isSwitchIdle = false;
  
        float timeAtk  = CalcBaseParam.CalculateTimeL2j(PlayerEntity.Instance.Stats.BasePAtkSpeed) / 2;
        _startTime = Time.time;
        _endTime = TimeUtils.ConvertMsToSec(timeAtk);
        float timeAnimation = clipInfos[0].clip.length;
        RecreateAnimationCurve(_animationCurve, _endTime, timeAnimation);
        PlayerAnimationController.Instance.UpdateAnimatorAtkSpeedL2j(timeAtk , timeAnimation);
 

        StopAnimationTrigger(animator , parameterName);

        //Debug.Log(" Attack Sate to Intention TIMEOUT Запуск е1 " + PlayerEntity.Instance.CurrentAttackCount + " end time " + _endTime + "  start time " + _startTime +  " timeAtk " + timeAtk);
    }
     
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = Time.time;
        float timeOut = currentTime - _startTime;

        if (timeOut >= _endTime) SwitchToIdle(stateInfo);

        float normalizedTime = timeOut / _endTime;
        float speed = _animationCurve.Evaluate(normalizedTime);

        PlayerAnimationController.Instance.SetPAtkSpeed(speed);
    }

    private void SwitchToIdle(AnimatorStateInfo stateInfo)
    {
        float currentNormalizedTime = stateInfo.normalizedTime;
        //Debug.Log("Сколько осталось до завершения анимации " + currentNormalizedTime);
        if (!_isSwitchIdle & currentNormalizedTime > 0.9f)
        {
           // Debug.Log("Сколько осталось до завершения мы переключаемся в idle" + currentNormalizedTime);
            _isSwitchIdle = true;
            PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_IDLE);
            PlayerStateMachine.Instance.NotifyEvent(Event.WAIT_RETURN);
          //  Debug.Log("Attack Sate to Intention switch OK");
        }
    }

  

    private void StopAnimationTrigger(Animator animator , string parameterName)
    {
        if (animator.GetBool(parameterName) != false)
        {
            AnimationManager.Instance.StopCurrentAnimation(parameterName);
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }


    private void RecreateAnimationCurve(AnimationCurve animationCurve , float timeAtk , float timeAnimation)
    {
        Keyframe startKey = new Keyframe(0f, 0f); 
        Keyframe endKey = new Keyframe(timeAtk, timeAnimation);
        //default to sword slow down
        Keyframe slowDownAttackKey = new Keyframe(0.07511136f, 0.4373413f);

        animationCurve.AddKey(startKey);
        animationCurve.AddKey(slowDownAttackKey);
        animationCurve.AddKey(endKey);
    }

  

}