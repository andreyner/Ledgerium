using FluentAssertions;
using Ledgerium.Domain;
using Ledgerium.Domain.Exceptions;

namespace Ledgerium.Tests;

public class BalanceTests
{
    [Fact]
    public void Credit_ShouldIncreaseValue()
    {
        var balance = new Balance(Guid.NewGuid(), 100m);
        balance.Credit(50m);
        balance.Value.Should().Be(150m);
    }

    [Fact]
    public void Credit_NegativeAmount_ShouldThrow()
    {
        var balance = new Balance(Guid.NewGuid(), 100m);
        Action act = () => balance.Credit(-10m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Debit_ShouldDecreaseValue()
    {
        var balance = new Balance(Guid.NewGuid(), 100m);
        balance.Debit(30m);
        balance.Value.Should().Be(70m);
    }

    [Fact]
    public void Debit_TooMuch_ShouldThrowInsufficientFunds()
    {
        var balance = new Balance(Guid.NewGuid(), 50m);
        Action act = () => balance.Debit(100m);
        act.Should().Throw<InsufficientFundsException>();
    }

    [Fact]
    public void ApplyRevert_Credit_ShouldDecreaseValue()
    {
        var balance = new Balance(Guid.NewGuid(), 100m);
        balance.ApplyRevert(30m, TransactionType.Credit);
        balance.Value.Should().Be(70m);
    }

    [Fact]
    public void ApplyRevert_Debit_ShouldIncreaseValue()
    {
        var balance = new Balance(Guid.NewGuid(), 100m);
        balance.ApplyRevert(20m, TransactionType.Debit);
        balance.Value.Should().Be(120m);
    }
}