namespace Pepper.Core.Extensions;

public static class QueueExtensions
{
    public static TItem[] DequeueMultiple<TItem>(this Queue<TItem> queue, int count)
    {
        if (queue.Count < count)
            throw new InvalidOperationException("Not enough items in the queue to dequeue the requested number of items.");

        var items = new List<TItem>(count);
        for (int i = 0; i < count; i++)
        {
            items.Add(queue.Dequeue());
        }

        return items.ToArray();
    }
}
