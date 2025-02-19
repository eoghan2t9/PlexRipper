﻿using Application.Contracts;
using BackgroundServices.Contracts;
using Data.Contracts;
using DownloadManager.Contracts;
using FileSystem.Contracts;
using Microsoft.EntityFrameworkCore;
using PlexRipper.DownloadManager;

namespace DownloadManager.UnitTests;

public class DownloadCommands_StopDownloadTasksAsync_UnitTests : BaseUnitTest<DownloadCommands>
{
    public DownloadCommands_StopDownloadTasksAsync_UnitTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task ShouldHaveFailedResult_WhenGivenAnInvalidId()
    {
        // Arrange

        // Act
        var result = await _sut.StopDownloadTasks(0);

        // Assert
        result.IsFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldHaveFailedResult_WhenGetAllRelatedDownloadTaskIdsFails()
    {
        // Arrange
        mock.Mock<INotificationsService>().Setup(x => x.SendResult(It.IsAny<Result>())).ReturnsAsync(Result.Ok());
        mock.Mock<INotificationsService>().Setup(x => x.SendResult(It.IsAny<Result<DownloadTask>>())).ReturnsAsync(Result.Ok());

        mock.SetupMediator(It.IsAny<GetDownloadTaskByIdQuery>).ReturnsAsync(Result.Fail(""));

        // Act
        var result = await _sut.StopDownloadTasks(1);

        // Assert
        result.IsFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldHaveSetDownloadTasksToStopped_WhenAtLeastOneValidIdIsGiven()
    {
        // Arrange
        Seed = 9999;
        await SetupDatabase(config =>
        {
            config.PlexServerCount = 1;
            config.PlexLibraryCount = 1;
            config.MovieCount = 5;
            config.MovieDownloadTasksCount = 2;
        });

        var downloadTasks = await DbContext.DownloadTasks.ToListAsync();
        downloadTasks = downloadTasks.Flatten(x => x.Children).ToList();
        var downloadTaskIds = downloadTasks.Select(x => x.Id).ToList();

        mock.Mock<IDownloadTaskScheduler>().Setup(x => x.StopDownloadTaskJob(It.IsAny<int>())).ReturnOk();
        mock.SetupMediator(It.IsAny<GetDownloadTaskByIdQuery>).ReturnsAsync(Result.Ok(downloadTasks.First()));
        mock.SetupMediator(It.IsAny<DownloadTaskUpdated>).Returns(Task.CompletedTask);
        mock.SetupMediator(It.IsAny<GetDownloadTaskByIdQuery>)
            .ReturnsAsync((GetDownloadTaskByIdQuery x, CancellationToken _) =>
                Result.Ok(downloadTasks.FirstOrDefault(y => y.Id == x.Id)));
        mock.PublishMediator(It.IsAny<CheckDownloadQueue>).Returns(Task.CompletedTask);
        mock.Mock<IDirectorySystem>().Setup(x => x.DeleteAllFilesFromDirectory(It.IsAny<string>())).Returns(Result.Ok());

        // Act
        var result = await _sut.StopDownloadTasks(downloadTaskIds.First());

        // Assert
        result.IsSuccess.ShouldBeTrue();
        mock.Mock<IDownloadTaskScheduler>()
            .Verify(x => x.StopDownloadTaskJob(It.IsAny<int>()), Times.Once);
        mock.VerifyMediator(It.IsAny<DownloadTaskUpdated>, Times.Once);
        mock.VerifyNotification(It.IsAny<CheckDownloadQueue>, Times.Once);
    }
}