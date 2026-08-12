namespace Dara.Entity;

public interface IEntity
{
    public void Update(float deltaTime, EntityManager manager);
    public void Render();
}