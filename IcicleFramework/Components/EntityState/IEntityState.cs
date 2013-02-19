namespace IcicleFramework.Components.EntityState
{
    public interface IEntityState
    {
        string Name { get; set; }

        IEntityStateObject Value { get; set; }
    }
}
