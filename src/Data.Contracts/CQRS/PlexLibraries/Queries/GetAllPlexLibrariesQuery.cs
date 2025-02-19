﻿using FluentResults;
using MediatR;
using PlexRipper.Domain;

namespace Data.Contracts;

public class GetAllPlexLibrariesQuery : IRequest<Result<List<PlexLibrary>>>
{
    /// <summary>
    /// Retrieves all the <see cref="PlexLibrary">PlexLibraries </see> from the database.
    /// </summary>
    /// <param name="includePlexServer">Should the <see cref="PlexServer"/> of each library be included too.</param>
    public GetAllPlexLibrariesQuery(bool includePlexServer = false)
    {
        IncludePlexServer = includePlexServer;
    }

    public bool IncludePlexServer { get; }
}