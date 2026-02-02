using System;
using System.Collections.Generic;

/**
 * Data structure used to quickly retrieve the largest (last by order) item inside
 */
public class MaxHeap<T> : Heap<T> where T : IComparable<T>
{
    protected override bool Compare(int indexA, int indexB)
    {
        return data[indexA].CompareTo(data[indexB]) > 0;
    }
}

/**
 * Data structure used to quickly retrieve the smallest (first by order) item inside
 */
public class MinHeap<T> : Heap<T> where T : IComparable<T>
{
    protected override bool Compare(int indexA, int indexB)
    {
        return data[indexA].CompareTo(data[indexB]) < 0;
    }
}

/**
 * Data structure used to quickly retrieve items in order
 */
public abstract class Heap<T> where T : IComparable<T>
{
    protected List<T> data = new();

    /**
     * Add some data to the heap.
     * <remarks>O(log n) time</remarks>
     */
    public void Push(T value)
    {
        data.Add(value);
        SiftUp(data.Count - 1);
    }

    /**
     * Remove from the top of the heap.
     * <remarks>O(log n) time</remarks>
     */
    public T Pop()
    {
        return RemoveAt(0);
    }

    /**
     * Look at the top of the heap without removing
     * <remarks>O(1) time</remarks>
     */
    public T Peak()
    {
        if (data.Count > 0)
            return data[0];
        else
            return default;
    }

    /**
     * Find an old element and replace it with a new copy, adjusting heap accordingly.
     * <remarks>O(n) time</remarks>
     */
    public void Replace(T oldElement, T newElement)
    {
        Remove(oldElement);
        Push(newElement);
    }

    /**
     * Remove an element given it's exact value.
     * <remarks>O(n) time</remarks>
     */
    public void Remove(T element)
    {
        RemoveAt(data.IndexOf(element));
    }

    /**
     * Remove an element at some index. Shifting heap accordingly.
     * <remarks>O(log n) time</remarks>
     */
    private T RemoveAt(int index)
    {
        if (index < 0 || index >= data.Count)
            return default;

        Swap(index, data.Count - 1);

        // remove last
        T value = data[^1];
        data.RemoveAt(data.Count - 1);

        SiftDown(index);

        return value;
    }

    /**
     * Allow an element to rise in the heap to its current position
     * <remarks>O(log n) time</remarks>
     */
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

    /**
     * Allow an element to sink in the heap to its current position
     * <remarks>O(log n) time</remarks>
     */
    private void SiftDown(int parent)
    {
        int left = LeftChild(parent);
        int right = RightChild(parent);

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

    /**
     * Compare two elements from given indexes.
     */
    protected abstract bool Compare(int indexA, int indexB);
    
    /**
     * Swaps two indexes in the heap.
     */
    private void Swap(int indexA, int indexB)
    {
        (data[indexA], data[indexB]) = (data[indexB], data[indexA]);
    }

    /**
     * Gets the index of the parent, given the child's index.
     */
    private int Parent(int index)
    {
        return (index - 1) >> 1;
    }

    /**
     * Gets the index of the left child, given the child's parents.
     */
    private int LeftChild(int index)
    {
        return (index << 1) + 1;
    }

    /**
     * Gets the index of the right child, given the child's parents.
     */
    private int RightChild(int index)
    {
        return (index << 1) + 2;
    }
    
    /**
     * Gets the number of values in the heap.
     */
    public int Count() {
        return data.Count; 
    }
}