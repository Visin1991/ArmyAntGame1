/*------------------------------------------------------------------
 Author's name: Wei,Zhu
 e-mail: zhuzhanhao1991@gmail.com & zhuzhanhao1991@qq.com
 data: Thrusday, Feburary 23,2017
 
 ------------------------------------------------------------------*/

using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

using Test1_2;

public class WeiASMB1_2 : StateMachineBehaviour
{
    //For internal bool value change
    public BoolValues[] boolValus;

    public FloatValue[] enterFloatValus;
    public FloatValue[] exitFloatValus;

    public IntValue[] enterIntValus;
    public IntValue[] exitIntValus;

    /// <summary>
    /// Block mask did almost the same thing as CallBack function.
    /// </summary>
    [Header("=======Enter Mask=======")]
    public bool inverseEnter;
    public MaskTypes[] enterBlockMasks;
    int enterBlockMaskValue;

    [Header("=======Exit Mask=======")]
    public bool inverseExit;
    public MaskTypes[] exitBlockMasks;
    int exitBlockMaskValue;

    [Header("=======Void CallBack=======")]
    public PlayerController1_2.NonParamsCallBackType[] enterCallbacks;
    public delegate void EnterDel(PlayerController1_2 _p);
    List<EnterDel> enterDels = new List<EnterDel>();
    //-------------------------------------------------
    public PlayerController1_2.NonParamsCallBackType[] exitCallbacks;
    public delegate void ExitDel(PlayerController1_2 _p);
    List<ExitDel> exitDels = new List<ExitDel>();

    [Header("=======Params CallBacks=======")]
    public SpecialCallBack[] enterSpecialCallbacks;
    public delegate void EnterSpecial(PlayerController1_2 _p, params object[] _list);
    List<EnterSpecial> enterSpecialDels = new List<EnterSpecial>();
    //-------------------------------------------------
    public SpecialCallBack[] exitSpecialCallbacks;
    public delegate void ExitSpecial(PlayerController1_2 _p, params object[] _list);
    List<ExitSpecial> exitSpecialDels = new List<ExitSpecial>();


    PlayerController1_2 p;

    private void OnEnable()
    {
        enterBlockMaskValue = 0x0000000;
        foreach (MaskTypes m in enterBlockMasks) { enterBlockMaskValue |= (int)m; }
        if (inverseEnter) enterBlockMaskValue ^= 0xfffffff;

        exitBlockMaskValue = 0x0000000;
        foreach (MaskTypes m in exitBlockMasks) { exitBlockMaskValue |= (int)m; }
        if (inverseExit) exitBlockMaskValue ^= 0xfffffff;

        //Get all callBack Method info for Enter
        foreach (PlayerController1_2.NonParamsCallBackType type in enterCallbacks)
        {
            enterDels.Add((EnterDel)Delegate.CreateDelegate(typeof(EnterDel), null, typeof(PlayerController1_2).GetMethod(type.ToString(), BindingFlags.Public | BindingFlags.Instance)));
        }
        //Get all callBack Method info for Exit
        foreach (PlayerController1_2.NonParamsCallBackType type in exitCallbacks)
        {
            exitDels.Add((ExitDel)Delegate.CreateDelegate(typeof(ExitDel), null, typeof(PlayerController1_2).GetMethod(type.ToString(), BindingFlags.Public | BindingFlags.Instance)));
        }

        for(int i = 0;i<enterSpecialCallbacks.Length;i++)
        {
            enterSpecialDels.Add((EnterSpecial)Delegate.CreateDelegate(typeof(EnterSpecial), null, typeof(PlayerController1_2).GetMethod(enterSpecialCallbacks[i].callBackType.ToString(), BindingFlags.Public | BindingFlags.Instance)));
            int paramSize = enterSpecialCallbacks[i].ints.Length + enterSpecialCallbacks[i].floats.Length + enterSpecialCallbacks[i].bools.Length + enterSpecialCallbacks[i].vector3s.Length;
            enterSpecialCallbacks[i].objs = new object[paramSize];
            int i2 = 0;
            //ints
            for (int j = 0; j < enterSpecialCallbacks[i].ints.Length; j++)
            {
                enterSpecialCallbacks[i].objs[i2] = enterSpecialCallbacks[i].ints[j];
                i2++;
            }
            //floats
            for (int k = 0; k < enterSpecialCallbacks[i].floats.Length; k++)
            {
                enterSpecialCallbacks[i].objs[i2] = enterSpecialCallbacks[i].floats[k];
                i2++;
            }
            //bools
            for (int l = 0; l < enterSpecialCallbacks[i].bools.Length; l++)
            {
                enterSpecialCallbacks[i].objs[i2] = enterSpecialCallbacks[i].bools[l];
                i2++;
            }
            //vector3s
            for (int m = 0; m < enterSpecialCallbacks[i].vector3s.Length; m++)
            {
                enterSpecialCallbacks[i].objs[i2] = enterSpecialCallbacks[i].vector3s[m];
                i2++;
            }
        }

        for (int i = 0; i < exitSpecialCallbacks.Length; i++)
        {
            exitSpecialDels.Add((ExitSpecial)Delegate.CreateDelegate(typeof(ExitSpecial), null, typeof(PlayerController1_2).GetMethod(exitSpecialCallbacks[i].callBackType.ToString(), BindingFlags.Public | BindingFlags.Instance)));
            int paramSize = exitSpecialCallbacks[i].ints.Length + exitSpecialCallbacks[i].floats.Length + exitSpecialCallbacks[i].bools.Length + enterSpecialCallbacks[i].vector3s.Length;
            exitSpecialCallbacks[i].objs = new object[paramSize];
            int i2 = 0;
            //ints
            for (int j = 0; j < exitSpecialCallbacks[i].ints.Length; j++)
            {
                exitSpecialCallbacks[i].objs[i2] = exitSpecialCallbacks[i].ints[j];
                i2++;
            }
            //floats
            for (int k = 0; k < exitSpecialCallbacks[i].floats.Length; k++)
            {
                exitSpecialCallbacks[i].objs[i2] = exitSpecialCallbacks[i].floats[k];
                i2++;
            }
            //bools
            for (int l = 0; l < exitSpecialCallbacks[i].bools.Length; l++)
            {
                exitSpecialCallbacks[i].objs[i2] = exitSpecialCallbacks[i].bools[l];
                i2++;
            }
            //vector3s
            for (int m = 0; m < exitSpecialCallbacks[i].vector3s.Length; m++)
            {
                exitSpecialCallbacks[i].objs[i2] = exitSpecialCallbacks[i].vector3s[m];
                i2++;
            }
        }
    }

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (p != null)
        {
            if (enterBlockMasks.Length != 0) { p.ResetAnimationBlockMask(enterBlockMaskValue); }
            foreach (EnterDel de in enterDels) { de(p); }
            for (int i = 0; i < enterSpecialCallbacks.Length; i++)
            {
                enterSpecialDels[i](p, enterSpecialCallbacks[i].objs);
            }
        }
        else
        {
            p = animator.transform.GetComponent<PlayerController1_2>();
            if (!p)
            {
                Debug.LogError("No PlayerController attach to this animator's parent GameObject");
            }
            else
            {
                p.ResetAnimationBlockMask(enterBlockMaskValue);
                foreach (EnterDel de in enterDels) { de(p); }
                for (int i = 0; i < enterSpecialCallbacks.Length; i++)
                {
                    enterSpecialDels[i](p, enterSpecialCallbacks[i].objs);
                }
            }
        }

