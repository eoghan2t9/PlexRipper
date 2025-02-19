﻿using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using PlexRipper.Data.Common;

namespace PlexRipper.Data.FolderPaths;

public class GetFolderPathByIdQueryValidator : AbstractValidator<GetFolderPathByIdQuery>
{
    public GetFolderPathByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetFolderPathByIdQueryHandler : BaseHandler, IRequestHandler<GetFolderPathByIdQuery, Result<FolderPath>>
{
    public GetFolderPathByIdQueryHandler(ILog log, PlexRipperDbContext dbContext) : base(log, dbContext) { }

    public async Task<Result<FolderPath>> Handle(GetFolderPathByIdQuery request, CancellationToken cancellationToken)
    {
        var folderPath = await _dbContext.FolderPaths.FindAsync(request.Id);
        if (folderPath == null)
            return ResultExtensions.EntityNotFound(nameof(FolderPath), request.Id);

        return Result.Ok(folderPath);
    }
}