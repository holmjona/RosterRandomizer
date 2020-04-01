using System;
using System.Collections.Generic;
using System.Text;


static class ObjectExtensions {

    public static void AddUnique<K,T>(this Dictionary<K,T> dict,K key,T value) {
        if (!dict.ContainsKey(key)) {
            dict.Add(key, value);
        }
    }

    public static K GetKeyAtIndex<K, T>(this Dictionary<K, T> dict,int index) {
        int curIndex = 0;
        K foundKey = default(K);
        foreach (K key in dict.Keys) {
            if (index == curIndex) {
                foundKey = key;
                break;
            }
            curIndex++;
        }
        return foundKey;
        
    }
    }

