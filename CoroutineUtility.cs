using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineUtility : MonoBehaviour
{
    public static CoroutineUtility Instance;    //Singleton
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static CoroutineUtility GetInstance()
    {
        if (Instance == null)
            Instance = new GameObject("CoroutineUtility").AddComponent<CoroutineUtility>();
        return Instance;
    }
    
    //建立CoroutineQueue回傳並開始排序工作
    public CoroutineQueue Do()
    {
        CoroutineQueue animObj = new CoroutineQueue(this);
        return animObj;
    }
}

public class CoroutineQueue
{
    List<IEnumerator> waitQueue;
    CoroutineUtility coroutineUtility;
    bool isStartPlaying = false;

    public CoroutineQueue(CoroutineUtility utility)
    {
        waitQueue = new List<IEnumerator>();
        coroutineUtility = utility;
    }

    public CoroutineQueue Play(Animator animator, string animStateName)
    {
        waitQueue.Add(PlayAnimation(animator, animStateName));
        return this;
    }

    public CoroutineQueue Wait(float time)
    {
        waitQueue.Add(WaitTime(time));
        return this;
    }

    public CoroutineQueue Move(GameObject obj, Vector3 newPos, float time)
    {
        waitQueue.Add(MoveObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue UIMove(GameObject obj, Vector2 newPos, float time)
    {
        waitQueue.Add(MoveUIObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue Then(UnityAction call)
    {
        waitQueue.Add(DoCall(call));
        return this;
    }

    public CoroutineQueue DoEnumerator(IEnumerator ie)
    {
        waitQueue.Add(ie);
        return this;
    }

    public CoroutineQueue WaitUntil(Func<bool> require)
    {
        waitQueue.Add(WaitUntilReqTrue(require));
        return this;
    }

    public void Go()
    {
        if (!isStartPlaying)
        {
            isStartPlaying = true;
            coroutineUtility.StartCoroutine(DoQueue());
        }
    }

    IEnumerator WaitUntilReqTrue(Func<bool> require)
    {
        yield return new WaitUntil(() => { return require(); });
    }

    IEnumerator PlayAnimation(Animator animator, string animStateName)
    {
        if (animator != null)
        {
            animator.Play(animStateName);
            yield return new WaitUntil(() => {
                if (animator != null && animator.enabled)
                    return animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName);
                else
                    return true;
            });
            yield return new WaitUntil(() => {
                if (animator != null && animator.enabled)
                    return !animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName);
                else
                    return true;
            });
        }
    }

    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    IEnumerator MoveObj(GameObject obj, Vector3 newPos, float time)
    {
        Vector3 prePos = obj.transform.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            obj.transform.position = Vector3.Lerp(prePos, newPos, t);
            yield return null;
        }
    }

    IEnumerator DoQueue()
    {
        while (waitQueue.Count > 0)
        {
            yield return coroutineUtility.StartCoroutine(waitQueue[0]);
            waitQueue.RemoveAt(0);
        }
    }

    IEnumerator DoCall(UnityAction func)
    {
        func.Invoke();
        yield return null;
    }
}
