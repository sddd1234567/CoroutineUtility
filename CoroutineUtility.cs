using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoroutineUtility : MonoBehaviour
{
    // Use this for initialization
    public static CoroutineUtility Instance;
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
        {
            Instance = new GameObject("CoroutineUtility").AddComponent<CoroutineUtility>();
            return Instance;
        }
        else
            return Instance;
    }

    public CoroutineQueue Do()
    {
        CoroutineQueue animObj = new CoroutineQueue(this);
        return animObj;
    }
}

public class CoroutineQueue
{
    Queue<IEnumerator> waitQueue;
    CoroutineUtility coroutineUtility;
    bool isStartPlaying = false;

    public CoroutineQueue(CoroutineUtility animUtility)
    {
        waitQueue = new Queue<IEnumerator>();
        coroutineUtility = animUtility;
    }

    public CoroutineQueue Play(Animator animator, string animStateName)
    {
        waitQueue.Enqueue(PlayAnimation(animator, animStateName));

        return this;
    }

    public CoroutineQueue Wait(float time)
    {
        waitQueue.Enqueue(WaitTime(time));
        return this;
    }

    public CoroutineQueue Move(GameObject obj, Vector3 newPos, float time)
    {
        waitQueue.Enqueue(MoveObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue UIMove(GameObject obj, Vector2 newPos, float time)
    {
        waitQueue.Enqueue(MoveUIObj(obj, newPos, time));
        return this;
    }

    public CoroutineQueue RadiusUIMove(GameObject obj, Vector2 newPos, float xRadius, float yRadius, float time)
    {
        waitQueue.Enqueue(RadiusMoveUIObj(obj, newPos, xRadius, yRadius, time));
        return this;
    }

    public CoroutineQueue Then(UnityAction call)
    {
        waitQueue.Enqueue(DoCall(call));
        return this;
    }

    public CoroutineQueue DoEnumerator(IEnumerator ie)
    {
        waitQueue.Enqueue(ie);
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



    IEnumerator PlayAnimation(Animator animator, string animStateName)
    {
        animator.Play(animStateName);
        yield return new WaitUntil(() => { return animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName); });
        yield return new WaitUntil(() => { return !animator.GetCurrentAnimatorStateInfo(0).IsName(animStateName); });
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

    IEnumerator MoveUIObj(GameObject obj, Vector2 newPos, float time)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 prePos = rect.anchoredPosition;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            rect.anchoredPosition = Vector2.Lerp(prePos, newPos, t);
            yield return null;
        }
    }

    IEnumerator RadiusMoveUIObj(GameObject obj, Vector2 newPos, float xRadius, float yRadius, float time)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        Vector2 prePos = rect.anchoredPosition;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            if (t > 1)
                t = 1;
            float x = Mathf.Lerp(rect.anchoredPosition.x, newPos.x, xRadius);
            float y = Mathf.Lerp(rect.anchoredPosition.y, newPos.y, yRadius);
            rect.anchoredPosition = new Vector2(x, y);
            yield return null;
        }
    }

    IEnumerator DoQueue()
    {
        while (waitQueue.Count > 0)
        {
            IEnumerator task = waitQueue.Dequeue();
            yield return coroutineUtility.StartCoroutine(task);
        }
    }

    IEnumerator DoCall(UnityAction func)
    {
        func.Invoke();
        yield return null;
    }
}
