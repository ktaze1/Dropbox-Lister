using System.Threading.Tasks;

namespace Dropbox_Lister.Services.AuthServices
{
    public interface IAuthService
    {
        Task GetCode(string appkey);
        Task SetBearerToken(string appCode);
        void AddSelectUserHeader();
    }
}
