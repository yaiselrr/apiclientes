using APIClientes.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIClientes.Repository
{
    public interface IClientesRepository
    {
        // AQUI EN LA INTERFAZ ESTAN TODOS LOS METODOS QUE VAMOS A CREAR PARA TRABAJAR CLIENTES EN EL REPOSITORIO
        Task<List<ClienteDto>> GetClientes();
        Task<ClienteDto> GetClienteById(int id);
        Task<ClienteDto> CreateUpdate(ClienteDto clienteDto);
        Task<bool> DeleteCliente(int id);
    }
}
