namespace HEF.XTask
{
    public class XDelayTask<TParams> : XTask<TParams>
    {
        public int DelaySeconds { get; set; }
    }
}
