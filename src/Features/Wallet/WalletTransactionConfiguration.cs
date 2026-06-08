using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Rotinik.Features.Wallet;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)    
    {
        builder.ToTable("WalletTransactions");
        
        // Define a chave primária
        builder.HasKey(x => x.Id);

        // Configurações básicas de segurança para as propriedades reais
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Source).IsRequired();
        
        // Limita o tamanho da string no banco para não desperdiçar espaço
        builder.Property(x => x.Description).HasMaxLength(250);
    }
}