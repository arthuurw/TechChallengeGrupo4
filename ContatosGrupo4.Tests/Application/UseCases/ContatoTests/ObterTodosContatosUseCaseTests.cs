﻿using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ContatosGrupo4.Tests.Application.UseCases.ContatoTests;

public class ObterTodosContatosUseCaseTests
{
    [Fact]
    public static async Task DeveRetornarTodosContatos()
    {
        var contatoRepository = new Mock<IContatoRepository>();
        var contatoUseCase = new ObterTodosContatosUseCase(contatoRepository.Object);
        var contatoEsperado = new Contato() { Nome = "testeContato", Email = "testeemail@google.com", CodigoArea = 32, Telefone = "99999-9999" };

        contatoRepository.Setup(r => r.GetAllContatos()).ReturnsAsync([contatoEsperado]);

        var contatos = await contatoUseCase.ExecuteAsync();

        contatos.Should().HaveCount(1);
        contatos.First().Nome.Should().Be(contatoEsperado.Nome);
        contatos.First().Email.Should().Be(contatoEsperado.Email);
        contatos.First().CodigoArea.Should().Be(contatoEsperado.CodigoArea);
        contatos.First().Telefone.Should().Be(contatoEsperado.Telefone);
    }
}