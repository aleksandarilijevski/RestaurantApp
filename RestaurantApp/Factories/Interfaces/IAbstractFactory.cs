namespace RestaurantApp.Factories.Interfaces
{
    public interface IAbstractFactory<T>
    {
        public T Create();
    }
}
