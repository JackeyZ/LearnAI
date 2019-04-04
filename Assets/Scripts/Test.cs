using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    Dictionary<int, int> emenyThreatDic = new Dictionary<int, int>(); //敌人威胁度列表
    int threatSum = 0;

    void Start ()
    {
        InitData();
        int random = Random.Range(0, threatSum);    //获得随机数
        int attachTargetIndex = 0;                  //初始化，用于储存攻击目标下标
        int temp = 0; 
        foreach (var item in emenyThreatDic)
        {
            temp += item.Value;
            if(temp > random)
            {
                attachTargetIndex = item.Key;       //最终攻击目标
                break;
            }
        }
        Debug.Log(attachTargetIndex);   //打印到控制台
    }
	
    /// <summary>
    /// 初始化数据
    /// </summary>
    void InitData()
    {
        /*三个敌人*/
        emenyThreatDic.Add(1, 5);   //第一个敌人，5威胁度
        emenyThreatDic.Add(2, 10);  //第二个敌人，10威胁度
        emenyThreatDic.Add(3, 15);
        threatSum = CalThreatSum();             //威胁度总和
    }

    /// <summary>
    /// 计算威胁度总和
    /// </summary>
    /// <returns></returns>
    int CalThreatSum()
    {
        int result = 0;
        foreach (var item in emenyThreatDic)
        {
            result += item.Value;
        }
        return result;
    }
}
