namespace Labeling
{
    public class log
    {
        public static void writeLog(string detail)
        {
            Serilog.Log.Information(detail);
            Serilog.Log.CloseAndFlush();
        }
    }
}
