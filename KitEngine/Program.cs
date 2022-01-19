namespace KitEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Core core = new Core())
            {
                core.Run();
            }
        }
    }
}
