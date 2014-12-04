
namespace CbwSever
{
    internal static class CbwContextFactory
    {
        private static ICbwContext InMemory = new InMemoryCbwContext();

        public static ICbwContext CreateCbwContext()
        {
            return InMemory;
        }
    }
}