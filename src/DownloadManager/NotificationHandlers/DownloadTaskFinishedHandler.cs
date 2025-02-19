using DownloadManager.Contracts;
using FileSystem.Contracts;

namespace PlexRipper.DownloadManager;

public class DownloadTaskFinishedHandler : INotificationHandler<DownloadTaskFinished>
{
    private readonly IMediator _mediator;
    private readonly IFileMergeScheduler _fileMergeScheduler;

    public DownloadTaskFinishedHandler(IMediator mediator, IFileMergeScheduler fileMergeScheduler)
    {
        _mediator = mediator;
        _fileMergeScheduler = fileMergeScheduler;
    }

    public async Task Handle(DownloadTaskFinished notification, CancellationToken cancellationToken)
    {
        var addFileTaskResult = await _fileMergeScheduler.CreateFileTaskFromDownloadTask(notification.DownloadTaskId);
        if (addFileTaskResult.IsFailed)
        {
            addFileTaskResult.LogError();
            return;
        }

        await _fileMergeScheduler.StartFileMergeJob(addFileTaskResult.Value.Id);
        await _mediator.Publish(new CheckDownloadQueue(addFileTaskResult.Value.DownloadTask.PlexServerId), cancellationToken);
    }
}