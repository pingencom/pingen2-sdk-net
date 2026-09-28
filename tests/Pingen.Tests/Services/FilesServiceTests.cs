using System.Net;
using Bogus;
using Pingen.Abstractions.Enums.Api;
using Pingen.Abstractions.Models.Api;
using Pingen.Abstractions.Models.Api.Embedded.DataResults;
using Pingen.Abstractions.Models.Files;
using Pingen.Interfaces.Connectors;
using Pingen.Tests.Helpers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace Pingen.Tests.Services;

[TestFixture]
public sealed class FilesServiceTests : EndpointTestBase
{
    private static readonly Faker _faker = new();

    private FileUploadData BuildUploadData(string path)
    {
        return new FileUploadData
        {
            Id = Guid.NewGuid().ToString(),
            Type = "file_uploads",
            Attributes = new FileUpload($"{Server.Url}{path}", _faker.Random.AlphaNumeric(32), DateTime.UtcNow.AddHours(1)),
        };
    }

    [Test]
    public async Task GetPath_ShouldReturnUploadUrlAndSignature()
    {
        string fileUploadId = Guid.NewGuid().ToString();

        Server.StubJsonGet("/file-upload", PingenResponseFactory.FileUploadPath(fileUploadId));

        ApiResult<SingleResult<FileUploadData>> result = await Client.Files.GetPath();

        result.ShouldSatisfyAllConditions(
            () => result.IsSuccess.ShouldBeTrue(),
            () => result.Data.ShouldNotBeNull(),
            () => result.Data!.Data.Id.ShouldBe(fileUploadId),
            () => result.Data!.Data.Attributes.Url.ShouldNotBeNullOrEmpty(),
            () => result.Data!.Data.Attributes.UrlSignature.ShouldNotBeNullOrEmpty()
        );
    }

    [TestCase(401)]
    [TestCase(403)]
    [TestCase(500)]
    [TestCase(503)]
    public async Task GetPath_OnApiError_ShouldReturnUnsuccessfulApiResult(int statusCode)
    {
        Server.StubError("/file-upload", "GET", PingenResponseFactory.ErrorResponse(status: statusCode.ToString()), statusCode);

        ApiResult<SingleResult<FileUploadData>> result = await Client.Files.GetPath();

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.ApiError.ShouldNotBeNull(), () => result.Data.ShouldBeNull());
    }

    [Test]
    public async Task UploadFile_ShouldPutFileToUploadUrl()
    {
        string uploadPath = $"/upload/{Guid.NewGuid()}";

        Server.Given(Request.Create().WithPath(uploadPath).UsingPut()).RespondWith(Response.Create().WithStatusCode(200));

        FileUploadData fileUploadData = BuildUploadData(uploadPath);
        using var stream = new MemoryStream(_faker.Random.Bytes(128));

        ExternalRequestResult result = await Client.Files.UploadFile(fileUploadData, stream);

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeTrue(), () => result.StatusCode.ShouldBe(HttpStatusCode.OK));
    }

    [TestCase(400, HttpStatusCode.BadRequest)]
    [TestCase(403, HttpStatusCode.Forbidden)]
    [TestCase(404, HttpStatusCode.NotFound)]
    [TestCase(500, HttpStatusCode.InternalServerError)]
    [TestCase(503, HttpStatusCode.ServiceUnavailable)]
    public async Task UploadFile_ShouldReturnErrorWhenS3Fails(int statusCode, HttpStatusCode expected)
    {
        string uploadPath = $"/upload/{Guid.NewGuid()}";

        Server.Given(Request.Create().WithPath(uploadPath).UsingPut()).RespondWith(Response.Create().WithStatusCode(statusCode));

        FileUploadData fileUploadData = BuildUploadData(uploadPath);
        using var stream = new MemoryStream(_faker.Random.Bytes(128));

        ExternalRequestResult result = await Client.Files.UploadFile(fileUploadData, stream);

        result.ShouldSatisfyAllConditions(() => result.IsSuccess.ShouldBeFalse(), () => result.StatusCode.ShouldBe(expected));
    }
}
