using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class CharacterSkills
{
    private Dictionary<int, SkillServer> _skills;
    private object _sync = new object();
    public CharacterSkills()
    {
        _skills = new Dictionary<int, SkillServer>();
    }

    public void AddSkillsList(List<SkillServer> skills)
    {
        ClearDict();

        foreach (SkillServer skill in skills)
        {
            _skills.Add(skill.Id, skill);  
        }
    }

    private void ClearDict()
    {
        if(_skills.Count > 0)
        {
            _skills.Clear();
        }
    }
    public void AddSkill(int id  , int pLevel , bool pPassive , bool pDisabled)
    {
        lock (_sync)
        {
            if (_skills.ContainsKey(id))
            {
                _skills.Remove(id);
                _skills.Add(id, new SkillServer(id, pLevel, pPassive, pDisabled));
            }
            else
            {
                _skills.Add(id, new SkillServer(id, pLevel, pPassive, pDisabled));
            }
        }
    }

    public SkillServer GetSkill(int id)
    {
        lock (_sync)
        {
            return (_skills.ContainsKey(id)) ? _skills[id] : null;
        }
    }
}
