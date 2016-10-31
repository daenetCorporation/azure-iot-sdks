using System.Threading.Tasks;

namespace IotHubCommander
{
    internal interface IHubModule
    {
        Task Execute();
    }
}