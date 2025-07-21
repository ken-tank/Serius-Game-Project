using System;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;

public class ActiveOneRandom : MonoBehaviour
{
    [SerializeField] GameObject[] objects;

    public int resultNumber;
    public UnityEvent<int> result;

    void Awake()
    {
        if (objects.Length == 0) return;

        var random = new RandomLCG(gameObject.GetInstanceID() + DateTime.Now.Ticks);
        var rng = random.Next(0, objects.Length);
        resultNumber = rng;
        result?.Invoke(resultNumber);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == rng);
        }
    }
}
