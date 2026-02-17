using FluentAssertions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Repositories;
using Ledgerium.Application.Services;
using Ledgerium.Application.Services.RevertService;
using Ledgerium.Domain;
using Ledgerium.Domain.Abstractions;
using Ledgerium.Domain.Exceptions;
using Moq;

namespace Ledgerium.Tests;

public class RevertServiceTests
{
    [Fact]
    public async Task RevertAsync_ShouldRevertCreditTransaction()
    {
        var clientId = Guid.NewGuid();
        var txId = Guid.NewGuid();

        var balance = new Balance(clientId, 100m);
        var transaction = new CreditTransaction(
            txId, clientId, DateTimeOffset.UtcNow, 30m,
            DateTimeOffset.UtcNow, false, null);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);
        balances.Setup(x => x.Save(balance, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(txId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        transactions.Setup(x => x.Save(transaction, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Мок транзакции
        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new RevertService(balances.Object, transactions.Object, txManager.Object);

        var result = await service.RevertAsync(new RevertCommand(txId), CancellationToken.None);

        balance.Value.Should().Be(70m);
        transaction.IsReverted.Should().BeTrue();
        result.ClientBalance.Should().Be(70m);

        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevertAsync_ShouldRevertDebitTransaction()
    {
        var clientId = Guid.NewGuid();
        var txId = Guid.NewGuid();

        var balance = new Balance(clientId, 50m);
        var transaction = new DebitTransaction(
            txId, clientId, DateTimeOffset.UtcNow, 20m,
            DateTimeOffset.UtcNow, false, null);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(txId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new RevertService(balances.Object, transactions.Object, txManager.Object);

        var result = await service.RevertAsync(new RevertCommand(txId), CancellationToken.None);

        balance.Value.Should().Be(70m);
        transaction.IsReverted.Should().BeTrue();
        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevertAsync_ShouldBeIdempotent_WhenAlreadyReverted()
    {
        var clientId = Guid.NewGuid();
        var txId = Guid.NewGuid();

        var balance = new Balance(clientId, 100m);

        var transaction = new CreditTransaction(
            txId, clientId, DateTimeOffset.UtcNow, 30m,
            DateTimeOffset.UtcNow, true, DateTimeOffset.UtcNow);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(txId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);

        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new RevertService(balances.Object, transactions.Object, txManager.Object);

        var result = await service.RevertAsync(new RevertCommand(txId), CancellationToken.None);

        balance.Value.Should().Be(100m); // баланс не изменился
        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RevertAsync_ShouldThrowNotFound_WhenTransactionNotExists()
    {
        var txId = Guid.NewGuid();

        var balances = new Mock<IBalanceRepository>();
        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(txId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);

        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new RevertService(balances.Object, transactions.Object, txManager.Object);

        Func<Task> act = () => service.RevertAsync(new RevertCommand(txId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        txMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}