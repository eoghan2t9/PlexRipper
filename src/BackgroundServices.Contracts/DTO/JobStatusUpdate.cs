namespace BackgroundServices.Contracts;

public class JobStatusUpdate
{
    public string Id { get; set; }

    public string JobName { get; set; }

    public string JobGroup { get; set; }

    public TimeSpan JobRuntime { get; set; }

    public DateTime JobStartTime { get; set; }

    public JobStatus Status { get; set; }

    public string PrimaryKey { get; set; }

    public int PrimaryKeyValue { get; set; }
}