﻿using FluentResults;
using MediatR;
using PlexRipper.Domain;

namespace Data.Contracts;

public class GetDownloadTaskByMediaKeyQuery : IRequest<Result<DownloadTask>>
{
    public GetDownloadTaskByMediaKeyQuery(int plexServerId, int mediaKey)
    {
        PlexServerId = plexServerId;
        MediaKey = mediaKey;
    }

    public int PlexServerId { get; }

    public int MediaKey { get; }
}