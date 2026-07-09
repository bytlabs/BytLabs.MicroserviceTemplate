using AutoMapper;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateEntityDef;
using BytLabs.MicroserviceTemplate.Application.Commands.RemoveEntityDef;
using BytLabs.MicroserviceTemplate.Application.Commands.UpdateEntityDef;
using BytLabs.MicroserviceTemplate.Application.MappingProfiles;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;
using FluentAssertions;
using Moq;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Unit.Application;

public class EntityDefCommandHandlerTests
{
    private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<EntityDefMappingProfile>()).CreateMapper();
    private readonly Mock<IRepository<EntityDef, Guid>> _repo = new();

    private static FormDataSchema Form() => new("product",
        new DataSchema("json", "{}"), new DataSchema("rjsf/formSchema", "{\"type\":\"object\"}"), new DataSchema("rjsf/uiSchema", "{}"));
    private static TableDataSchema Table() => new(
        new DataSchema("json", "{}"), new DataSchema("tanstack/columnDef", "[]"), new DataSchema("json", "{}"), new DataSchema("cms/view", "{}"));

    [Fact]
    public async Task Create_inserts_and_returns_dto()
    {
        _repo.Setup(r => r.InsertAsync(It.IsAny<EntityDef>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((EntityDef d, CancellationToken _) => d);
        var handler = new CreateEntityDefCommandHandler(_repo.Object, _mapper);

        var dto = await handler.Handle(new CreateEntityDefCommand(Guid.NewGuid(), "Product", Form(), Table()), CancellationToken.None);

        dto.EntityType.Should().Be("Product");
        _repo.Verify(r => r.InsertAsync(It.IsAny<EntityDef>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_loads_updates_and_returns_dto()
    {
        var existing = EntityDef.Create(new(Guid.NewGuid(), "Product", Form(), Table()));
        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<EntityDef>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((EntityDef d, CancellationToken _) => d);
        var handler = new UpdateEntityDefCommandHandler(_repo.Object, _mapper);

        var dto = await handler.Handle(new UpdateEntityDefCommand(existing.Id, Form(), Table()), CancellationToken.None);

        dto.Id.Should().Be(existing.Id);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<EntityDef>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remove_soft_deletes_and_returns_dto()
    {
        var existing = EntityDef.Create(new(Guid.NewGuid(), "Product", Form(), Table()));
        _repo.Setup(r => r.GetByIdAsync(existing.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<EntityDef>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((EntityDef d, CancellationToken _) => d);
        var handler = new RemoveEntityDefCommandHandler(_repo.Object, _mapper);

        var dto = await handler.Handle(new RemoveEntityDefCommand(existing.Id), CancellationToken.None);

        existing.IsDeleted.Should().BeTrue();
        dto.Id.Should().Be(existing.Id);
    }
}
