namespace NerdStore.Vendas.Domain.Tests
{
   public class VoucherTests
   {
      [Fact(DisplayName = "Validar Voucher Tipo Valor Valido")]
      [Trait("Categoria", "Vendas - Voucher")]
      public void Voucher_ValidarVoucherTipoValor_DeveEstarValido()
      {
         // Arrange
         var voucher = new Voucher("PROMO-15-REAIS", null, 15, ETipoDescontoVoucher.Valor, 1, DateTime.Now.AddDays(15), true, false);

         // Act
         var resultado = voucher.ValidarSeAplicavel();

         // Assert
         Assert.True(resultado.IsValid);
      }

      [Fact(DisplayName = "Validar Voucher Tipo Valor Invalido")]
      [Trait("Categoria", "Vendas - Voucher")]
      public void Voucher_ValidarVoucherTipoValor_DeveEstarInvalido()
      {
         // Arrange
         var voucher = new Voucher("", null, null, ETipoDescontoVoucher.Valor, 0, DateTime.Now.AddDays(-1), false, true);

         // Act
         var resultado = voucher.ValidarSeAplicavel();

         // Assert
         Assert.False(resultado.IsValid);
         Assert.Equal(6, resultado.Errors.Count);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.CodigoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.DataValidadeErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.AtivoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.UtilizadoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.QuantidadeErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.ValorDescontoErroMsg);
      }

      [Fact(DisplayName = "Validar Voucher Percentual Valido")]
      [Trait("Categoria", "Vendas - Voucher")]
      public void Voucher_ValidarVoucherPorcentagem_DeveEstarValido()
      {
         // Arrange
         var voucher = new Voucher("PROMO-15-PORCENTO", 15, null, ETipoDescontoVoucher.Percentual, 1, DateTime.Now.AddDays(15), true, false);

         // Act
         var resultado = voucher.ValidarSeAplicavel();

         // Assert
         Assert.True(resultado.IsValid);
      }

      [Fact(DisplayName = "Validar Voucher Percentual Invalido")]
      [Trait("Categoria", "Vendas - Voucher")]
      public void Voucher_ValidarVoucherPorcentagem_DeveEstarInvalido()
      {
         // Arrange
         var voucher = new Voucher("", null, null, ETipoDescontoVoucher.Percentual, 0, DateTime.Now.AddDays(-1), false, true);

         // Act
         var resultado = voucher.ValidarSeAplicavel();

         // Assert
         Assert.False(resultado.IsValid);
         Assert.Equal(6, resultado.Errors.Count);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.CodigoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.DataValidadeErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.AtivoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.UtilizadoErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.QuantidadeErroMsg);
         Assert.Contains(resultado.Errors, e => e.ErrorMessage == VoucherAplicavelValidation.PercentualDescontoErroMsg);
      }
   }
}
