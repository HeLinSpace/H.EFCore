namespace H.EF.Core
{
    public interface IBaseModel<TKey>
    {
        TKey Id { get; set; }
    }
}
