namespace LoginMVC.Domain.Entities;

public class Entity<TEntityId>
{
    protected Entity() { }
    protected Entity(TEntityId id)
    {
        Id = id;
    }
    public TEntityId? Id { get; init; }
}