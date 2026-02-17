using FluentAssertions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Repositories;
using Ledgerium.Application.Services;
using Ledgerium.Application.Services.CreditService;
using Ledgerium.Domain;
using Ledgerium.Domain.Abstractions;
using Moq;

namespace Ledgerium.Tests;

public class CreditServiceTests
{
    [Fact]
    public async Task CreditAsync_ShouldIncreaseBalance()
    {
        var clientId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var balance = new Balance(clientId, 50m);

        var balanceRepoMock = new Mock<IBalanceRepository>();
        balanceRepoMock.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);
        balanceRepoMock.Setup(x => x.Save(balance, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(x => x.GetById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);
        transactionRepoMock.Setup(x => x.Add(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var txManagerMock = new Mock<IDbTransactionManager>();
        txManagerMock.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new CreditService(
            txManagerMock.Object,
            transactionRepoMock.Object,
            balanceRepoMock.Object);

        var result = await service.CreditAsync(
            new CreditCommand(transactionId, clientId, DateTimeOffset.UtcNow, 30m),
            CancellationToken.None
        );

        balance.Value.Should().Be(80m);

        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        txMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}