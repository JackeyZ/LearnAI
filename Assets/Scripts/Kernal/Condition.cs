using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition {
    public delegate bool ConditionFunc();
    ConditionFunc conditionFunc;
    public Condition(ConditionFunc conditionFunc)
    {
        this.conditionFunc = conditionFunc;
    }
    
    public bool Pass()
    {
        return conditionFunc();
    }

}
