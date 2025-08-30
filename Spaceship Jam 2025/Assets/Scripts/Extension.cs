using UnityEngine;
using System.Collections.Generic;
using System;

public class Extension : MonoBehaviour
{
    public static T GetWeighedRandom<T>(List<T> objectList, Func<T, float> keyFunc)
    {
        if (objectList == null || keyFunc == null)
        {
            return default;
        }

        if (objectList.Count == 0)
        {
            return default;
        }

        List<float> weights = new List<float>();

        foreach (T obj in objectList)
        {
            if (obj == null)
            {
                continue;
            }

            weights.Add(keyFunc(obj));
        }

        return GetWeighedRandom(objectList, weights);
    }


    public static T GetWeighedRandom<T>(List<T> objectList, List<float> weights)
    {
        if (objectList == null || weights == null)
        {
            return default;
        }

        if (objectList.Count != weights.Count)
        {
            return default;
        }

        if (objectList.Count == 0)
        {
            return default;
        }

        int indexChosen = ChooseWeighedRandomIndexFromList(weights);

        return objectList[indexChosen];
    }


    public static int ChooseWeighedRandomIndexFromList(List<float> weights)
    {
        if (weights == null)
        {
            return 0;
        }

        if (weights.Count <= 1)
        {
            return 0;
        }

        float totalWeights = 0;

        foreach (float weight in weights)
        {
            totalWeights += weight;
        }

        float currentWeight = 0;
        float randomizedValue = UnityEngine.Random.Range(0f, totalWeights);

        for (int i = 0; i < weights.Count; i++)
        {
            float selectedWeight = weights[i];

            if (randomizedValue >= currentWeight && randomizedValue < currentWeight + selectedWeight)
            {
                return i;
            }

            currentWeight += selectedWeight;
        }

        Debug.LogError("Logic failure happened somewhere! ChooseWeighedRandomIndexFromList should never come here!");

        return 0;
    }

}