        foreach (BoolValues b in boolValus)
        {
            animator.SetBool(b.boolName, b.enterStatu);
        }
        foreach (FloatValue f in enterFloatValus)
        {
            animator.SetFloat(f.floatName, f.value);
        }
        foreach (IntValue i in enterIntValus)
        {
            animator.SetInteger(i.intName, i.value);
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (p != null)
        {
            if (exitBlockMasks.Length != 0) {p.ResetAnimationBlockMask(exitBlockMaskValue);}
            foreach (ExitDel de in exitDels) { de(p); }
            for (int i = 0; i < exitSpecialCallbacks.Length; i++)
            {
                exitSpecialDels[i](p, exitSpecialCallbacks[i].objs);
            }
        }
        else
        {
            p = animator.transform.GetComponent<PlayerController1_2>();
            if (!p)
            {
                Debug.LogError("No PlayerController attach to this animator's parent GameObject");
            }
            else
            {
                p.ResetAnimationBlockMask(exitBlockMaskValue);
                foreach (ExitDel de in exitDels) { de(p); }
                for (int i = 0; i < exitSpecialCallbacks.Length; i++)
                {
                    exitSpecialDels[i](p, exitSpecialCallbacks[i].objs);
                }
            }
        }

        foreach (BoolValues b in boolValus)
        {
            if (b.resetOnExit) { animator.SetBool(b.boolName, !b.enterStatu); }
        }
        foreach (FloatValue f in exitFloatValus)
        {
            animator.SetFloat(f.floatName, f.value);
        }
        foreach (IntValue i in exitIntValus)
        {
            animator.SetInteger(i.intName, i.value);
        }
    }

    [System.Serializable]
    public struct BoolValues
    {
        public string boolName;
        public bool enterStatu;
        public bool resetOnExit;
    }
    [System.Serializable]
    public struct FloatValue
    {
        public string floatName;
        public float value;
    }
    [System.Serializable]
    public struct IntValue
    {
        public string intName;
        public int value;
    }

    [System.Serializable]
    public struct SpecialCallBack
    {
        public PlayerController1_2.CallBackSpecialType callBackType;
        public int[] ints;
        public float[] floats;
        public bool[] bools;
        public Vector3[] vector3s;
        [HideInInspector]
        public object[] objs;
    }

    [Flags]
    public enum MaskTypes
    {
        Non = 0x0,
        blockJump = 0x1,
        blockMovement = 0x2,
        blockAttack = 0x4,
        All = 0xfffffff,
    }
}