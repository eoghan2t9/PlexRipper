﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Data.Common;

namespace PlexRipper.Data;

public class UpdateRootDownloadStatusOfDownloadTaskCommandHandlerValidator : AbstractValidator<UpdateRootDownloadStatusOfDownloadTaskCommand>
{
    public UpdateRootDownloadStatusOfDownloadTaskCommandHandlerValidator()
    {
        RuleFor(x => x.RootDownloadTaskId).GreaterThan(0);
    }
}

public class UpdateRootDownloadStatusOfDownloadTaskCommandHandler : BaseHandler,
    IRequestHandler<UpdateRootDownloadStatusOfDownloadTaskCommand, Result>
{
    public UpdateRootDownloadStatusOfDownloadTaskCommandHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result> Handle(UpdateRootDownloadStatusOfDownloadTaskCommand command, CancellationToken cancellationToken)
    {
        var downloadTaskDb = await DownloadTasksQueryable
            .IncludeDownloadTasks()
            .FirstOrDefaultAsync(x => x.Id == command.RootDownloadTaskId, cancellationToken);

        void SetDownloadStatus(DownloadTask downloadTask)
        {
            if (downloadTask.Children is null || !downloadTask.Children.Any())
                return;

            foreach (var downloadTaskChild in downloadTask.Children)
                SetDownloadStatus(downloadTaskChild);

            downloadTask.DownloadStatus = DownloadTaskActions.Aggregate(downloadTask.Children.Select(x => x.DownloadStatus).ToList());
        }

        SetDownloadStatus(downloadTaskDb);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}