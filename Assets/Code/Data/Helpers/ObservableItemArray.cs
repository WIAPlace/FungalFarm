using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class ObservableItemArray{
    public Item[] items;

    public event Action<Item[],int> AnyValueChanged = delegate { };
    public int Count => items.Count(i => i != null);
    public int Length => items.Length;

    //public Item this[int index] => throw new NotImplementedException();


    public ObservableItemArray(int size = 25, IList<Item> initialList = null) {
        items = new Item[size];
        if (initialList != null) {
            initialList.Take(size).ToArray().CopyTo(items, 0);
            Invoke(-1);
        }
    }

    void Invoke(int index) => AnyValueChanged.Invoke(items, index);

    public void Swap(int index1, int index2) {
        (items[index1], items[index2]) = (items[index2], items[index1]);
        Invoke(-1);
    }

    public void Clear() {
        items = new Item[items.Length];
        Invoke(-1);
    }

    public bool TryAdd(Item item) {
        for (var i = 0; i < items.Length; i++) {
            if (TryAddAt(i, item)) return true;
        }
        return false;
    }
    
    public bool TryAddAt(int index, Item item) {
        if (index < 0 || index >= items.Length) return false;
        
        if (items[index] != null && items[index].quantity != 0) return false;

        items[index] = item;
        Invoke(index);
        return true;
    }

    public bool TryRemove(Item item) {
        for (var i = 0; i < items.Length; i++) {
            if (EqualityComparer<Item>.Default.Equals(items[i], item) && TryRemoveAt(i)) return true;
        }
        return false;
    }
    
    public bool TryRemoveAt(int index) {
        if (index < 0 || index >= items.Length) return false;
        
        if (items[index] == null) return false;

        items[index] = default;
        Invoke(index);
        return true;
    }
}