﻿using ContatosGrupo4.Api.Controllers;
using ContatosGrupo4.Application.Configurations;
using ContatosGrupo4.Application.DTOs;
using ContatosGrupo4.Application.Interfaces;
using ContatosGrupo4.Application.UseCases.Contatos;
using ContatosGrupo4.Domain.Entities;
using ContatosGrupo4.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace ContatosGrupo4.Tests.Unit.Controllers
{
    public class ContatoControllerUnitTests
    {
        private readonly Mock<IContatoRepository> _repository;
        private readonly Mock<IMessagePublisher> _messagePublisher;
        private readonly ContatoController _controller;

        public ContatoControllerUnitTests()
        {
            var options = new RabbitMQOptions
            {
                HostName = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                Queues = new RabbitMQQueues
                {
                    CriarContato = "contato-criar-queue",
                    AtualizarContato = "contato-atualizar-queue",
                    ExcluirContato = "contato-excluir-queue"
                }
            };

            var rabbitMqOptions = Options.Create(options);

            _repository = new Mock<IContatoRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            var cache = new Mock<IMemoryCache>();
            var obterTodosContatosUseCase = new ObterTodosContatosUseCase(_repository.Object, cache.Object);
            var obterContatoPorIdUseCase = new ObterContatoPorIdUseCase(_repository.Object, cache.Object);
            var obterContatoPorNomeEmailUseCase = new ObterContatoPorNomeEmailUseCase(_repository.Object);
            var criarContatoUseCase = new CriarContatoUseCase(
                obterContatoPorNomeEmailUseCase, 
                cache.Object, 
                _messagePublisher.Object, 
                rabbitMqOptions);
            var atualizarContatoUseCase = new AtualizarContatoUseCase(
                obterContatoPorIdUseCase, 
                cache.Object,
                _messagePublisher.Object,
                rabbitMqOptions);
            var excluirContatoUseCase = new ExcluirContatoUseCase(
                obterContatoPorIdUseCase,
                cache.Object,
                _messagePublisher.Object,
                rabbitMqOptions);
            var obterContatosPorDddUseCase = new ObterContatosPorDddUseCase(_repository.Object, cache.Object);
            _controller = new ContatoController(
                obterTodosContatosUseCase,
                obterContatoPorIdUseCase,
                criarContatoUseCase,
                atualizarContatoUseCase,
                excluirContatoUseCase,
                obterContatosPorDddUseCase
            );

            object unusedCacheValue;
            cache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out unusedCacheValue!))
                .Returns(false);
            cache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());
        }

        [Fact]
        public async Task ObterTodosContatos_DeveRetornarListaVazia_QuandoNaoHaContatos()
        {
            _repository.Setup(r => r.ObterTodosAsync()).ReturnsAsync([]);

            var resultado = await _controller.ObterTodosContatos();

            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeAssignableTo<IEnumerable<ContatoDto>>();
        }

        [Fact]
        public async Task ObterTodosContatos_DeveRetornarOk_QuandoHaContatos()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterTodosAsync()).ReturnsAsync([contato]);

            var resultado = await _controller.ObterTodosContatos();

            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeAssignableTo<IEnumerable<ContatoDto>>();
        }

        [Fact]
        public async Task ObterTodosContatosDDD_DeveRetornarOk_QuandoNaoHaContatos()
        {
            _repository.Setup(r => r.ObterPorDddsAsync(It.IsAny<int>())).ReturnsAsync([]);

            var resultado = await _controller.ObterTodosContatosDDD(32);

            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeAssignableTo<IEnumerable<ContatoDto>>();
        }

        [Fact]
        public async Task ObterTodosContatosDDD_DeveRetornarOk_QuandoHaContatos()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorDddsAsync(It.IsAny<int>())).ReturnsAsync([contato]);

            var resultado = await _controller.ObterTodosContatosDDD(32);

            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeAssignableTo<IEnumerable<ContatoDto>>();
        }

        [Fact]
        public async Task ObterContatoPorId_DeveRetornarNotFound_QuandoNaoHaContatos()
        {
            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Contato)null!);

            var resultado = await _controller.ObterContatoPorId(32);

            resultado.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ObterContatoPorId_DeveRetornarOk_QuandoHaContatos()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);

            var resultado = await _controller.ObterContatoPorId(32);

            resultado.Should().BeOfType<OkObjectResult>();
            var okResult = resultado as OkObjectResult;
            okResult!.Value.Should().BeOfType<ContatoDto>();
        }

        [Fact]
        public async Task ObterContatoPorId_DeveRetornar500_QuandoLancarExcecao()
        {
            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).Throws<Exception>();

            var resultado = await _controller.ObterContatoPorId(32);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);

        }

        [Fact]
        public async Task CriarContato_DeveRetornarBadRequest_QuandoDtoForNulo()
        {
            var resultado = await _controller.CriarContato(null!);

            resultado.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = resultado as BadRequestObjectResult;
            objectResult!.Value.Should().Be("Dados inválidos");
        }

        [Fact]
        public async Task CriarContato_DeveRetornarBadRequest_QuandoDtoForInvalido()
        {
            var contato = new CriarContatoDto { Email = "teste@teste.com", Telefone = "32999999999" };

            var resultado = await _controller.CriarContato(contato);

            resultado.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CriarContato_DeveRetornarAccepted_QuandoDtoForValido()
        {
            var contato = new CriarContatoDto { Email = "teste@teste.com", Nome = "Teste", Telefone = "32999999999" };

            var resultado = await _controller.CriarContato(contato);

            resultado.Should().BeOfType<AcceptedResult>();
        }

        [Fact]
        public async Task CriarContato_DeveRetornarConflict_QuandoContatoJaExistir()
        {
            _repository.Setup(r => r.ObterPorNomeEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            });

            var contato = new CriarContatoDto { Email = "teste@teste.com", Nome = "Teste", Telefone = "32999999999" };

            var resultado = await _controller.CriarContato(contato);

            resultado.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task CriarContato_DeveRetornar502_QuandoFalharAoPublicarMensagem()
        {
            _messagePublisher.Setup(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>())).Throws<ApplicationException>();

            var contato = new CriarContatoDto { Email = "teste@teste.com", Nome = "Teste", Telefone = "32999999999" };

            var resultado = await _controller.CriarContato(contato);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(502);
        }

        [Fact]
        public async Task CriarContato_DeveRetornar500_QuandoLancarExcecao()
        {
            _repository.Setup(r => r.ObterPorNomeEmailAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var contato = new CriarContatoDto { Email = "teste@teste.com", Nome = "Teste", Telefone = "32999999999" };

            var resultado = await _controller.CriarContato(contato);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornarBadRequest_QuandoDtoForNulo()
        {
            var resultado = await _controller.AtualizarContato(1, null!);

            resultado.Should().BeOfType<BadRequestObjectResult>();
            var objectResult = resultado as BadRequestObjectResult;
            objectResult!.Value.Should().Be("Dados inválidos");
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornarBadRequest_QuandoDtoForInvalido()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);

            var contatoDto = new AtualizarContatoDto { Id = 1, Telefone = "32999999999" };

            var resultado = await _controller.AtualizarContato(1, contatoDto);

            resultado.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornarAccepted_QuandoDtoForValido()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);

            var contatoDto = new AtualizarContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "32999999999" };
            var resultado = await _controller.AtualizarContato(1, contatoDto);

            resultado.Should().BeOfType<AcceptedResult>();
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
        {
            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Contato)null!);

            var contatoDto = new AtualizarContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "32999999999" };

            var resultado = await _controller.AtualizarContato(1, contatoDto);

            resultado.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornar500_QuandoLancarExcecao()
        {
            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).Throws<Exception>();

            var contatoDto = new AtualizarContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "32999999999" };

            var resultado = await _controller.AtualizarContato(1, contatoDto);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AtualizarContato_DeveRetornar502_QuandoFalharAoPublicarMensagem()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);
            _messagePublisher.Setup(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>())).Throws<ApplicationException>();

            var contatoDto = new AtualizarContatoDto { Id = 1, Nome = "Teste", Email = "teste@teste.com", Telefone = "32999999999" };

            var resultado = await _controller.AtualizarContato(1, contatoDto);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(502);
        }

        [Fact]
        public async Task ExcluirContato_DeveRetornarAccepted_QuandoIdForValido()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);

            var resultado = await _controller.ExcluirContato(1);

            resultado.Should().BeOfType<AcceptedResult>();
        }

        [Fact]
        public async Task ExcluirContato_DeveRetornarNotFound_QuandoUsuarioNaoExistir()
        {
            var resultado = await _controller.ExcluirContato(1);

            resultado.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ExcluirContato_DeveRetornar500_QuandoLancarExcecao()
        {
            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).Throws<Exception>();

            var resultado = await _controller.ExcluirContato(1);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ExcluirContato_DeveRetornar502_QuandoFalharAoPublicarMensagem()
        {
            var contato = new Contato
            {
                Id = 1,
                Nome = "Teste",
                Email = "teste@teste.com",
                Telefone = "32999999999"
            };

            _repository.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync(contato);
            _messagePublisher.Setup(m => m.PublishAsync<It.IsAnyType>(It.IsAny<It.IsAnyType>(), It.IsAny<string>())).Throws<ApplicationException>();

            var resultado = await _controller.ExcluirContato(1);

            resultado.Should().BeOfType<ObjectResult>();
            var objectResult = resultado as ObjectResult;
            objectResult!.StatusCode.Should().Be(502);
        }
    }
}
