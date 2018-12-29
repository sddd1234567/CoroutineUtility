# CoroutineUtility

**How To Use?**

1. Put CoroutineUtility.cs to your unity project.
2. Call CoroutineUtility.GetInstance().Do() at the beginning.
3. Call Go() at the end.

**Play "Walk" Animation After 2 Seconds**
```csharp==
CoroutineUtility.GetInstance().Do()
    .Wait(2)
    .Play("Walk", animator)
    .Go();
```

**Play "Jump" Animation After "Walk" animation done**
```csharp==
CoroutineUtility.GetInstance().Do()
    .Play("Walk", animator)
    .Play("Jump", animator)
    .Go();
```

**Move gameobject to (0,0,1), and then call the function MoveComplete() **
```csharp==
CoroutineUtility.GetInstance().Do()
    .Move(obj,)
    .Play("Jump", animator)
    .Go();
```
