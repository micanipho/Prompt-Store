namespace Domain.Interfaces;

/// <summary>Abstracts the persistence mechanism so services can save all pending changes.</summary>
public interface IUnitOfWork
{
    void SaveChanges();
}
