﻿using FluentResults;
using MediatR;
using PlexRipper.Domain;

namespace Data.Contracts;

public class GetPlexTvShowByIdQuery : IRequest<Result<PlexTvShow>>
{
    public GetPlexTvShowByIdQuery(int id, bool includePlexServer = false, bool includePlexLibrary = false)
    {
        Id = id;
        IncludePlexServer = includePlexServer;
        IncludePlexLibrary = includePlexLibrary;
    }

    public int Id { get; }

    public bool IncludePlexServer { get; }

    public bool IncludePlexLibrary { get; }
}