using System.Threading.Tasks;


namespace ReverseGeocode.Processors
{
    public interface IProcessor
    {
        Task ProcessAsync();
    }
}