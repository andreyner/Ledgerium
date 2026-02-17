using FluentAssertions;
using Ledgerium.Application.Commands;
using Ledgerium.Application.Repositories;
using Ledgerium.Application.Services;
using Ledgerium.Application.Services.DebitService;
using Ledgerium.Domain;
using Ledgerium.Domain.Abstractions;
using Ledgerium.Domain.Exceptions;
using Moq;

namespace Ledgerium.Tests;

public class DebitServiceTests
{
    [Fact]
    public async Task DebitAsync_ShouldDecreaseBalance_WhenEnoughFunds()
    {
        var clientId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var balance = new Balance(clientId, 100m);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);
        balances.Setup(x => x.Save(balance, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);
        transactions.Setup(x => x.Add(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Мок самой транзакции
        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new DebitService(
            transactions.Object,
            balances.Object,
            txManager.Object);

        var result = await service.DebitAsync(
            new DebitCommand(transactionId, clientId, DateTimeOffset.UtcNow, 40m),
            CancellationToken.None);

        balance.Value.Should().Be(60m);

        txManager.Verify(x => x.BeginAsync(It.IsAny<CancellationToken>()), Times.Once);
        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        txMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task DebitAsync_ShouldThrowInsufficientFunds_WhenNotEnoughBalance()
    {
        var clientId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var balance = new Balance(clientId, 10m);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction?)null);

        // Мок транзакции
        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new DebitService(
            transactions.Object,
            balances.Object,
            txManager.Object);

        Func<Task> act = () => service.DebitAsync(
            new DebitCommand(transactionId, clientId, DateTimeOffset.UtcNow, 50m),
            CancellationToken.None);

        await act.Should().ThrowAsync<InsufficientFundsException>();

        txMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task DebitAsync_ShouldBeIdempotent_WhenTransactionAlreadyExists()
    {
        var clientId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var existingTx = new DebitTransaction(
            transactionId,
            clientId,
            DateTimeOffset.UtcNow,
            20m,
            DateTimeOffset.UtcNow,
            false,
            null);

        var balance = new Balance(clientId, 100m);

        var balances = new Mock<IBalanceRepository>();
        balances.Setup(x => x.Get(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(balance);

        var transactions = new Mock<ITransactionRepository>();
        transactions.Setup(x => x.GetById(transactionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTx);

        // Мок транзакции
        var txMock = new Mock<IAppDbTransaction>();
        txMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        txMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var txManager = new Mock<IDbTransactionManager>();
        txManager.Setup(x => x.BeginAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(txMock.Object);

        var service = new DebitService(
            transactions.Object,
            balances.Object,
            txManager.Object);

        var result = await service.DebitAsync(
            new DebitCommand(transactionId, clientId, DateTimeOffset.UtcNow, 999m),
            CancellationToken.None);

        balance.Value.Should().Be(100m); // баланс не изменился
        transactions.Verify(x => x.Add(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);

        // Транзакция была открыта и сразу закоммичена, даже если идемпотентность
        txMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        txMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}