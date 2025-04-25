using System.Collections.Generic;
using UnityEngine;

public interface IAnimationManager
{
    void SetAnimationManager(PlayerAnimationController controller , PlayerEntity Player);
    void PlayAnimation(string animationName , bool disableTriggerAfterStart);
    void PlayOriginalAnimation(string animationName);
    string GetCurrentAnimationName();
    string GetLastAnimationName();
    void StopCurrentAnimation(string paramName);

    void PlayMonsterAnimation(int objId, NetworkAnimationController controllerAnimator, string animationName);

    void StopMonsterCurrentAnimation(Animator animator, string animationName);

    Dictionary<string, float> PlayerGetAllFloat();

    void PlayerSetAllFloat(Dictionary<string, float> floatValues);
}
