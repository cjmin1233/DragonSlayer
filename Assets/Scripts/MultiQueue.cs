using System.Collections.Generic;
public struct MultiQueue<T>
{
    private Queue<T>[] queues;

    public MultiQueue(int count)
    {
        queues = new Queue<T>[count];
        for (int i = 0; i < count; i++) queues[i] = new Queue<T>();
    }
    public void Enqueue(int index, T item)
    {
        queues[index].Enqueue(item);
    }
    public T Dequeue(int index)
    {
        return queues[index].Dequeue();
    }
    public int Count(int index)
    {
        return queues[index].Count;
    }
    public T Peek(int index)
    {
        return queues[index].Peek();
    }
}