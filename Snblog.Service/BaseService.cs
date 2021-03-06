using Snblog.IRepository;
using Snblog.IRepository.IRepository;
using Snblog.IService;

namespace Snblog.Service
{
    public class BaseService : IBaseService
    {
        private IRepositoryFactory _repositoryFactory;
        private IConcardContext _mydbcontext;
        public BaseService(IRepositoryFactory repositoryFactory, IConcardContext mydbcontext)
        {
            this._repositoryFactory = repositoryFactory;
            this._mydbcontext = mydbcontext;
        }

        public IRepositorys<T> CreateService<T>() where T : class, new()
        {
            return _repositoryFactory.CreateRepository<T>(_mydbcontext);
        }
    }
}
