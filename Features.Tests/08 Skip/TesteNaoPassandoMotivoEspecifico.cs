﻿namespace Features.Tests
{
    public class TesteNaoPassandoMotivoEspecifico
    {
        [Fact(DisplayName = "Novo Cliente 2.0", /*Para pular testes utilizar skip*/Skip = "Nova versão 2.0 quebrando")]
        [Trait("Categoria", "Escapando dos Testes")]
        public void Teste_NaoEstaPassando_VersaoNovaNaoCompativel()
        {
            Assert.True(false);
        }
    }
}