using AutoMapper;
using Ledgerium.Domain;
using Ledgerium.Domain.Abstractions;
using Ledgerium.Infrastructure.Entities;

namespace Ledgerium.Infrastructure.Mapping;

public sealed class TransactionProfile: Profile
{
    public TransactionProfile()
    {
        CreateMap<TransactionEntity, Transaction>()
            .ConvertUsing(src => MapToDomain(src));
        CreateMap<Transaction, TransactionEntity>(MemberList.Destination);
    }
    
    private static Transaction MapToDomain(TransactionEntity src)
    {
        return src.Type switch
        {
            (int)TransactionType.Credit => new CreditTransaction(
                src.Id,
                src.ClientId,
                src.OccurredAt,
                src.Amount,
                src.InsertedAt,
                src.IsReverted,
                src.RevertedAt
            ),
            (int)TransactionType.Debit => new DebitTransaction(
                src.Id,
                src.ClientId,
                src.OccurredAt,
                src.Amount,
                src.InsertedAt,
                src.IsReverted,
                src.RevertedAt
            ),
            _ => throw new InvalidOperationException($"Unknown transaction type {src.Type}")
        };
    }
}