namespace IcicleFramework.Components.EntityState
{
    public interface IEntityStateModifier
    {
        bool RequestEntityStateChanged(IEntityState newState);
    }
}
