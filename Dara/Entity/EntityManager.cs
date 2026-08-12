namespace Dara.Entity;

public class EntityManager
{
    private readonly HashSet<IEntity> _entities = new();
    private readonly Queue<IEntity> _removeQueue = new();
    private readonly Queue<IEntity> _addQueue = new();

    public int Count => _entities.Count;

    public void Update(float deltaTime)
    {
        foreach (var entity in _entities)
        {
            entity.Update(deltaTime, this);
        }

        HandleAddQueue();
        HandleRemoveQueue();
    }

    public void Render()
    {
        foreach (var entity in _entities)
        {
            entity.Render();
        }
    }

    public void AddEntity(IEntity entity) => _addQueue.Enqueue(entity);

    public void RemoveEntity(IEntity entity) => _removeQueue.Enqueue(entity);

    private void HandleRemoveQueue()
    {
        int queueSize = _removeQueue.Count;

        for (int i = 0; i < queueSize; i++)
        {
            _entities.Remove(_removeQueue.Dequeue());
        }
    }

    private void HandleAddQueue()
    {
        int queueSize = _addQueue.Count;

        for (int i = 0; i < queueSize; i++)
        {
            _entities.Add(_addQueue.Dequeue());
        }
    }

    public List<T> Query<T>() where T : IEntity
    {
        List<T> found = new();

        foreach (IEntity entity in _entities)
        {
            if (entity is T t)
            {
                found.Add(t);
            }
        }

        return found;
    }

    public List<T> QueryExact<T>() where T : IEntity
    {
        List<T> found = new();

        foreach (IEntity entity in _entities)
        {
            if (entity.GetType() == typeof(T))
            {
                found.Add((T)entity);
            }
        }

        return found;
    }
}