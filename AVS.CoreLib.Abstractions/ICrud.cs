namespace AVS.CoreLib.Abstractions
{
    public interface ICrud<TEntity>
    {
        TEntity GetById(int id);
        void Update(TEntity entity);
        void Insert(TEntity entity);
        void Delete(TEntity entity);
    }
}