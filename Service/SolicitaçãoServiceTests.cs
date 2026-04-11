using CrudCafeteria.DTOs;
using CrudCafeteria.Models.Entidade;
using CrudCafeteria.Models.Enums;
using CrudCafeteria.Repositories;
using CrudCafeteria.Services;
using Moq;

namespace CrudCafeteriaTest.Service
{
    public class SolicitacaoServiceTests
    {
        // =====================
        // UPDATE
        // =====================

        [Fact]
        public async Task Update_DeveRetornarFalse_QuandoIdNaoExiste()
        {
            // ARRANGE
            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(99))
                .ReturnsAsync((SolicitacaoManutencao)null);

            var service = new SolicitacaoService(repositorioFalso.Object);
            var dto = new UpdateSolicitacaoDto { NomeMaquina = "Cafeteira 99" };

            // ACT
            var resultado = await service.Update(99, dto);

            // ASSERT
            Assert.False(resultado);
        }

        [Fact]
        public async Task Update_DeveRetornarTrue_QuandoIdExiste()
        {
            // ARRANGE
            var solicitacaoExistente = new SolicitacaoManutencao
            {
                Id = 1,
                NomeMaquina = "Cafeteira Velha"
            };

            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(1))
                .ReturnsAsync(solicitacaoExistente);

            var service = new SolicitacaoService(repositorioFalso.Object);
            var dto = new UpdateSolicitacaoDto { NomeMaquina = "Cafeteira Nova" };

            // ACT
            var resultado = await service.Update(1, dto);

