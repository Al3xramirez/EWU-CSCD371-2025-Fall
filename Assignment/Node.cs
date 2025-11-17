using System;
using System.Collections;
using System.Collections.Generic;

namespace Assignment;

public class Node<T> : IEnumerable<T>
{
    public T Value { get; }
    public Node<T> Next { get; private set; }

    public Node(T value)
    {
        Value = value;
        Next = this;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public void Append(T value)
    {
        Node<T> current = this;
        do
        {
            if (object.Equals(current.Value, value))
                throw new InvalidOperationException("Duplicate value detected");
            current = current.Next;
        } while (current != this);

        var newNode = new Node<T>(value) { Next = this.Next };
        this.Next = newNode;
    }

    public void Clear()
    {
        if (this.Next == this)
            return;

        Node<T> firstRemoved = this.Next;
        Node<T> lastRemoved = firstRemoved;
        while (lastRemoved.Next != this)
        {
            lastRemoved = lastRemoved.Next;
        }

        lastRemoved.Next = firstRemoved;
        this.Next = this;
    }

    public bool Exists(T value)
    {
        Node<T> current = this;
        do
        {
            if (object.Equals(current.Value, value))
                return true;
            current = current.Next;
        } while (current != this);

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node<T> current = this;
        do
        {
            yield return current.Value;
            current = current.Next;
        }
        while (current != this);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<T> ChildItems(int maximum)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maximum, nameof(maximum));

        Node<T> current = this.Next;
        int count = 0;
        while (current != this && count < maximum)
        {
            yield return current.Value;
            current = current.Next;
            count++;
        }
    }
}

