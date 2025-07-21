using UnityEngine;
using UnityEngine.Events;

public class CheckIfInt : MonoBehaviour
{
    [SerializeField] int _targetInt;

    public UnityEvent onTrue;
    public UnityEvent onFalse;

    public int targetInt {
        get => _targetInt;
        set {
            _targetInt = value;
        }
    }

    public void Answer(int value) 
    {
        if (targetInt == value)
        {
            onTrue.Invoke();
        }
        else
        {
            onFalse.Invoke();
        }
    }
}