            // ASSERT
            Assert.True(resultado);
            Assert.Equal("Cafeteira Nova", solicitacaoExistente.NomeMaquina);
        }

        [Fact]
        public async Task Update_NaoDeveAlterarCampo_QuandoNaoEnviado()
        {
            // ARRANGE — envia dto sem NomeMaquina, deve manter o valor antigo
            var solicitacaoExistente = new SolicitacaoManutencao
            {
                Id = 1,
                NomeMaquina = "Cafeteira Original",
                Localizacao = "Sala RH"
            };

            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(1))
                .ReturnsAsync(solicitacaoExistente);

            var service = new SolicitacaoService(repositorioFalso.Object);
            var dto = new UpdateSolicitacaoDto { Localizacao = "Sala TI" }; // só muda localizacao

            // ACT
            await service.Update(1, dto);

            // ASSERT — NomeMaquina deve continuar o mesmo
            Assert.Equal("Cafeteira Original", solicitacaoExistente.NomeMaquina);
            Assert.Equal("Sala TI", solicitacaoExistente.Localizacao);
        }

        // =====================
        // DELETE
        // =====================

        [Fact]
        public async Task Delete_DeveRetornarFalse_QuandoIdNaoExiste()
        {
            // ARRANGE
            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(99))
                .ReturnsAsync((SolicitacaoManutencao)null);

            var service = new SolicitacaoService(repositorioFalso.Object);

            // ACT
            var resultado = await service.Delete(99);

            // ASSERT
            Assert.False(resultado);
        }

        [Fact]
        public async Task Delete_DeveRetornarTrue_QuandoIdExiste()
        {
            // ARRANGE
            var solicitacao = new SolicitacaoManutencao { Id = 1, NomeMaquina = "Cafeteira 01" };

            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(1))
                .ReturnsAsync(solicitacao);

            var service = new SolicitacaoService(repositorioFalso.Object);

            // ACT
            var resultado = await service.Delete(1);

            // ASSERT
            Assert.True(resultado);
        }

        [Fact]
        public async Task Delete_DeveChamarRepositorio_QuandoIdExiste()
        {
            // ARRANGE — verifica se o Delete do repositório foi chamado de verdade
            var solicitacao = new SolicitacaoManutencao { Id = 1, NomeMaquina = "Cafeteira 01" };

            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(1))
                .ReturnsAsync(solicitacao);

            var service = new SolicitacaoService(repositorioFalso.Object);

            // ACT
            await service.Delete(1);

            // ASSERT — confirma que o Delete foi chamado exatamente 1 vez
            repositorioFalso.Verify(r => r.Delete(solicitacao), Times.Once);
        }

        // =====================
        // GET BY ID
        // =====================

        [Fact]
        public async Task GetById_DeveRetornarNull_QuandoIdNaoExiste()
        {
            // ARRANGE
            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(99))
                .ReturnsAsync((SolicitacaoManutencao)null);

            var service = new SolicitacaoService(repositorioFalso.Object);

            // ACT
            var resultado = await service.GetById(99);

            // ASSERT
            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetById_DeveRetornarDto_QuandoIdExiste()
        {
            // ARRANGE
            var solicitacao = new SolicitacaoManutencao
            {
                Id = 1,
                NomeMaquina = "Cafeteira 01",
                Localizacao = "Sala RH",
                DescricaoProblema = "Não aquece",
                Prioridade = Prioridade.Alta,
                Status = Status.Aberta,
                DataAbertura = DateTime.UtcNow
            };

            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            repositorioFalso
                .Setup(r => r.GetById(1))
                .ReturnsAsync(solicitacao);

            var service = new SolicitacaoService(repositorioFalso.Object);

            // ACT
            var resultado = await service.GetById(1);

            // ASSERT
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Cafeteira 01", resultado.NomeMaquina);
            Assert.Equal("Sala RH", resultado.Localizacao);
        }

        // =====================
        // CREATE
        // =====================

        [Fact]
        public async Task Create_DeveCriarComStatusAberta_Sempre()
        {
            // ARRANGE — independente do que vier no dto, status deve ser Aberta
            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            // captura o objeto salvo para verificar depois
            SolicitacaoManutencao? solicitacaoSalva = null;

            repositorioFalso
                .Setup(r => r.Add(It.IsAny<SolicitacaoManutencao>()))
                .Callback<SolicitacaoManutencao>(s => solicitacaoSalva = s)
                .Returns(Task.CompletedTask);

            // GetById chamado após o Add para montar o ResponseDto
            repositorioFalso
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => solicitacaoSalva);

            var service = new SolicitacaoService(repositorioFalso.Object);

            var dto = new CreateSolicitacaoDto
            {
                NomeMaquina = "Cafeteira 02",
                Localizacao = "Sala TI",
                DescricaoProblema = "Vazando água",
                Prioridade = Prioridade.Alta
            };

            // ACT
            await service.Create(dto);

            // ASSERT — status sempre começa como Aberta
            Assert.NotNull(solicitacaoSalva);
            Assert.Equal(Status.Aberta, solicitacaoSalva!.Status);
        }

        [Fact]
        public async Task Create_DevePreencherDataAbertura_Automaticamente()
        {
            // ARRANGE
            var repositorioFalso = new Mock<ISolicitacaoRepository>();

            SolicitacaoManutencao? solicitacaoSalva = null;
            var antes = DateTime.UtcNow;

            repositorioFalso
                .Setup(r => r.Add(It.IsAny<SolicitacaoManutencao>()))
                .Callback<SolicitacaoManutencao>(s => solicitacaoSalva = s)
                .Returns(Task.CompletedTask);

            repositorioFalso
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(() => solicitacaoSalva);

            var service = new SolicitacaoService(repositorioFalso.Object);

            var dto = new CreateSolicitacaoDto
            {
                NomeMaquina = "Cafeteira 03",
                Localizacao = "Recepção",
                DescricaoProblema = "Sem pressão",
                Prioridade = Prioridade.Media
            };

            // ACT
            await service.Create(dto);

            var depois = DateTime.UtcNow;

            // ASSERT — DataAbertura foi preenchida automaticamente entre antes e depois
            Assert.NotNull(solicitacaoSalva);
            Assert.True(solicitacaoSalva!.DataAbertura >= antes);
            Assert.True(solicitacaoSalva!.DataAbertura <= depois);
        }
    }
}