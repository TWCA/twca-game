using System;
using System.Collections;
using System.Collections.Generic;

public class MaxHeap<T> : Heap<T> where T : IComparable<T>
{
    protected override bool Compare(int indexA, int indexB)
    {
        return data[indexA].CompareTo(data[indexB]) > 0;
    }
}

public class MinHeap<T> : Heap<T> where T : IComparable<T>
{
    protected override bool Compare(int indexA, int indexB)
    {
        return data[indexA].CompareTo(data[indexB]) < 0;
    }
}

public abstract class Heap<T> where T : IComparable<T>
{
    protected List<T> data;

    public Heap()
    {
        data = new List<T>();
    }

    public void Push(T value)
    {
        data.Add(value);
        SiftUp(data.Count - 1);
    }

    public T Pop()
    {
        return RemoveAt(0);
    }

    public T Peak()
    {
        if (data.Count > 0)
            return data[0];
        else
            return default;
    }

    public void Replace(T oldElement, T newElement)
    {
        Remove(oldElement);
        Push(newElement);
    }

    public void Remove(T element)
    {
        RemoveAt(data.IndexOf(element));
    }

    private T RemoveAt(int index)
    {
        if (index < 0 || index >= data.Count)
            return default;

        Swap(index, data.Count - 1);

        // remove last
        T value = data[data.Count - 1];
        data.RemoveAt(data.Count - 1);

        SiftDown(index);

        return value;
    }

    private void SiftUp(int child)
    {
        int parent = Parent(child);

        if (parent < 0) return;

        if (Compare(child, parent))
        {
            Swap(child, parent);
            SiftUp(parent);
        }
    }

    private void SiftDown(int parent)
    {
        int left = ChildLeft(parent);
        int right = ChildRight(parent);

        if (right < data.Count)
        {
            // two children in the heap
            if (Compare(left, right))
            {
                // left is greater
                if (Compare(left, parent))
                {
                    Swap(parent, left);
                    SiftDown(left);
                }
            }
            else
            {
                // right is greater
                if (Compare(right, parent))
                {
                    Swap(parent, right);
                    SiftDown(right);
                }
            }
        }
        else if (left < data.Count)
        {
            // one child in the heap
            if (data[left].CompareTo(data[parent]) < 0)
                Swap(parent, left);
        }
        
    }

    protected abstract bool Compare(int indexA, int indexB);
    private void Swap(int indexA, int indexB)
    {
        (data[indexA], data[indexB]) = (data[indexB], data[indexA]);
    }

    private int Parent(int index)
    {
        return (index - 1) >> 1;
    }

    private int ChildLeft(int index)
    {
        return (index << 1) + 1;
    }

    private int ChildRight(int index)
    {
        return (index << 1) + 2;
    }
    
    public int Count() {
        return data.Count; 
    }
}